using System;
using System.Linq;
using TrunkRings.DAL.Models;
using TrunkRings.Domain.Models;

namespace TrunkRings.Domain
{
    public static class MessageDataSetExtensions
    {
        public static IQueryable<MessageDataSet> GetActualDates(this IQueryable<MessageDataSet> dataSets, DateTime begin, DateTime end)
        {
            return dataSets.Where(x => x.Date >= begin && x.Date < end);
        }

        public static IQueryable<MessageDataSet> GetActualChats(this IQueryable<MessageDataSet> dataSets, string exactChatName = null)
        {
            if (exactChatName == null)
                return dataSets.Where(x => x.ChatName != null);

            return dataSets
                .Where(x => x.ChatName.Equals(exactChatName));
        }

        public static IQueryable<MessageDataSet> GetActualUser(this IQueryable<MessageDataSet> dataSets, long? exactUserId = null)
        {
            if (exactUserId == null)
                return dataSets;

            var messageDataSets = from dataset in dataSets
                                  join chatId in dataSets.Where(x => x.UserId == exactUserId)
                                      .Select(x => x.ChatId).Distinct() on dataset.ChatId equals chatId
                                  select dataset;
            return messageDataSets;
        }

        public static User FindUserByUserName(this IQueryable<MessageDataSet> dataSets, string userName)
        {
            var message = dataSets.Where(x => x.UserName.ToLower() == userName.ToLower()).OrderByDescending(x => x.Date).FirstOrDefault();
            if (message == null)
                return null;

            return new User
            {
                UserId = message.UserId,
                Username = message.UserName,
                Name = message.UserFirstName,
                Surname = message.UserLastName
            };
        }

        public static Chat FindChatByChatName(this IQueryable<MessageDataSet> dataSets, string chatName)
        {
            //todo 3 seconds - too much
            var message = dataSets.Where(x => x.ChatName.ToLower() == chatName.ToLower()).OrderByDescending(x => x.Date).FirstOrDefault();
            if (message == null)
                return null;

            return new Chat
            {
                Id = message.ChatId,
                Name = message.ChatName,
            };
        }
    }
}