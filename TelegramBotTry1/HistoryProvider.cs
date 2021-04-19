using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1
{
    public static class HistoryProvider
    {
        public static HistoryResult GetRows(HistoryCommand command, bool isAdminAsking)
        {
            using (var context = new MsgContext())
            {
                if (command.Type == HistoryCommandType.Unknown)
                    return new HistoryResult {Error = "Неизвестная команда"};//todo help for history case

                if (!isAdminAsking)
                    return new HistoryResult { Error = "У вас не хватает прав" };

                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking().GetActualDates(command);
                if (!messageDataSets.Any())
                    return new HistoryResult { Error = "В данном периоде нет сообщений" };

                messageDataSets = messageDataSets.GetActualChats(command);
                if (messageDataSets == null || !messageDataSets.Any())
                    return new HistoryResult { Error = "В выбранных чатах нет сообщений" };

                messageDataSets = messageDataSets.GetActualUser(command);
                if (!messageDataSets.Any())
                    return new HistoryResult { Error = "По данному пользователю нет сообщений" };

                return new HistoryResult {Records = messageDataSets.ToList()};
            }
        }
    }
}
