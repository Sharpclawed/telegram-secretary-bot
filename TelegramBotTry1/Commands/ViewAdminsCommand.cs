using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewAdminsCommand : IBotCommand
    {
        public CommandResult Process()
        {
            return AdminsListProvider.GetRows();
        }
    }
}