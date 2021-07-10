using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class ViewHistoryAllCommand : IBotCommand
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
                await tgClient.SendTextMessagesAsExcelReportAsync(
                    chatId,
                    records,
                    "История сообщений",
                    new[]
                    {
                        nameof(DomainMessage.Date),
                        nameof(DomainMessage.Message),
                        nameof(DomainMessage.UserFirstName),
                        nameof(DomainMessage.UserLastName),
                        nameof(DomainMessage.UserName),
                        nameof(DomainMessage.UserId)
                    },
                    msg => msg.ChatName);
        }
    }
}