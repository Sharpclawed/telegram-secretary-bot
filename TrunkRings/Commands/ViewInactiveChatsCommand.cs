using System;
using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using TrunkRings.DomainExtensions;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class ViewInactiveChatsCommand : IBotCommand
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
            var recordsWithColumnsToReport = records.Select(msg => new
            {
                Date = Formatter.DateEkbTime(msg),
                msg.ChatName,
                msg.Message,
                msg.UserFirstName,
                msg.UserLastName,
                msg.UserName
            }).ToList();

            await tgClient.SendTextMessagesAsExcelReportAsync(chatId, recordsWithColumnsToReport, caption);
        }
    }
}