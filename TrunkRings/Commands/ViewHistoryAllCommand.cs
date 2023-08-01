using System;
using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using TrunkRings.DomainExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class ViewHistoryAllCommand : IBotCommand
    {
        private readonly IMessageService messageService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public DateTime Begin { get; }
        public DateTime End { get; }

        public ViewHistoryAllCommand(IMessageService messageService, ITgBotClientEx tgClient, ChatId chatId, DateTime begin, DateTime end)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            Begin = begin;
            End = end;
        }

        public async Task ProcessAsync()
        {
            var records = messageService.GetHistory(Begin, End, null, null).ToList();

            if (!records.Any())
                await tgClient.SendTextMessageAsync(chatId, "За указанный период сообщений не найдено");
            else
            {
                var recordsWithColumnsToReport = records
                    .ToLookup(msg => msg.ChatName, msg => new
                    {
                        Date = Formatter.DateEkbTime(msg),
                        msg.Message,
                        msg.UserFirstName,
                        msg.UserLastName,
                        msg.UserName,
                        msg.UserId
                    });
                await tgClient.SendTextMessagesAsExcelReportAsync(chatId, recordsWithColumnsToReport, "История сообщений");
            }
        }
    }
}