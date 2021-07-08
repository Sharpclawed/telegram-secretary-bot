using System;
using System.Threading.Tasks;
using DAL.Models;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;

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
                    nameof(MessageDataSet.Date),
                    nameof(MessageDataSet.ChatName),
                    nameof(MessageDataSet.Message),
                    nameof(MessageDataSet.UserFirstName),
                    nameof(MessageDataSet.UserLastName),
                    nameof(MessageDataSet.UserName),
                    nameof(MessageDataSet.UserId)
                });
        }
    }
}