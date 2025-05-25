using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using EvacProject.GENERAL.Data;
using EvacProject.GENERAL.Entity;
using Telegram.Bot.Types.ReplyMarkups;

namespace EvacProject.Services
{
    public class TelegramBotService : IHostedService, ITelegramBotService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TelegramBotService> _logger;
        private readonly Dictionary<long, string> _userStates = new();
        private readonly Dictionary<long, string> _userRoles = new();
        private readonly Dictionary<long, UserTestState> _userTestStates = new();
        private readonly Dictionary<long, string> _pendingHelpMessages = new();
        private readonly WeatherService _weatherService;

        public TelegramBotService(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            IWebHostEnvironment environment,
            WeatherService weatherService,
            ILogger<TelegramBotService> logger,
            ITelegramBotClient botClient)
        {
            _logger = logger;
            _logger.LogInformation("TelegramBotService: Constructor called");
            try
            {
                _botClient = botClient ?? new TelegramBotClient("7739045967:AAH9D4tGz7v8H7vZC67b3yI6D3diwDy5i0U");
                _scopeFactory = scopeFactory;
                _weatherService = weatherService;
                _environment = environment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize TelegramBotService");
                throw;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TelegramBotService: StartAsync called");
            try
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync("https://api.telegram.org", cancellationToken);
                _logger.LogInformation($"Telegram API reachability: {response.StatusCode}");

                var botInfo = await _botClient.GetMe(cancellationToken);
                _logger.LogInformation($"Bot started: @{botInfo.Username}");

                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = new[] { UpdateType.Message }
                };

                _botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    errorHandler: HandleErrorAsync,
                    receiverOptions: receiverOptions,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Telegram bot polling started");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start Telegram bot");
                throw;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TelegramBotService: StopAsync called");
            await Task.CompletedTask;
        }

        public async Task SendMessageToAllStudents(string message)
        {
            _logger.LogInformation("SendMessageToAllStudents: Starting to send message to all students");
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var studentChatIds = await dbContext.Students
                .Where(s => s.TelegramChatId != null)
                .Select(s => s.TelegramChatId)
                .ToListAsync();

            _logger.LogInformation($"Found {studentChatIds.Count} students with TelegramChatId");
            foreach (var chatId in studentChatIds)
            {
                try
                {
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: message,
                        cancellationToken: CancellationToken.None);
                    _logger.LogInformation($"Message sent to chat {chatId}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send message to chat {chatId}");
                }
            }
            _logger.LogInformation("SendMessageToAllStudents: Completed");
        }
        
        public async Task StartEvacuationTestAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TelegramBotService: Starting evacuation test");
            try
            {
                var students = await dbContext.Students
                    .Where(s => !string.IsNullOrEmpty(s.TelegramChatId))
                    .ToListAsync(cancellationToken);

                var keyboard = new ReplyKeyboardMarkup(new[]
                {
                    new[] { new KeyboardButton("1"), new KeyboardButton("2") },
                    new[] { new KeyboardButton("3"), new KeyboardButton("4") }
                })
                {
                    OneTimeKeyboard = true
                };

                foreach (var student in students)
                {
                    try
                    {
                        student.CurrentState = EvacuationState.WaitingForCampus.ToString();
                        student.SelectedCampus = null;
        
                        await _botClient.SendTextMessageAsync(
                            chatId: long.Parse(student.TelegramChatId),
                            text: "ВНИМАНИЕ! В здании университета произошло ЧС, немедленно эвакуируйтесь\n" +
                                  "Пожалуйста, укажите, в каком корпусе вы находитесь:",
                            replyMarkup: keyboard,
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"Sent evacuation alert to student: StudentNumber={student.StudentNumber}, ChatId={student.TelegramChatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to send evacuation alert to student: StudentNumber={student.StudentNumber}, ChatId={student.TelegramChatId}");
                    }
                }
        
                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation($"Evacuation test started: NotifiedStudents={students.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting evacuation test");
                throw;
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Received update: Type={update.Type}, ChatId={update.Message?.Chat.Id}, Text={update.Message?.Text}");
                if (update.Type == UpdateType.Message && (update.Message?.Text != null || update.Message?.Location != null))
                {
                    var chatId = update.Message.Chat.Id;
                    var messageText = update.Message.Text?.ToLower() ?? string.Empty;

                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var student = await dbContext.Students
                        .FirstOrDefaultAsync(s => s.TelegramChatId != null && s.TelegramChatId == chatId.ToString(), cancellationToken);
                    var admin = await dbContext.Admins
                        .FirstOrDefaultAsync(a => a.TelegramChatId != null && a.TelegramChatId == chatId.ToString(), cancellationToken);

                    if (student != null || admin != null)
                    {
                        await HandleAuthenticatedUser(chatId, messageText, student, admin, dbContext, cancellationToken, update);
                    }
                    else
                    {
                        await HandleUnauthenticatedUser(chatId, messageText, dbContext, cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Telegram update");
            }
        }

        private async Task HandleAuthenticatedUser(long chatId, string messageText, Student student, Admin admin, ApplicationDbContext dbContext, CancellationToken cancellationToken, Update update)
        {
            try
            {
                if (messageText == "/logout")
                {
                    if (student != null)
                    {
                        student.TelegramChatId = null;
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }
                    else if (admin != null)
                    {
                        admin.TelegramChatId = null;
                        await dbContext.SaveChangesAsync(cancellationToken);
                    }

                    _userStates.Remove(chatId);
                    _pendingHelpMessages.Remove(chatId);
                    _userTestStates.Remove(chatId);

                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Вы успешно вышли из системы.",
                        cancellationToken: cancellationToken);
                    _logger.LogInformation($"User logged out: ChatId={chatId}");
                    return;
                }
                
                if (messageText.StartsWith("/help"))
                {
                    if (student == null)
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Команда /help доступна только студентам. Пожалуйста, авторизуйтесь как студент.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    try
                    {
                        string helpMessage = messageText.Length > 5 ? messageText.Substring(5).Trim() : string.Empty;
                        if (string.IsNullOrWhiteSpace(helpMessage))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Пожалуйста, укажите сообщение после команды /help. Например: /help Нужна помощь на этаже 3",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        _pendingHelpMessages[chatId] = helpMessage;
                        _userStates[chatId] = "waiting_for_location";

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Пожалуйста, отправьте вашу геолокацию или напишите 'без геолокации' для продолжения.",
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"Waiting for location for help message: Student={student.FirstName} {student.LastName}, Message={helpMessage}, ChatId={chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing /help command");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при обработке команды /help. Попробуйте позже.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                if (_userStates.ContainsKey(chatId) && _userStates[chatId] == "waiting_for_location")
                {
                    try
                    {
                        if (!_pendingHelpMessages.ContainsKey(chatId))
                        {
                            _userStates.Remove(chatId);
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Сессия /help истекла. Пожалуйста, отправьте команду /help заново.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var helpMessage = _pendingHelpMessages[chatId];
                        var helpMsg = new HelpMessage
                        {
                            StudentFullName = $"{student.FirstName} {student.LastName}",
                            SentAt = DateTime.UtcNow,
                            MessageText = helpMessage,
                            TelegramChatId = chatId.ToString()
                        };

                        if (update.Message?.Location != null)
                        {
                            helpMsg.Latitude = update.Message.Location.Latitude;
                            helpMsg.Longitude = update.Message.Location.Longitude;
                            _logger.LogInformation($"Received location: Latitude={helpMsg.Latitude}, Longitude={helpMsg.Longitude}");
                        }
                        else if (messageText == "без геолокации")
                        {
                            _logger.LogInformation("User skipped location");
                        }
                        else
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Пожалуйста, отправьте геолокацию или напишите 'без геолокации'.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        dbContext.HelpMessages.Add(helpMsg);
                        await dbContext.SaveChangesAsync(cancellationToken);

                        _userStates.Remove(chatId);
                        _pendingHelpMessages.Remove(chatId);

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Ваше сообщение отправлено администрации. Мы скоро свяжемся с вами.",
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"Help message saved: Student={helpMsg.StudentFullName}, Message={helpMsg.MessageText}, ChatId={chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving help message with location");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при сохранении сообщения. Попробуйте позже.",
                            cancellationToken: cancellationToken);
                        _userStates.Remove(chatId);
                        _pendingHelpMessages.Remove(chatId);
                    }
                    return;
                }

                if (messageText.StartsWith("/danger"))
                {
                    if (student == null)
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Команда /danger доступна только студентам. Пожалуйста, авторизуйтесь как студент.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    try
                    {
                        var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length < 2)
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Введите название региона после команды /danger, например: /danger Минск",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var region = string.Join(" ", parts[1..]);
                        _logger.LogInformation($"Handling /danger for region: {region}, ChatId={chatId}");

                        var weatherData = await _weatherService.GetWeatherByCity(region);
                        var dangerLevel = _weatherService.DetermineDangerLevel(weatherData);
                        var responseText = $"Погода в {region}:\n" +
                                           $"Температура: {(double)weatherData["main"]["temp"]:F1}°С,\n" +
                                           $"Ветер: {(double)weatherData["wind"]["speed"]:F1} м/с,\n" +
                                           $"Уровень опасности: {dangerLevel}";

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: responseText,
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"Sent weather data for region: {region}, DangerLevel={dangerLevel}, ChatId={chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error handling /danger for region, ChatId={chatId}");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Ошибка: Не удалось получить данные о погоде. Проверьте название региона.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                // Обработка геолокации для /danger (без команды)
                if (update.Message?.Location != null)
                {
                    if (student == null)
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Отправка геолокации для /danger доступна только студентам. Пожалуйста, авторизуйтесь как студент.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    try
                    {
                        var location = update.Message.Location;
                        _logger.LogInformation($"Handling /danger with location: Lat={location.Latitude}, Lon={location.Longitude}, ChatId={chatId}");

                        var weatherData = await _weatherService.GetWeatherByCoordinates(location.Latitude, location.Longitude);
                        var dangerLevel = _weatherService.DetermineDangerLevel(weatherData);
                        var responseText = "Погода по координатам:\n" +
                                           $"Температура: {(double)weatherData["main"]["temp"]:F1}°С,\n" +
                                           $"Ветер: {(double)weatherData["wind"]["speed"]:F1} м/с,\n" +
                                           $"Уровень опасности: {dangerLevel}";

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: responseText,
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"Sent weather data for coordinates: Lat={location.Latitude}, Lon={location.Longitude}, DangerLevel={dangerLevel}, ChatId={chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error handling /danger for coordinates, ChatId={chatId}");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Ошибка при получении данных о погоде.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }
                
                if (messageText == "/subscribe")
                {
                    if (student == null)
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Команда /subscribe доступна только студентам. Пожалуйста, авторизуйтесь как студент.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    try
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "subscribers.json");
                        if (!System.IO.File.Exists(filePath))
                        {
                            await System.IO.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(new SubscribersData()), cancellationToken);
                            _logger.LogInformation($"Created subscribers.json at {filePath}");
                        }

                        var jsonContent = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
                        var subscribersData = JsonSerializer.Deserialize<SubscribersData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new SubscribersData();

                        if (subscribersData.Subscribers.Any(s => s.TelegramChatId == chatId.ToString()))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Вы уже подписаны на новостную рассылку.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        subscribersData.Subscribers.Add(new Subscriber
                        {
                            StudentNumber = student.StudentNumber,
                            TelegramChatId = chatId.ToString()
                        });

                        await System.IO.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(subscribersData, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Вы успешно подписаны на новостную рассылку! Новости будут приходить каждый день в 7:00.",
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"User subscribed to news: StudentNumber={student.StudentNumber}, ChatId={chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error handling /subscribe, ChatId={chatId}");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при подписке. Попробуйте позже.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                if (messageText == "/unsubscribe")
                {
                    if (student == null)
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Команда /unsubscribe доступна только студентам. Пожалуйста, авторизуйтесь как студент.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    try
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "subscribers.json");
                        if (!System.IO.File.Exists(filePath))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Вы не подписаны на новостную рассылку.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var jsonContent = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
                        var subscribersData = JsonSerializer.Deserialize<SubscribersData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new SubscribersData();

                        var subscriber = subscribersData.Subscribers.FirstOrDefault(s => s.TelegramChatId == chatId.ToString());
                        if (subscriber == null)
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Вы не подписаны на новостную рассылку.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        subscribersData.Subscribers.Remove(subscriber);
                        await System.IO.File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(subscribersData, new JsonSerializerOptions { WriteIndented = true }), cancellationToken);

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Вы успешно отписались от новостной рассылки.",
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"User unsubscribed from news: StudentNumber={student.StudentNumber}, ChatId={chatId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error handling /unsubscribe, ChatId={chatId}");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при отписке. Попробуйте позже.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                if (messageText == "/news")
                {
                    if (student == null)
                    {
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Команда /news доступна только студентам. Пожалуйста, авторизуйтесь как студент.",
                            cancellationToken: cancellationToken);
                        return;
                    }

                    try
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "News.json");
                        if (!System.IO.File.Exists(filePath))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Ошибка: Файл новостей не найден.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var jsonContent = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
                        var newsData = JsonSerializer.Deserialize<NewsData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (newsData?.News == null || !newsData.News.Any())
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Новости отсутствуют.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var responseText = "Новости:\n" + string.Join("\n\n", newsData.News.Select(n => $"{n.Title}\n{n.Description}\nДата: {n.Date}"));
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: responseText,
                            cancellationToken: cancellationToken);
                        _logger.LogInformation($"Sent news to user: ChatId={chatId}, NewsCount={newsData.News.Count}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error handling /news, ChatId={chatId}");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Ошибка: Не удалось получить новости.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                if (messageText == "/history")
                {
                    try
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "Info.json");
                        if (!System.IO.File.Exists(filePath))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Ошибка: Файл Info.json не найден.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var jsonContent = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
                        var historyData = JsonSerializer.Deserialize<HistoryData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (historyData?.History == null || !historyData.History.Any())
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "История событий пуста.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var response = "История событий:\n" + string.Join("\n", historyData.History.Select(kvp => $"{kvp.Key} - {kvp.Value.Количество}"));
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: response,
                            cancellationToken: cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing /history command");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при получении истории.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                if (messageText == "/kit")
                {
                    try
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "Info.json");
                        if (!System.IO.File.Exists(filePath))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Ошибка: Файл Info.json не найден.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var jsonContent = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
                        var kitData = JsonSerializer.Deserialize<KitData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (kitData?.Эвакуационный_рюкзак == null || !kitData.Эвакуационный_рюкзак.Any())
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Список предметов эвакуационного рюкзака пуст.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var items = string.Join("\n", kitData.Эвакуационный_рюкзак.Select(item => $"- {item.Название}: {item.Описание}"));
                        var response = "Ваш базовый эвакуационный рюкзак должен содержать:\nПредметы:\n" + items;

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: response,
                            cancellationToken: cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing /kit command");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при получении списка предметов.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                if (messageText == "/test")
                {
                    try
                    {
                        var filePath = Path.Combine(_environment.ContentRootPath, "Info.json");
                        if (!System.IO.File.Exists(filePath))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Ошибка: Файл Info.json не найден.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var jsonContent = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
                        var testData = JsonSerializer.Deserialize<TestData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (testData?.Вопросы == null || !testData.Вопросы.Any())
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Вопросы для теста отсутствуют.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        _userTestStates[chatId] = new UserTestState { IsTesting = true, CurrentQuestionIndex = 0, CorrectAnswers = 0 };
                        _userStates[chatId] = "testing";

                        var question = testData.Вопросы[0];
                        var response = $"Вопрос 1:\n{question.Вопрос}\n" +
                                       $"1. {question.Варианты[0]}\n" +
                                       $"2. {question.Варианты[1]}\n" +
                                       $"3. {question.Варианты[2]}\n" +
                                       "Введите номер ответа (1, 2 или 3):";

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: response,
                            cancellationToken: cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing /test command");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при запуске теста.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }

                if (_userStates.ContainsKey(chatId) && _userStates[chatId] == "testing")
                {
                    try
                    {
                        if (!int.TryParse(messageText, out int answerIndex) || answerIndex < 1 || answerIndex > 3)
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Пожалуйста, введите корректный номер ответа (1, 2 или 3).",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var filePath = Path.Combine(_environment.ContentRootPath, "Info.json");
                        if (!System.IO.File.Exists(filePath))
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Ошибка: Файл Info.json не найден.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var jsonContent = await System.IO.File.ReadAllTextAsync(filePath, cancellationToken);
                        var testData = JsonSerializer.Deserialize<TestData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (testData?.Вопросы == null || !testData.Вопросы.Any())
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Вопросы для теста отсутствуют.",
                                cancellationToken: cancellationToken);
                            return;
                        }

                        var testState = _userTestStates[chatId];
                        var currentQuestion = testData.Вопросы[testState.CurrentQuestionIndex];

                        var userAnswer = currentQuestion.Варианты[answerIndex - 1];
                        bool isCorrect = userAnswer == currentQuestion.Правильный_ответ;
                        if (isCorrect)
                        {
                            testState.CorrectAnswers++;
                        }

                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: isCorrect ? "Правильно!" : $"Неправильно. Правильный ответ: {currentQuestion.Правильный_ответ}",
                            cancellationToken: cancellationToken);

                        testState.CurrentQuestionIndex++;
                        if (testState.CurrentQuestionIndex < testData.Вопросы.Count)
                        {
                            var nextQuestion = testData.Вопросы[testState.CurrentQuestionIndex];
                            var response = $"Вопрос {testState.CurrentQuestionIndex + 1}:\n{nextQuestion.Вопрос}\n" +
                                           $"1. {nextQuestion.Варианты[0]}\n" +
                                           $"2. {nextQuestion.Варианты[1]}\n" +
                                           $"3. {nextQuestion.Варианты[2]}\n" +
                                           "Введите номер ответа (1, 2 или 3):";

                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: response,
                                cancellationToken: cancellationToken);
                        }
                        else
                        {
                            double scorePercentage = (double)testState.CorrectAnswers / testData.Вопросы.Count * 100;
                            string readinessMessage = scorePercentage switch
                            {
                                >= 80 => "Отличная эвакуационная готовность! Вы хорошо подготовлены.",
                                >= 50 => "Средняя эвакуационная готовность. Рекомендуем повторить правила безопасности.",
                                _ => "Низкая эвакуационная готовность. Пожалуйста, изучите материалы по эвакуации."
                            };

                            var finalResponse = $"Тест завершен!\n" +
                                               $"Правильных ответов: {testState.CorrectAnswers} из {testData.Вопросы.Count}\n" +
                                               $"Ваш результат: {scorePercentage:F0}%\n" +
                                               readinessMessage;

                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: finalResponse,
                                cancellationToken: cancellationToken);

                            _userTestStates.Remove(chatId);
                            _userStates[chatId] = "authenticated";
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing test answer");
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Произошла ошибка при обработке ответа.",
                            cancellationToken: cancellationToken);
                    }
                    return;
                }
                
                if (student != null && student.CurrentState == EvacuationState.WaitingForCampus.ToString())
                {
                    if (new[] { "1", "2", "3", "4" }.Contains(messageText))
                    {
                        try
                        {
                            student.SelectedCampus = messageText;
                            student.CurrentState = EvacuationState.WaitingForFloor.ToString();
                            await dbContext.SaveChangesAsync(cancellationToken);

                            var keyboard = new ReplyKeyboardMarkup(new[]
                            {
                                new[] { new KeyboardButton("1"), new KeyboardButton("2") },
                                new[] { new KeyboardButton("3"), new KeyboardButton("4") },
                                new[] { new KeyboardButton("5") }
                            })
                            {
                                OneTimeKeyboard = true
                            };

                            await _botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Пожалуйста, укажите, на каком этаже вы находитесь:",
                                replyMarkup: keyboard,
                                cancellationToken: cancellationToken);
                            _logger.LogInformation($"Student selected campus: StudentNumber={student.StudentNumber}, Campus={messageText}, ChatId={chatId}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error processing campus selection, ChatId={chatId}");
                            await _botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Произошла ошибка. Попробуйте позже.",
                                cancellationToken: cancellationToken);
                        }
                        return;
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Неверный выбор корпуса. Пожалуйста, выберите 1, 2, 3 или 4.",
                            cancellationToken: cancellationToken);
                        return;
                    }
                }

                if (student != null && student.CurrentState == EvacuationState.WaitingForFloor.ToString())
                {
                    if (new[] { "1", "2", "3", "4", "5" }.Contains(messageText))
                    {
                        try
                        {
                            var campus = student.SelectedCampus;
                            var floor = messageText;
                            var imagePath = Path.Combine(_environment.WebRootPath, "EvacuationPlans", $"Campus_{campus}", $"Flor_{floor}.png");

                            if (System.IO.File.Exists(imagePath))
                            {
                                await using var stream = System.IO.File.OpenRead(imagePath);
                                await _botClient.SendPhotoAsync(
                                    chatId: chatId,
                                    photo: new Telegram.Bot.Types.InputFileStream(stream, $"Flor_{floor}.png"),
                                    caption: "Пожалуйста, следуйте плану эвакуации. Берегите себя!",
                                    replyMarkup: new ReplyKeyboardRemove(),
                                    cancellationToken: cancellationToken);
                                _logger.LogInformation($"Sent evacuation plan: StudentNumber={student.StudentNumber}, Campus={campus}, Floor={floor}, ChatId={chatId}");
                            }
                            else
                            {
                                await _botClient.SendTextMessageAsync(
                                    chatId: chatId,
                                    text: "Извините, план эвакуации не найден.",
                                    replyMarkup: new ReplyKeyboardRemove(),
                                    cancellationToken: cancellationToken);
                                _logger.LogWarning($"Evacuation plan not found: Path={imagePath}, ChatId={chatId}");
                            }

                            student.CurrentState = EvacuationState.None.ToString();
                            student.SelectedCampus = null;
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error sending evacuation plan, ChatId={chatId}");
                            await _botClient.SendTextMessageAsync(
                                chatId: chatId,
                                text: "Произошла ошибка при отправке плана эвакуации. Попробуйте позже.",
                                replyMarkup: new ReplyKeyboardRemove(),
                                cancellationToken: cancellationToken);
                            student.CurrentState = EvacuationState.None.ToString();
                            student.SelectedCampus = null;
                            await dbContext.SaveChangesAsync(cancellationToken);
                        }
                        return;
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: chatId,
                            text: "Неверный выбор этажа. Пожалуйста, выберите 1, 2, 3, 4 или 5.",
                            cancellationToken: cancellationToken);
                        return;
                    }
                }

                await _botClient.SendMessage(
                    chatId: chatId,
                    text: "Команда не распознана. Попробуйте /help, /danger, /subscribe, /unsubscribe, /news, /history, /kit, /test или /logout.",
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling authenticated user: ChatId={chatId}, Message={messageText}");
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: "Произошла ошибка. Попробуйте позже.",
                    cancellationToken: cancellationToken);
            }
        }

        private async Task HandleUnauthenticatedUser(long chatId, string messageText, ApplicationDbContext dbContext, CancellationToken cancellationToken)
        {
            try
            {
                if (!_userStates.ContainsKey(chatId))
                {
                    _userStates[chatId] = "waiting_for_role";
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: "Пожалуйста, укажите, кто вы: студент или преподаватель.",
                        cancellationToken: cancellationToken);
                    return;
                }

                switch (_userStates[chatId])
                {
                    case "waiting_for_role":
                        if (messageText == "студент" || messageText == "преподаватель")
                        {
                            _userRoles[chatId] = messageText;
                            _userStates[chatId] = messageText == "студент" ? "waiting_for_student_number" : "waiting_for_userid";
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: $"Пожалуйста, введите ваш {(messageText == "студент" ? "номер студенческого билета" : "UserID")}.",
                                cancellationToken: cancellationToken);
                        }
                        else
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Неверная роль. Укажите 'студент' или 'преподаватель'.",
                                cancellationToken: cancellationToken);
                        }
                        break;

                    case "waiting_for_student_number":
                        var student = await dbContext.Students.FirstOrDefaultAsync(s => s.StudentNumber == messageText, cancellationToken);
                        if (student != null)
                        {
                            var existingStudent = await dbContext.Students.FirstOrDefaultAsync(s => s.TelegramChatId == chatId.ToString(), cancellationToken);
                            var existingAdmin = await dbContext.Admins.FirstOrDefaultAsync(a => a.TelegramChatId == chatId.ToString(), cancellationToken);
                            if (existingStudent != null) existingStudent.TelegramChatId = null;
                            if (existingAdmin != null) existingAdmin.TelegramChatId = null;

                            student.TelegramChatId = chatId.ToString();
                            await dbContext.SaveChangesAsync(cancellationToken);
                            _userStates[chatId] = "authenticated";
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: $"Вы вошли в систему как {student.FirstName} {student.LastName}.",
                                cancellationToken: cancellationToken);
                        }
                        else
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "Неверный номер студенческого билета. Попробуйте еще раз.",
                                cancellationToken: cancellationToken);
                        }
                        break;

                    case "waiting_for_userid":
                        if (long.TryParse(messageText, out long userId))
                        {
                            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
                            if (admin != null)
                            {
                                var existingStudent = await dbContext.Students.FirstOrDefaultAsync(s => s.TelegramChatId == chatId.ToString(), cancellationToken);
                                var existingAdmin = await dbContext.Admins.FirstOrDefaultAsync(a => a.TelegramChatId == chatId.ToString(), cancellationToken);
                                if (existingStudent != null) existingStudent.TelegramChatId = null;
                                if (existingAdmin != null) existingAdmin.TelegramChatId = null;

                                admin.TelegramChatId = chatId.ToString();
                                await dbContext.SaveChangesAsync(cancellationToken);
                                _userStates[chatId] = "authenticated";
                                await _botClient.SendMessage(
                                    chatId: chatId,
                                    text: $"Вы вошли в систему как {admin.FirstName} {admin.LastName}.",
                                    cancellationToken: cancellationToken);
                            }
                            else
                            {
                                await _botClient.SendMessage(
                                    chatId: chatId,
                                    text: "Неверный UserID. Попробуйте еще раз.",
                                    cancellationToken: cancellationToken);
                            }
                        }
                        else
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: "UserID должен быть числом. Попробуйте еще раз.",
                                cancellationToken: cancellationToken);
                        }
                        break;

                    default:
                        await _botClient.SendMessage(
                            chatId: chatId,
                            text: "Пожалуйста, авторизуйтесь, чтобы использовать бота.",
                            cancellationToken: cancellationToken);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling unauthenticated user: ChatId={chatId}, Message={messageText}");
                await _botClient.SendMessage(
                    chatId: chatId,
                    text: "Произошла ошибка. Попробуйте позже.",
                    cancellationToken: cancellationToken);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "Telegram polling error");
            return Task.CompletedTask;
        }
        
        public class HistoryEvent
        {
            public string Количество { get; set; }
        }

        public class HistoryData
        {
            public Dictionary<string, HistoryEvent> History { get; set; }
        }

        public class KitItem
        {
            public string Название { get; set; }
            public string Описание { get; set; }
        }

        public class KitData
        {
            public List<KitItem> Эвакуационный_рюкзак { get; set; }
        }

        public class TestQuestion
        {
            public string Вопрос { get; set; }
            public List<string> Варианты { get; set; }
            public string Правильный_ответ { get; set; }
        }

        public class TestData
        {
            public List<TestQuestion> Вопросы { get; set; }
        }

        public class UserTestState
        {
            public int CurrentQuestionIndex { get; set; } = 0;
            public int CorrectAnswers { get; set; } = 0;
            public bool IsTesting { get; set; } = false;
        }
    }
}