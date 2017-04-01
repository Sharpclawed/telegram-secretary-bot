using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.Enums; using Telegram.Bot;

namespace TelegramBotTry1
{
    public static class DataSetExtensions     {         public static IQueryable<MessageDataSet> GetActualDates(this IQueryable<MessageDataSet> dataSets,
                                                                HistoryCommandConfig commandConfig)
        {
            return dataSets.Where(x => x.Date >= commandConfig.Begin && x.Date < commandConfig.End);
        }

        public static IQueryable<MessageDataSet> GetActualChats(this IQueryable<MessageDataSet> dataSets,
            HistoryCommandConfig commandConfig)
        {
            switch (commandConfig.Type)
            {
                case HistoryCommandType.SingleChat:
                    //TODO по названию могут определяться разные чаты в разные моменты времени. Нам нужен актуальный или все?
                    return dataSets
                        .Where(x => x.ChatName.Equals(commandConfig.Argument, StringComparison.InvariantCultureIgnoreCase));
                case HistoryCommandType.AllChats:
                case HistoryCommandType.SingleUser:
                    return dataSets.Where(x => x.ChatName != null);
                default:
                    return null;
            }
        }

        public static IQueryable<MessageDataSet> GetActualUser(this IQueryable<MessageDataSet> dataSets,
                                                                HistoryCommandConfig commandConfig)
        {
            switch (commandConfig.Type)
            {
                case HistoryCommandType.SingleUser:
                    var groupedSet = dataSets.Where(x => x.UserId.ToString() == commandConfig.Argument)
                        .GroupBy(x => x.ChatId)
                        .Select(grp => grp.FirstOrDefault());
                    return from dataset in dataSets
                        join set in groupedSet on dataset.ChatId equals set.ChatId
                        select dataset;
                default:
                    return dataSets;
            }
        }

        public static Dictionary<long, List<MessageDataSet>> CheckAskerRights(
            this Dictionary<long, List<MessageDataSet>> dataSetsDct, TelegramBotClient bot, int askerId)
        {
            if (IsUserDatabaseAbsoluteAdmin(askerId)) return dataSetsDct;

            var result = new Dictionary<long, List<MessageDataSet>>();
            foreach (var dataSet in dataSetsDct)
            {
                if (dataSet.Value.Count == 0) continue;
                var chatMember = bot.GetChatMemberAsync(dataSet.Value[0].ChatId, askerId).Result;
                var allowSendFeedback = chatMember.Status == ChatMemberStatus.Member ||
                                        chatMember.Status == ChatMemberStatus.Administrator ||
                                        chatMember.Status == ChatMemberStatus.Creator;
                if (allowSendFeedback) result.Add(dataSet.Key, dataSet.Value);
            }
            return result;
        }

        private static bool IsUserDatabaseAbsoluteAdmin(int askerId)
        {
            return askerId == 210604626 || askerId == 160511161; //Вика/я
        }     }
}