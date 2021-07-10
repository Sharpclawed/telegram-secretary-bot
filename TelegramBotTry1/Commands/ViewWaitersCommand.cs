using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Telegram.Bot.Types;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.Commands
{
    public class ViewWaitersCommand : IBotCommand
    {
        private readonly MessageService messageService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewWaitersCommand(MessageService messageService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public ViewWaitersCommand(MessageService messageService, ITgBotClientEx tgClient, ChatId chatId,
            DateTime sinceDate, DateTime untilDate)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }
        
        public async Task ProcessAsync()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.Date.AddMonths(-1);
            var untilDateValue = UntilDate ?? DateTime.UtcNow.Date.AddMinutes(-30);
            var records = messageService.GetUnansweredDirMsgs(sinceDateValue, untilDateValue).FilterObviouslySuperfluous().ToList();
            var formattedRecords = records.Select(Formatter.Waiters).ToList();

            if (formattedRecords.Count <= TgBotSettings.ReadableCountOfMessages)
                await tgClient.SendTextMessagesAsListAsync(chatId, formattedRecords, СorrespondenceType.Personal);
            else
                await tgClient.SendTextMessagesAsExcelReportAsync(
                    chatId,
                    records,
                    "Отчет по неотвеченным сообщениям",
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