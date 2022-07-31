using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class AddOnetimeChatCommand : IBotCommand
    {
        private readonly IOneTimeChatService oneTimeChatService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string ChatName { get; }

        public AddOnetimeChatCommand(IOneTimeChatService oneTimeChatService, ITgBotClientEx tgClient, ChatId chatId, string chatName)
        {
            this.oneTimeChatService = oneTimeChatService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            ChatName = chatName;
        }

        public async Task ProcessAsync()
        {
            if (oneTimeChatService.TryMake(ChatName, out string resultMessage))
                await tgClient.SendTextMessageAsync(chatId, "Команда обработана");
            else
                await tgClient.SendTextMessageAsync(chatId, resultMessage);
        }
    }
}