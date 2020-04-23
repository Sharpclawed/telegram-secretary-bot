using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    public static class ViewWaitersProvider
    {
        public static List<MessageDataSet> GetUnanswered(DateTime sinceDate, DateTime untilDate)
        {
            using (var context = new MsgContext())
            {
                var adminDataSets = context.Set<AdminDataSet>().ToArray();
                var bkDataSets = context.Set<BookkeeperDataSet>().ToArray();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();

                var allLastMessagesInChats = (
                            from msg in messageDataSets
                            where msg.ChatName != null
                                  && msg.Date > sinceDate
                            group msg by msg.ChatId
                            into groups
                            select groups.OrderByDescending(p => p.Date).FirstOrDefault()
                        )
                        .Where(msg => msg.Date <= untilDate)
                        .ToList()
                    ;

                var lastMessagesFromDirectors =
                        from msg in allLastMessagesInChats
                        from bookkeeper in bkDataSets
                            .Where(bk => bk.UserId == msg.UserId).DefaultIfEmpty()
                        from admin in adminDataSets.Where(x => x.DeleteTime == null)
                            .Where(adm => adm.UserId == msg.UserId).DefaultIfEmpty()
                        where bookkeeper == null && admin == null
                        select msg
                    ;

                return lastMessagesFromDirectors.ToList();
            }
        }

        public static List<MessageDataSet> GetWaiters(DateTime sinceDate, DateTime untilDate)
        {
            return GetUnanswered(sinceDate, untilDate).Where(z => !IgnoreUnanswered.Contains(z.Message.ToLower().Replace(new []{'!','.',','}, ""))).ToList();
        }

        private static readonly List<string> IgnoreUnanswered = new List<string>
        {
            "ок",
            "спасибо",
            "спасибо большое",
            "большое спасибо",
            "понял спасибо",
            "поняла спасибо",
            "добрый день хорошо",
            "добрый день хорошо спасибо",
            "хорошо",
            "хорошо спасибо",
            "ага спасибо",
            "да конечно спасибо",
            "да",
            "спс",
            "увидел благодарю",
            "увидела благодарю",
            "ок благодарю",
            "понял большое спасибо",
            "благодарим вас",
            "понятно спасибо за информацию",
        };
    }
}