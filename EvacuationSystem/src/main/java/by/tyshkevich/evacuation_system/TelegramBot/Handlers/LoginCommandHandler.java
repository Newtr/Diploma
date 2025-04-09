package by.tyshkevich.evacuation_system.TelegramBot.Handlers;

import by.tyshkevich.evacuation_system.General.Services.UserService;
import by.tyshkevich.evacuation_system.General.Entity.BotCommandHandler;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.objects.Update;

@Component
public class LoginCommandHandler implements BotCommandHandler
{

    @Autowired
    private UserService userService;

    @Autowired
    private EvacuationBotHelper botHelper;

    @Override
    public boolean canHandle(Update update)
    {
        return update.hasMessage() &&
                update.getMessage().hasText() &&
                update.getMessage().getText().startsWith("/login");
    }

    @Override
    public void handle(Update update)
    {
        long chatId = update.getMessage().getChatId();
        String msg = update.getMessage().getText();
        String[] parts = msg.split(" ");
        if (parts.length < 2)
        {
            botHelper.sendMessage(chatId, "Введите номер студенческого. Например: /login 12345678");
            return;
        }
        String studentNumber = parts[1];
        String response = userService.loginStudent(studentNumber, String.valueOf(chatId));
        botHelper.sendMessage(chatId, response);
    }
}
