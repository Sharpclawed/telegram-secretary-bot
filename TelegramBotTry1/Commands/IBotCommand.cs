using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public interface IBotCommand
    {
        CommandResult Process();
    }
}