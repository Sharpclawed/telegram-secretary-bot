using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewOneTimeChatsCommand : IBotCommand
    {
        public CommandResult Process()
        {
            return IgnoreChatsListProvider.GetRows();
        }
    }
}