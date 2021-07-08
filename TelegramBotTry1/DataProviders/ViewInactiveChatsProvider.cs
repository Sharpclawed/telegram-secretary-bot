using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using DAL.Models;

namespace TelegramBotTry1.DataProviders
{
    public static class ViewInactiveChatsProvider
    {
        public static List<MessageDataSet> GetInactive(DateTime sinceDate, DateTime untilDate, TimeSpan checkingPeriod)
        {
            using (var context = new SecretaryContext())
            {
                var adminDataSets = context.Set<AdminDataSet>();
                var bkDataSets = context.Set<BookkeeperDataSet>();
                var messageDataSets = context.Set<MessageDataSet>();
                var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                var timeBorder = untilDate.Add(checkingPeriod.Negate());

                var lastMessagesFromDirectors = (
                        from msgExt in (
                            from msg in messageDataSets.Where(z => z.Date > sinceDate && z.Date <= untilDate && z.ChatName != null)
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
                    .OrderByDescending(msg => msg.Date)
                    .ToList();

                return lastMessagesFromDirectors;
            }
        }
    }
}