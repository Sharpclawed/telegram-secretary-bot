using System;
using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;
using TrunkRings.DomainExtensions;
using TrunkRings.Settings;

namespace TrunkRings.Commands
{
    class ViewWaitersCommand : IBotCommand
    {
        private readonly IMessageService messageService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewWaitersCommand(IMessageService messageService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public ViewWaitersCommand(IMessageService messageService, ITgBotClientEx tgClient, ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }
        
        public async Task ProcessAsync()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.AddMonths(-1);
            var untilDateValue = UntilDate ?? DateTime.UtcNow;
            var records = messageService.GetUnansweredDirMsgs(sinceDateValue, untilDateValue).FilterObviouslySuperfluous().ToList();

            if (records.Count <= TgBotSettings.ReadableCountOfMessages)
            {
                var formattedRecords = records.Select(Formatter.Waiters).ToList();
                await tgClient.SendTextMessagesAsListAsync(chatId, formattedRecords, СorrespondenceType.Personal);
            }
            else
            {
                var recordsWithColumnsToReport = records.Select(msg => new
                {
                    Date = msg.Date.ToString("dd.MM.yy HH:mm:ss"),
                    msg.ChatName,
                    msg.Message,
                    msg.UserFirstName,
                    msg.UserLastName,
                    msg.UserName,
                    msg.UserId
                }).ToList();
                await tgClient.SendTextMessagesAsExcelReportAsync(chatId, recordsWithColumnsToReport, "Отчет по неотвеченным сообщениям");}
        }
    }
}