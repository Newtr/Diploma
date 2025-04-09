package by.tyshkevich.evacuation_system.General.Entity;

import by.tyshkevich.evacuation_system.General.Services.EmergencySimulator;
import by.tyshkevich.evacuation_system.TelegramBot.Handlers.EvacuationBotHelper;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.bots.TelegramLongPollingBot;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;
import org.telegram.telegrambots.meta.api.objects.Update;
import org.telegram.telegrambots.meta.api.methods.ParseMode;

import java.util.List;

@Component
public class EvacuationBot extends TelegramLongPollingBot
{

    @Autowired
    private List<BotCommandHandler> commandHandlers;

    @Autowired
    private EmergencySimulator simulator;

    @Autowired
    private EvacuationBotHelper botHelper;

    @Override
    public String getBotUsername()
    {
        return "EvacuationAlertBot";
    }

    @Override
    public String getBotToken()
    {
        return "7739045967:AAH9D4tGz7v8H7vZC67b3yI6D3diwDy5i0U";
    }

    @Override
    public void onUpdateReceived(Update update)
    {
        boolean handled = false;
        for (BotCommandHandler handler : commandHandlers)
        {
            if (handler.canHandle(update))
            {
                handler.handle(update);
                handled = true;
                break;
            }
        }
        if (!handled)
        {
            long chatId = update.getMessage().getChatId();
            botHelper.sendMessage(chatId, "Команда не распознана. Для входа в систему введите /login <номер студенческого>.");
        }
    }

    private void sendMessage(long chatId, String text)
    {
        EvacuationBotHelper helper = new EvacuationBotHelper();
        helper.sendMessage(chatId, text);
    }
}
