using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    internal class GetChatIdCommand : IBotCommand
    {
        private IMessageService messageService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatToSendId;
        public string ChatName { get; }

        public GetChatIdCommand(IMessageService messageService, ITgBotClientEx tgClient, ChatId chatToSendId, string chatName)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatToSendId = chatToSendId;
            ChatName = chatName;
        }

        public async Task ProcessAsync()
        {
            var chatId = messageService.FindChatId(ChatName);
            if (chatId.HasValue)
                await tgClient.SendTextMessageAsync(chatToSendId, "Id чата: " + chatId);
            else
                await tgClient.SendTextMessageAsync(chatToSendId, "Чат не найден");
        }
    }
}
