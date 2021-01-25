using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    public static class ViewWaitersProvider
    {
        public static List<MessageDataSet> GetUnanswered(DateTime sinceDate, DateTime untilDate)
        {
            using (var context = new MsgContext())
            {
                var adminDataSets = context.Set<AdminDataSet>();
                var bkDataSets = context.Set<BookkeeperDataSet>();
                var messageDataSets = context.Set<MessageDataSet>();

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
                                  && !(msg.Message == "MessageType: " + MessageType.SuccessfulPayment ||
                                       msg.Message == "MessageType: " + MessageType.WebsiteConnected ||
                                       msg.Message == "MessageType: " + MessageType.ChatMembersAdded ||
                                       msg.Message == "MessageType: " + MessageType.ChatMemberLeft ||
                                       msg.Message == "MessageType: " + MessageType.ChatTitleChanged ||
                                       msg.Message == "MessageType: " + MessageType.ChatPhotoChanged ||
                                       msg.Message == "MessageType: " + MessageType.MessagePinned ||
                                       msg.Message == "MessageType: " + MessageType.ChatPhotoDeleted ||
                                       msg.Message == "MessageType: " + MessageType.GroupCreated ||
                                       msg.Message == "MessageType: " + MessageType.SupergroupCreated ||
                                       msg.Message == "MessageType: " + MessageType.ChannelCreated ||
                                       msg.Message == "MessageType: " + MessageType.MigratedToSupergroup ||
                                       msg.Message == "MessageType: " + MessageType.MigratedFromGroup ||
                                       msg.Message == "MessageType: " + MessageType.Unknown)
                                  && (!isByDir || !msg.Message.StartsWith("MessageType: "))
                            select new {msg, isByDir}
                        ).ToArray()
                        group msgExt by msgExt.msg.ChatId
                        into groups
                        select groups.OrderByDescending(p => p.msg.Date).FirstOrDefault()
                    )
                    .Where(msgExt => msgExt.msg.Date <= untilDate && msgExt.isByDir)
                    .Select(msgExt => msgExt.msg)
                    .OrderBy(msg => msg.Date);

                return lastMessagesFromDirectors.ToList();
            }
        }

        public static List<MessageDataSet> GetWaiters(DateTime sinceDate, DateTime untilDate)
        {
            return GetUnanswered(sinceDate, untilDate)
                .FilterObviouslySuperfluous()
                .ToList();
        }
    }
}