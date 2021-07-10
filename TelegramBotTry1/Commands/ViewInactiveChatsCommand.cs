using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class ViewInactiveChatsCommand : IBotCommand
    {
        private readonly IMessageService messageService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewInactiveChatsCommand(IMessageService messageService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public ViewInactiveChatsCommand(IMessageService messageService, ITgBotClientEx tgClient, ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }

        public async Task ProcessAsync()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.AddDays(-365);
            var untilDateValue = UntilDate ?? DateTime.UtcNow;
            var records = messageService.GetLastDirMsgFromInactiveChats(sinceDateValue, untilDateValue, TimeSpan.FromDays(7));
            var caption = "Отчет по неактивным чатам";

            await tgClient.SendTextMessagesAsExcelReportAsync(
                chatId,
                records.ToList(),
                caption,
                new[]
                {
                    nameof(DomainMessage.Date),
                    nameof(DomainMessage.ChatName),
                    nameof(DomainMessage.Message),
                    nameof(DomainMessage.UserFirstName),
                    nameof(DomainMessage.UserLastName),
                    nameof(DomainMessage.UserName),
                    nameof(DomainMessage.UserId)
                });
        }
    }
}