using EvacProject.GENERAL.Data;
using Telegram.Bot.Types;

public interface ITelegramBotService
{
    Task SendMessageToAllStudents(string message);
    Task StartEvacuationTestAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken);
}