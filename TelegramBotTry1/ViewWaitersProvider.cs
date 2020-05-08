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
                                  && !msg.Message.StartsWith("MessageType: ")
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
            return GetUnanswered(sinceDate, untilDate)
                .Select(z => new
                {
                    msg = z,
                    txt = z.Message.ToLower()
                        .Replace(new[] {'!', '.', ',', ';', ':', ')', '(', '+'}, "")
                        .Replace("👍", "")
                        .Replace("👌🏻", "")
                        .Replace("🙏", "")
                        .Trim()
                })
                .Where(z => !IgnoreUnanswered.Contains(z.txt))
                .Select(z => z.msg)
                .ToList();
        }

        private static readonly List<string> IgnoreUnanswered = new List<string>
        {
            "",
            "ага",
            "ага спасибо",
            "ага хорошо",
            "благодарю",
            "благодарим вас",
            "большое спасибо",
            "взаимно",
            "вроде бы все хорошо",
            "да",
            "да спасибо",
            "да конечно спасибо",
            "добрый день да",
            "добрый день спасибо",
            "добрый день хорошо",
            "добрый день хорошо спасибо",
            "ну да",
            "ну хорошо",
            "ок",
            "ок благодарю",
            "ок, понял",
            "ок, поняла",
            "ок спс",
            "ок спасибо",
            "отлично",
            "понял",
            "понял большое спасибо",
            "понял спасибо",
            "поняла",
            "поняла спасибо",
            "понятно спасибо за информацию",
            "принято",
            "спасибо",
            "спасибо большое",
            "спс",
            "супер",
            "супер спасибо",
            "увидел благодарю",
            "увидела благодарю",
            "ура",
            "хорлшо",
            "хорошо",
            "хорошо благодарю",
            "хорошо попробую спасибо",
            "хорошо примем",
            "хорошо сделаю сегодня",
            "хорошо спасибо",
            "ясно",
            "ok",
        };
    }
}