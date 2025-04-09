package by.tyshkevich.evacuation_system.TelegramBot.Handlers;

import by.tyshkevich.evacuation_system.General.Entity.BotCommandHandler;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.objects.Update;

@Component
public class StartCommandHandler implements BotCommandHandler
{

    @Autowired
    private EvacuationBotHelper botHelper;

    @Override
    public boolean canHandle(Update update)
    {
        return update.hasMessage() &&
                update.getMessage().hasText() &&
                update.getMessage().getText().equalsIgnoreCase("/start");
    }

    @Override
    public void handle(Update update)
    {
        long chatId = update.getMessage().getChatId();
        String welcome = "Привет! Чтобы начать пользоваться ботом, войдите в систему, введя команду:\n" +
                "/login <номер студенческого>";
        botHelper.sendMessage(chatId, welcome);
    }
}
