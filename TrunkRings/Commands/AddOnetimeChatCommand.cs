using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    public class AddOnetimeChatCommand : IBotCommand
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
            oneTimeChatService.Make(ChatName);

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}