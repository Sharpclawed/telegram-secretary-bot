using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DAL.Models;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.DataProviders
{
    public static class ViewWaitersProvider
    {
        public static List<MessageDataSet> GetUnanswered(DateTime sinceDate, DateTime untilDate)
        {
            using (var context = new SecretaryContext())
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
                                  && (!isByDir || !msg.Message.StartsWith("MessageType: "))
                            select new { msg, isByDir }
                        ).ToArray()
                        group msgExt by msgExt.msg.ChatId
                        into groups
                        select groups.OrderByDescending(p => p.msg.Date).FirstOrDefault()
                    )
                    .Where(msgExt => msgExt.msg.Date <= untilDate && msgExt.isByDir)
                    .Select(msgExt => msgExt.msg)
                    .OrderBy(msg => msg.Date);

                return new List<MessageDataSet>(lastMessagesFromDirectors);
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