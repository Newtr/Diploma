using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EvacProject.GENERAL.Entity;
using Telegram.Bot;

namespace EvacProject.Services
{
    public class NewsNotificationService : BackgroundService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<NewsNotificationService> _logger;

        public NewsNotificationService(
            ITelegramBotClient botClient,
            IWebHostEnvironment environment,
            ILogger<NewsNotificationService> logger)
        {
            _botClient = botClient;
            _environment = environment;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("NewsNotificationService: Started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextRun = now.Date.AddHours(7);  //Для проверки var nextRun = now.AddMinutes(1); и закоментить проверку 
                    if (now.Hour >= 7)
                    {
                        nextRun = nextRun.AddDays(1);
                    }

                    var delay = nextRun - now;
                    _logger.LogInformation($"NewsNotificationService: Next news broadcast scheduled at {nextRun}");
                    await Task.Delay(delay, stoppingToken);

                    await SendNewsToSubscribers(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "NewsNotificationService: Error in news broadcast");
                }
            }
        }

        private async Task SendNewsToSubscribers(CancellationToken cancellationToken)
        {
            _logger.LogInformation("NewsNotificationService: Sending news to subscribers");
            try
            {
                var news = await LoadNewsAsync();
                if (news == null || !news.News.Any())
                {
                    _logger.LogInformation("NewsNotificationService: No news available to send");
                    return;
                }

                var newsText = "Ежедневные новости:\n" + string.Join("\n\n", news.News.Select(n => $"{n.Title}\n{n.Description}\nДата: {n.Date}"));

                var subscribers = await LoadSubscribersAsync();
                _logger.LogInformation($"NewsNotificationService: Found {subscribers.Subscribers.Count} subscribers");
                foreach (var subscriber in subscribers.Subscribers)
                {
                    if (long.TryParse(subscriber.TelegramChatId, out long chatId))
                    {
                        try
                        {
                            await _botClient.SendMessage(
                                chatId: chatId,
                                text: newsText,
                                cancellationToken: cancellationToken);
                            _logger.LogInformation($"NewsNotificationService: News sent to ChatId={chatId}, StudentNumber={subscriber.StudentNumber}");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"NewsNotificationService: Failed to send news to ChatId={chatId}, StudentNumber={subscriber.StudentNumber}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NewsNotificationService: Error sending news to subscribers");
            }
        }

        private async Task<NewsData> LoadNewsAsync()
        {
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "News.json");
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"NewsNotificationService: News file not found at {filePath}");
                    return null;
                }

                var jsonContent = await File.ReadAllTextAsync(filePath);
                var newsData = JsonSerializer.Deserialize<NewsData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                _logger.LogInformation($"NewsNotificationService: Loaded {newsData?.News?.Count ?? 0} news items");
                return newsData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NewsNotificationService: Error loading news file");
                return null;
            }
        }

        private async Task<SubscribersData> LoadSubscribersAsync()
        {
            try
            {
                var filePath = Path.Combine(_environment.ContentRootPath, "subscribers.json");
                if (!File.Exists(filePath))
                {
                    _logger.LogError($"NewsNotificationService: Subscribers file not found at {filePath}");
                    return new SubscribersData();
                }

                var jsonContent = await File.ReadAllTextAsync(filePath);
                var subscribersData = JsonSerializer.Deserialize<SubscribersData>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new SubscribersData();
                _logger.LogInformation($"NewsNotificationService: Loaded {subscribersData.Subscribers.Count} subscribers");
                return subscribersData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NewsNotificationService: Error loading subscribers file");
                return new SubscribersData();
            }
        }
    }
}