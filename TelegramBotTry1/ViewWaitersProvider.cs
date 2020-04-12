using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    public static class ViewWaitersProvider
    {
        public static List<MessageDataSet> GetWaiters(DateTime sinceDate, DateTime untilDate)
        {
            using (var context = new MsgContext())
            {
                var adminDataSets = context.Set<AdminDataSet>().AsNoTracking().ToArray();
                var bkDataSets = context.Set<BookkeeperDataSet>().AsNoTracking().ToArray();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();

                var allLastMessagesInChats = (
                        from msg in messageDataSets
                        group msg by msg.ChatId
                        into groups
                        select groups.OrderByDescending(p => p.Date).FirstOrDefault()
                    ).ToList()
                    ;

                var lastMessagesFromDirectors =
                        from msg in allLastMessagesInChats
                        from bookkeeper in bkDataSets
                            .Where(bk => bk.UserId == msg.UserId).DefaultIfEmpty()
                        from admin in adminDataSets.Where(x => x.DeleteTime == null)
                            .Where(adm => adm.UserId == msg.UserId).DefaultIfEmpty()
                        where bookkeeper == null && admin == null
                                                 && msg.ChatName != null
                                                 && msg.Date > sinceDate
                                                 && msg.Date <= untilDate
                        select msg
                    ;

                return lastMessagesFromDirectors.ToList();
            }
        }
    }
}