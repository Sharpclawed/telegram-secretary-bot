using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrunkRings.DAL;
using TrunkRings.DAL.Models;
using Microsoft.EntityFrameworkCore;
using TrunkRings.Domain.Models;

namespace TrunkRings.Domain.Services
{
    class MessageService : IMessageService
    {
        public async Task SaveAsync(DomainMessage message)
        {
            await using var context = new SecretaryContext();
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
            await context.SaveChangesAsync();
        }

        public IEnumerable<DomainMessage> GetHistory(DateTime begin, DateTime end, string exactChatName, long? exactUserId)
        {
            using var context = new SecretaryContext();
            begin = DateTime.SpecifyKind(begin, DateTimeKind.Utc);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);

            return context.MessageDataSets.AsNoTracking()
                .GetActualDates(begin, end)
                .GetActualChats(exactChatName)
                .GetActualUser(exactUserId)
                .ToList()
                .Select(msg => new DomainMessage
                {
                    UserId = msg.UserId,
                    UserName = msg.UserName,
                    UserFirstName = msg.UserFirstName,
                    UserLastName = msg.UserLastName,
                    ChatId = msg.ChatId,
                    ChatName = msg.ChatName,
                    Date = msg.Date,
                    Message = msg.Message,
                    MessageId = msg.MessageId
                })
                .OrderBy(msg => msg.Date);
        }

        public IOrderedEnumerable<DomainMessage> GetLastDirMsgFromInactiveChats(DateTime sinceDate, DateTime untilDate, TimeSpan checkingPeriod)
        {
            using var context = new SecretaryContext();
            var adminDataSets = context.AdminDataSets.AsNoTracking();
            var bkDataSets = context.BookkeeperDataSets.AsNoTracking();
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var onetimeChatDataSets = context.OnetimeChatDataSets.AsNoTracking();
            var timeBorder = untilDate.Add(checkingPeriod.Negate());
            sinceDate = DateTime.SpecifyKind(sinceDate, DateTimeKind.Utc);
            untilDate = DateTime.SpecifyKind(untilDate, DateTimeKind.Utc);

#pragma warning disable CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
            var lastMessagesFromDirectors = (
                    from msgExt in (
                        from msg in messageDataSets.Where(m =>
                            m.Date > sinceDate && m.Date <= untilDate && m.ChatName != null)
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
#pragma warning restore CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    group msgExt by msgExt.ChatId
                    into groups
                    select groups.OrderByDescending(p => p.Date).FirstOrDefault()
                )
                .Where(msg => msg.Date <= timeBorder)
                .Select(msg => new DomainMessage
                {
                    UserId = msg.UserId,
                    UserName = msg.UserName,
                    UserFirstName = msg.UserFirstName,
                    UserLastName = msg.UserLastName,
                    ChatId = msg.ChatId,
                    ChatName = msg.ChatName,
                    Date = msg.Date,
                    Message = msg.Message,
                    MessageId = msg.MessageId
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
            sinceDate = DateTime.SpecifyKind(sinceDate, DateTimeKind.Utc);
            untilDate = DateTime.SpecifyKind(untilDate, DateTimeKind.Utc);

#pragma warning disable CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
            var lastMessagesFromDirectors = (
                    from msgExt in (
                        from msg in messageDataSets.Where(m => m.Date > sinceDate)
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
#pragma warning restore CS8073 // The result of the expression is always the same since a value of this type is never equal to 'null'
                    group msgExt by msgExt.msg.ChatId
                    into groups
                    select groups.OrderByDescending(p => p.msg.Date).FirstOrDefault()
                )
                .Where(msgExt => msgExt.msg.Date <= untilDate && msgExt.isByDir)
                .Select(msgExt => msgExt.msg)
                .Select(m => new DomainMessage
                {
                    UserId = m.UserId,
                    UserName = m.UserName,
                    UserFirstName = m.UserFirstName,
                    UserLastName = m.UserLastName,
                    ChatId = m.ChatId,
                    ChatName = m.ChatName,
                    Date = m.Date,
                    Message = m.Message,
                    MessageId = m.MessageId
                })
                .OrderBy(msg => msg.Date);

            return lastMessagesFromDirectors;
        }

        public DomainMessage GetLastChatMessage()
        {
            using var context = new SecretaryContext();
            var lastMsg = context.MessageDataSets.AsNoTracking()
                .OrderByDescending(message => message.Date)
                .First(msg => msg.ChatName != null);
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

        public long? FindChatId(string chatName)
        {
            using var context = new SecretaryContext();
            var lastMsg = context.MessageDataSets.AsNoTracking()
                .Where(msg => msg.ChatName == chatName)
                .OrderByDescending(msg => msg.Date)
                .FirstOrDefault();
            return lastMsg?.ChatId;
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
                select groups.OrderByDescending(x => x.Year).FirstOrDefault()
            ).ToArray();
            return chatIds
                .Distinct()
                .Join(chatsWithNamesDistinct, x => x, x => x.ChatId,
                    (a, b) => new KeyValuePair<long, string>(a, b.ChatName))
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }

    public interface IMessageService
    {
        Task SaveAsync(DomainMessage message);
        IEnumerable<DomainMessage> GetHistory(DateTime begin, DateTime end, string exactChatName, long? exactUserId);
        IOrderedEnumerable<DomainMessage> GetLastDirMsgFromInactiveChats(DateTime sinceDate, DateTime untilDate, TimeSpan checkingPeriod);
        IEnumerable<DomainMessage> GetUnansweredDirMsgs(DateTime sinceDate, DateTime untilDate);
        DomainMessage GetLastChatMessage();
        long? FindChatId(string chatName);
        Dictionary<long, string> GetChatNames(long[] cnatIds);
    }
}