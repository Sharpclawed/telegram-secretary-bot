using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewBkCommand : IBotCommand
    {
        public CommandResult Process()
        {
            return BookkeepersListProvider.GetRows();
        }
    }
}