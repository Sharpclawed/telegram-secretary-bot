using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class RemoveOnetimeChatCommand : IBotCommand
    {
        private readonly OneTimeChatService oneTimeChatService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string ChatName { get; }

        public RemoveOnetimeChatCommand(OneTimeChatService oneTimeChatService, ITgBotClientEx tgClient, ChatId chatId,
            string chatName)
        {
            this.oneTimeChatService = oneTimeChatService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            ChatName = chatName;
        }

        public async Task ProcessAsync()
        {
            var succeeded = oneTimeChatService.Unmake(ChatName);
            var result = succeeded ? "Команда обработана" : "Чат не найден";

            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}