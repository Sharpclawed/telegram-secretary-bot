using System;
using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.DataProviders
{
    public static class HistoryProvider
    {
        public static CommandResult GetRows(DateTime begin, DateTime end, string exactChatName, string exactUserId)
        {
            using (var context = new MsgContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking().GetActualDates(begin, end);
                if (!messageDataSets.Any())
                    return new CommandResult { Error = "В данном периоде нет сообщений" };

                messageDataSets = messageDataSets.GetActualChats(exactChatName);
                if (messageDataSets == null || !messageDataSets.Any())
                    return new CommandResult { Error = "В выбранных чатах нет сообщений" };

                messageDataSets = messageDataSets.GetActualUser(exactUserId);
                if (!messageDataSets.Any())
                    return new CommandResult { Error = "По данному пользователю нет сообщений" };

                return new CommandResult {Messages = messageDataSets.ToList(), Caption = "История сообщений" };
            }
        }
    }
}