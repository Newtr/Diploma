package by.tyshkevich.evacuation_system.TelegramBot.Handlers;

import by.tyshkevich.evacuation_system.General.Entity.BotCommandHandler;
import by.tyshkevich.evacuation_system.General.Services.WeatherService;
import by.tyshkevich.evacuation_system.General.Services.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.objects.Location;
import org.telegram.telegrambots.meta.api.objects.Update;
import org.json.JSONObject;

@Component
public class DangerCheckCommandHandler implements BotCommandHandler
{

    @Autowired
    private UserService userService;

    @Autowired
    private WeatherService weatherService;

    @Autowired
    private EvacuationBotHelper botHelper;

    @Override
    public boolean canHandle(Update update)
    {
        return update.hasMessage() &&
                ((update.getMessage().hasText() && update.getMessage().getText().startsWith("/danger"))
                        || update.getMessage().hasLocation());
    }

    @Override
    public void handle(Update update)
    {
        long chatId = update.getMessage().getChatId();
        if (!userService.isUserLoggedIn(chatId))
        {
            botHelper.sendMessage(chatId, "Доступ запрещён. Введите /login <номер студенческого> для входа в систему.");
            return;
        }

        if (update.getMessage().hasLocation())
        {
            Location loc = update.getMessage().getLocation();
            try
            {
                JSONObject weatherData = weatherService.getWeatherByCoordinates(loc.getLatitude(), loc.getLongitude());
                String dangerLevel = weatherService.determineDangerLevel(weatherData);
                String responseText = String.format("Погода по координатам:\nТемпература: %.1f°С,\nВетер: %.1f м/с,\nУровень опасности: *%s*",
                        weatherData.getJSONObject("main").getDouble("temp"),
                        weatherData.getJSONObject("wind").getDouble("speed"),
                        dangerLevel);
                botHelper.sendMessage(chatId, responseText);
            }
            catch (Exception e)
            {
                e.printStackTrace();
                botHelper.sendMessage(chatId, "Ошибка при получении данных о погоде.");
            }
        }
        else
        {
            String msg = update.getMessage().getText();
            String[] parts = msg.split(" ");
            if (parts.length < 2)
            {
                botHelper.sendMessage(chatId, "Введите название региона после команды /danger, например: /danger Минск");
                return;
            }
            String region = msg.substring(msg.indexOf(" ") + 1);
            try
            {
                JSONObject weatherData = weatherService.getWeatherByCity(region);
                String dangerLevel = weatherService.determineDangerLevel(weatherData);
                String responseText = String.format("Погода в %s:\nТемпература: %.1f°С,\nВетер: %.1f м/с,\nУровень опасности: *%s*",
                        region,
                        weatherData.getJSONObject("main").getDouble("temp"),
                        weatherData.getJSONObject("wind").getDouble("speed"),
                        dangerLevel);
                botHelper.sendMessage(chatId, responseText);
            }
            catch (Exception e)
            {
                e.printStackTrace();
                botHelper.sendMessage(chatId, "Ошибка: Не удалось получить данные о погоде. Проверьте название региона.");
            }
        }
    }
}
