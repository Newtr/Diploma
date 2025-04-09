package by.tyshkevich.evacuation_system.TelegramBot.Handlers;

import by.tyshkevich.evacuation_system.General.Entity.BotCommandHandler;
import by.tyshkevich.evacuation_system.General.Entity.EmergencyType;
import by.tyshkevich.evacuation_system.General.Services.UserService;
import by.tyshkevich.evacuation_system.General.Services.EmergencySimulator;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.objects.Update;

@Component
public class SimulationCommandHandler implements BotCommandHandler
{
    @Autowired
    private UserService userService;

    @Autowired
    private EmergencySimulator simulator;

    @Autowired
    private EvacuationBotHelper botHelper;

    @Override
    public boolean canHandle(Update update)
    {
        return update.hasMessage() &&
                update.getMessage().hasText() &&
                update.getMessage().getText().startsWith("/simulate");
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

        String msg = update.getMessage().getText();
        try
        {
            String[] parts = msg.split(" ");
            if (parts.length < 2)
            {
                botHelper.sendMessage(chatId, "Укажите идентификатор ЧС. Например: /simulate 3");
                return;
            }
            int id = Integer.parseInt(parts[1]);
            EmergencyType type = EmergencyType.fromId(id);
            String responseText = simulator.modelSituation(type);
            botHelper.sendMessage(chatId, responseText);
        }
        catch (Exception e)
        {
            e.printStackTrace();
            botHelper.sendMessage(chatId, "Ошибка при обработке команды.");
        }
    }
}
