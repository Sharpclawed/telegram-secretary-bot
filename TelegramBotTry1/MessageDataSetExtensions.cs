using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1
{
    public static class MessageDataSetExtensions
    {
        public static IQueryable<IMessageDataSet> GetActualDates(this IQueryable<IMessageDataSet> dataSets,
            HistoryCommand command)
        {
            return dataSets.Where(x => x.Date >= command.Begin && x.Date < command.End);
        }

        public static IQueryable<IMessageDataSet> GetActualChats(this IQueryable<IMessageDataSet> dataSets,
            HistoryCommand command)
        {
            switch (command.Type)
            {
                case HistoryCommandType.SingleChat:
                    //TODO по названию могут определяться разные чаты в разные моменты времени. Нам нужен актуальный или все?
                    return dataSets
                        .Where(x => x.ChatName.Equals(command.NameOrId, StringComparison.InvariantCultureIgnoreCase));
                case HistoryCommandType.AllChats:
                case HistoryCommandType.SingleUser:
                    return dataSets.Where(x => x.ChatName != null);
                default:
                    return null;
            }
        }

        public static IQueryable<IMessageDataSet> GetActualUser(this IQueryable<IMessageDataSet> dataSets,
            HistoryCommand command)
        {
            switch (command.Type)
            {
                case HistoryCommandType.SingleUser:
                    return from dataset in dataSets
                        join chatId in dataSets.Where(x => x.UserId.ToString() == command.NameOrId)
                            .Select(x => x.ChatId).Distinct() on dataset.ChatId equals chatId
                        select dataset;
                default:
                    return dataSets;
            }
        }

        public static Dictionary<long, List<IMessageDataSet>> CheckAskerRights(
            this Dictionary<long, List<IMessageDataSet>> dataSetsDct, TelegramBotClient bot, int askerId)
        {
            var result = new Dictionary<long, List<IMessageDataSet>>();
            foreach (var dataSet in dataSetsDct)
            {
                if (dataSet.Value.Count == 0)
                    continue;
                var chatMemberStatus = bot.GetChatMemberAsync(dataSet.Value[0].ChatId, askerId).Result.Status;
                var allowSendFeedback = chatMemberStatus == ChatMemberStatus.Member ||
                                        chatMemberStatus == ChatMemberStatus.Administrator ||
                                        chatMemberStatus == ChatMemberStatus.Creator;
                if (allowSendFeedback)
                    result.Add(dataSet.Key, dataSet.Value);
            }

            return result;
        }

        public static User GetUserByUserName(this IQueryable<IMessageDataSet> dataSets, string userName)
        {
            var message = dataSets.Where(x => x.UserName == userName).OrderByDescending(x => x.Date).FirstOrDefault();
            if (message == null)
                throw new ArgumentException("Пользователь не найден");
            return new User
            {
                UserId = message.UserId,
                Username = message.UserName,
                Name = message.UserFirstName,
                Surname = message.UserLastName
            };
        }
    }
}