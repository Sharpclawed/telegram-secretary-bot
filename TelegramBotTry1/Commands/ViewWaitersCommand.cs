using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Commands
{
    public class ViewWaitersCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewWaitersCommand(ITgBotClientEx tgClient, ChatId chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public ViewWaitersCommand(ITgBotClientEx tgClient, ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }

        //todo generalize or split code
        public async Task ProcessAsync()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.Date.AddMonths(-1);
            var untilDateValue = UntilDate ?? DateTime.UtcNow.Date.AddMinutes(-30);
            var waitersReport = ViewWaitersProvider.GetWaiters(sinceDateValue, untilDateValue);

            var formattedRecords = waitersReport.Select(Formatter.Waiters).ToList();
            await tgClient.SendTextMessagesAsListAsync(chatId, formattedRecords, СorrespondenceType.Personal);
        }

        public async Task Process2Async()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.Date.AddMonths(-1);
            var untilDateValue = UntilDate ?? DateTime.UtcNow.Date.AddMinutes(-30);
            var waitersReport = ViewWaitersProvider.GetWaiters(sinceDateValue, untilDateValue);

            var caption = "Отчет по неотвеченным сообщениям";

            await tgClient.SendTextMessagesAsExcelReportAsync(
                ChatIds.Unanswered,
                waitersReport,
                caption,
                new[]
                {
                    nameof(IMessageDataSet.Date),
                    nameof(IMessageDataSet.ChatName),
                    nameof(IMessageDataSet.Message),
                    nameof(IMessageDataSet.UserFirstName),
                    nameof(IMessageDataSet.UserLastName),
                    nameof(IMessageDataSet.UserName),
                    nameof(IMessageDataSet.UserId)
                });
        }
    }
}