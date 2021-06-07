using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1.DataProviders
{
    public static class HistoryProvider
    {
        public static ViewReportResult GetRows(HistoryCommand command)
        {
            using (var context = new MsgContext())
            {
                if (command.Type == HistoryCommandType.Unknown)
                    return new ViewReportResult {Error = "Неизвестная команда"};//todo help for history case
                
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking().GetActualDates(command);
                if (!messageDataSets.Any())
                    return new ViewReportResult { Error = "В данном периоде нет сообщений" };

                messageDataSets = messageDataSets.GetActualChats(command);
                if (messageDataSets == null || !messageDataSets.Any())
                    return new ViewReportResult { Error = "В выбранных чатах нет сообщений" };

                messageDataSets = messageDataSets.GetActualUser(command);
                if (!messageDataSets.Any())
                    return new ViewReportResult { Error = "По данному пользователю нет сообщений" };

                return new ViewReportResult {Messages = messageDataSets.ToList(), Caption = "История сообщений" };
            }
        }
    }
}
