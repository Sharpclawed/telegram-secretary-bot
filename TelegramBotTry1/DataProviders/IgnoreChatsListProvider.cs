using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.DataProviders
{
    public static class IgnoreChatsListProvider
    {
        public static CommandResult GetRows()
        {
            var result = new CommandResult();
            using (var context = new MsgContext())
            {
                var dataSets = context.Set<OnetimeChatDataSet>().AsNoTracking();
                result.Records = dataSets
                    .Select(x => x.ChatName)
                    .ToList();
            }

            if (result.Records.Any())
                result.Caption = "Список исключений для просмотра неактивных чатов:\r\n";
            else
                result.Error = "Список исключений для просмотра неактивных чатов пуст\r\n";

            return result;
        }
    }
}