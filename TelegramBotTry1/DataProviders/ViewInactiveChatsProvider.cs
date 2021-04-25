using System;
using System.Collections.Generic;
using System.Linq;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.DataProviders
{
    public static class ViewInactiveChatsProvider
    {
        public static List<IMessageDataSet> GetInactive(DateTime sinceDate, DateTime untilDate, TimeSpan checkingPeriod)
        {
            using (var context = new MsgContext())
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
                    .ToList<IMessageDataSet>();

                return lastMessagesFromDirectors;
            }
        }

        //todo formatting is another responsibility
        public static List<string> GetInactiveFormatted(DateTime sinceDate, DateTime untilDate, TimeSpan checkingPeriod)
        {
            return GetInactive(sinceDate, untilDate, checkingPeriod)
                .Select(msg =>
                {
                    var mark = string.Empty;
                    switch (msg.Date)
                    {
                        case DateTime date when (untilDate - date).TotalDays < 7:
                            break;
                        case DateTime date when (untilDate - date).TotalDays < 14:
                            mark = " больше недели 😐";
                            break;
                        case DateTime date when (untilDate - date).TotalDays < 21:
                            mark = " больше двух недель 😕";
                            break;
                        default:
                            mark = " больше трех недель 😟";
                            break;
                    }

                    return string.Format(
                        @"В чате {0} нет активной переписки{1}. Последнее сообщение от клиента было {2}. Текст сообщения: ""{3}"""
                        , msg.ChatName
                        , mark
                        , msg.Date.AddHours(5).ToString("dd.MM.yyyy в H:mm")
                        , msg.Message);
                }).ToList();
        }
    }
}