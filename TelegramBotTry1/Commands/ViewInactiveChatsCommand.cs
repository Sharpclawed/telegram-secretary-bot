using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Commands
{
    public class ViewInactiveChatsCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewInactiveChatsCommand(ITgBotClientEx tgClient, ChatId chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public ViewInactiveChatsCommand(ITgBotClientEx tgClient, ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }

        public async Task ProcessAsync()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.AddDays(-365);
            var untilDateValue = UntilDate ?? DateTime.UtcNow;
            var report = ViewInactiveChatsProvider.GetInactive(sinceDateValue, untilDateValue, TimeSpan.FromDays(7));
            var caption = "Отчет по неактивным чатам";

            await tgClient.SendTextMessagesAsExcelReportAsync(
                chatId,
                report,
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