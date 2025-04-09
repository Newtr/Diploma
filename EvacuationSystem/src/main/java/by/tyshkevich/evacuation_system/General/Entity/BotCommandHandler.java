package by.tyshkevich.evacuation_system.General.Entity;

import org.telegram.telegrambots.meta.api.objects.Update;

public interface BotCommandHandler
{
    boolean canHandle(Update update);

    void handle(Update update);
}
