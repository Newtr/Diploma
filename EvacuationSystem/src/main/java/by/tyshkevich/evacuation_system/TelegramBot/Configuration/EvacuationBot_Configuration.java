package by.tyshkevich.evacuation_system.TelegramBot.Configuration;

import by.tyshkevich.evacuation_system.General.Entity.EvacuationBot;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.telegram.telegrambots.meta.TelegramBotsApi;
import org.telegram.telegrambots.updatesreceivers.DefaultBotSession;


@Configuration
public class EvacuationBot_Configuration
{
    @Bean
    public TelegramBotsApi telegramBotsApi(EvacuationBot bot) throws Exception
    {
        TelegramBotsApi botsApi = new TelegramBotsApi(DefaultBotSession.class);
        botsApi.registerBot(bot);
        return botsApi;
    }
}
