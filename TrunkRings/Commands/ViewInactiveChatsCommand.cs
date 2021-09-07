using System;
using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
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
            var recordsWithColumnsToReport = records.Select(z => new
            {
                Date = z.Date.ToString("dd.MM.yy HH:mm:ss"),
                z.ChatName,
                z.Message,
                z.UserFirstName,
                z.UserLastName,
                z.UserName
            }).ToList();

            await tgClient.SendTextMessagesAsExcelReportAsync(chatId, recordsWithColumnsToReport, caption);
        }
    }
}