using System;
using System.Collections.Generic;
using System.Linq;
using TrunkRings.DAL;
using TrunkRings.DAL.Models;
using Microsoft.EntityFrameworkCore;
using TrunkRings.Domain.Models;

namespace TrunkRings.Domain.Services
{
    class MessageService : IMessageService
    {
        public void Save(DomainMessage message)
        {
            using var context = new SecretaryContext();
            var messageDataSet = new MessageDataSet
            {
                MessageId = message.MessageId,
                Date = message.Date,
                UserName = message.UserName,
                UserFirstName = message.UserFirstName,
                UserLastName = message.UserLastName,
                UserId = message.UserId,
                ChatId = message.ChatId,
                ChatName = message.ChatName,
                Message = message.Message
            };
            context.MessageDataSets.Add(messageDataSet); //todo sql injection protection
            context.SaveChanges();
        }

        public IEnumerable<DomainMessage> GetHistory(DateTime begin, DateTime end, string exactChatName, long? exactUserId)
        {
            using var context = new SecretaryContext();
            return context.MessageDataSets.AsNoTracking()
                .GetActualDates(begin, end)
                .GetActualChats(exactChatName)
                .GetActualUser(exactUserId)
                .ToList()
                .Select(z => new DomainMessage
                {
                    UserId = z.UserId,
                    UserName = z.UserName,
                    UserFirstName = z.UserFirstName,
                    UserLastName = z.UserLastName,
                    ChatId = z.ChatId,
                    ChatName = z.ChatName,
                    Date = z.Date,
                    Message = z.Message,
                    MessageId = z.MessageId
                })
                .OrderBy(z => z.Date);
        }

        public IOrderedEnumerable<DomainMessage> GetLastDirMsgFromInactiveChats(DateTime sinceDate, DateTime untilDate, TimeSpan checkingPeriod)
        {
            using var context = new SecretaryContext();
            var adminDataSets = context.AdminDataSets.AsNoTracking();
            var bkDataSets = context.BookkeeperDataSets.AsNoTracking();
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var onetimeChatDataSets = context.OnetimeChatDataSets.AsNoTracking();
            var timeBorder = untilDate.Add(checkingPeriod.Negate());

            var lastMessagesFromDirectors = (
                    from msgExt in (
                        from msg in messageDataSets.Where(z =>
                            z.Date > sinceDate && z.Date <= untilDate && z.ChatName != null)
                        from onetimeChat in onetimeChatDataSets
                            .Where(chat => chat.ChatId == msg.ChatId).DefaultIfEmpty()
                        from bookkeeper in bkDataSets
                            .Where(bk => bk.UserId == msg.UserId).DefaultIfEmpty()
                        from admin in adminDataSets.Where(x => x.DeleteTime == null)
                            .Where(adm => adm.UserId == msg.UserId).DefaultIfEmpty()
                        where bookkeeper.BookkeeperDataSetId == null
                              && admin.AdminDataSetId == null //только сообщения директоров
                              && onetimeChat == null //не чаты для разовых консультаций
                        select msg
                    ).ToList()
                    group msgExt by msgExt.ChatId
                    into groups
                    select groups.OrderByDescending(p => p.Date).FirstOrDefault()
                )
                .Where(msg => msg.Date <= timeBorder)
                .Select(z => new DomainMessage
                {
                    UserId = z.UserId,
                    UserName = z.UserName,
                    UserFirstName = z.UserFirstName,
                    UserLastName = z.UserLastName,
                    ChatId = z.ChatId,
                    ChatName = z.ChatName,
                    Date = z.Date,
                    Message = z.Message,
                    MessageId = z.MessageId
                })
                .OrderByDescending(msg => msg.Date);

            return lastMessagesFromDirectors;
        }

        public IEnumerable<DomainMessage> GetUnansweredDirMsgs(DateTime sinceDate, DateTime untilDate)
        {
            using var context = new SecretaryContext();
            var adminDataSets = context.AdminDataSets.AsNoTracking();
            var bkDataSets = context.BookkeeperDataSets.AsNoTracking();
            var messageDataSets = context.MessageDataSets.AsNoTracking();

            var lastMessagesFromDirectors = (
                    from msgExt in (
                        from msg in messageDataSets.Where(z => z.Date > sinceDate)
                        from bookkeeper in bkDataSets
                            .Where(bk => bk.UserId == msg.UserId).DefaultIfEmpty()
                        from admin in adminDataSets.Where(x => x.DeleteTime == null)
                            .Where(adm => adm.UserId == msg.UserId).DefaultIfEmpty()
                        let isByDir = bookkeeper.BookkeeperDataSetId == null && admin.AdminDataSetId == null
                        where msg.ChatName != null
                              && msg.Date > sinceDate
                              && (!isByDir || !msg.Message.StartsWith("MessageType: "))
                        select new { msg, isByDir }
                    ).ToArray()
                    group msgExt by msgExt.msg.ChatId
                    into groups
                    select groups.OrderByDescending(p => p.msg.Date).FirstOrDefault()
                )
                .Where(msgExt => msgExt.msg.Date <= untilDate && msgExt.isByDir)
                .Select(msgExt => msgExt.msg)
                .Select(z => new DomainMessage
                {
                    UserId = z.UserId,
                    UserName = z.UserName,
                    UserFirstName = z.UserFirstName,
                    UserLastName = z.UserLastName,
                    ChatId = z.ChatId,
                    ChatName = z.ChatName,
                    Date = z.Date,
                    Message = z.Message,
                    MessageId = z.MessageId
                })
                .OrderBy(msg => msg.Date);

            return lastMessagesFromDirectors;
        }

        public DomainMessage GetLastChatMessage()
        {
            using var context = new SecretaryContext();
            var lastMsg = context.MessageDataSets.AsNoTracking()
                .OrderByDescending(message => message.Date)
                .First(z => z.ChatName != null);
            return new DomainMessage
                {
                    UserId = lastMsg.UserId,
                    UserName = lastMsg.UserName,
                    UserFirstName = lastMsg.UserFirstName,
                    UserLastName = lastMsg.UserLastName,
                    ChatId = lastMsg.ChatId,
                    ChatName = lastMsg.ChatName,
                    Date = lastMsg.Date,
                    Message = lastMsg.Message,
                    MessageId = lastMsg.MessageId
                };
        }

        public Dictionary<long, string> GetChatNames(long[] chatIds)
        {
            using var context = new SecretaryContext();
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var chatsWithNames = (
                    from msg in messageDataSets
                    select new {msg.ChatId, msg.ChatName, msg.Date.Year}
                ).Distinct()
                .ToArray();
            var chatsWithNamesDistinct = (
                from cht in chatsWithNames
                group cht by cht.ChatId
                into groups
                select groups.OrderByDescending(z => z.Year).FirstOrDefault()
            ).ToArray();
            return chatIds
                .Distinct()
                .Join(chatsWithNamesDistinct, z => z, z => z.ChatId,
                    (a, b) => new KeyValuePair<long, string>(a, b.ChatName))
                .ToDictionary(z => z.Key, z => z.Value);
        }
    }

    public interface IMessageService
    {
        void Save(DomainMessage message);
        IEnumerable<DomainMessage> GetHistory(DateTime begin, DateTime end, string exactChatName, long? exactUserId);
        IOrderedEnumerable<DomainMessage> GetLastDirMsgFromInactiveChats(DateTime sinceDate, DateTime untilDate, TimeSpan checkingPeriod);
        IEnumerable<DomainMessage> GetUnansweredDirMsgs(DateTime sinceDate, DateTime untilDate);
        DomainMessage GetLastChatMessage();
        Dictionary<long, string> GetChatNames(long[] cnatIds);
    }
}