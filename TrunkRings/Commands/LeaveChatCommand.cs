using System.Threading.Tasks;
using Telegram.Bot;

namespace TrunkRings.Commands
{
    class LeaveChatCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly long chatId;

        public LeaveChatCommand(ITgBotClientEx tgClient, long chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            await tgClient.LeaveChatAsync(chatId);
        }
    }
}
