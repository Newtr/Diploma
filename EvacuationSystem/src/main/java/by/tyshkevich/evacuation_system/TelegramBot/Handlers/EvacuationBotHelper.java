package by.tyshkevich.evacuation_system.TelegramBot.Handlers;

import by.tyshkevich.evacuation_system.General.Entity.EvacuationBot;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Lazy;
import org.springframework.stereotype.Component;
import org.telegram.telegrambots.meta.api.methods.ParseMode;
import org.telegram.telegrambots.meta.api.methods.send.SendMessage;

@Component
public class EvacuationBotHelper
{

    @Autowired
    @Lazy
    private EvacuationBot bot;

    public void sendMessage(long chatId, String text)
    {
        SendMessage message = new SendMessage();
        message.setChatId(String.valueOf(chatId));
        message.setText(text);
        message.setParseMode(ParseMode.MARKDOWN);
        try
        {
            bot.execute(message);
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }
}
