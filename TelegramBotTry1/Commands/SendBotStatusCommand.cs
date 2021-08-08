using System;
using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class SendBotStatusCommand : IBotCommand
    {
        private readonly IMessageService messageService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public SendBotStatusCommand(IMessageService messageService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var lastMessage = messageService.GetLastChatMessage();
            var lastMessageDate = lastMessage?.Date.AddHours(5) ?? DateTime.MinValue;
            var lastMessageChat = lastMessage?.ChatName;
            var iAmAliveMessage = $"Работаю в штатном режиме\r\nПоследнее сообщение от {lastMessageDate:dd.MM.yyyy H:mm} в \"{lastMessageChat}\"";
            await tgClient.SendTextMessageAsync(chatId, iAmAliveMessage);
        }
    }
}