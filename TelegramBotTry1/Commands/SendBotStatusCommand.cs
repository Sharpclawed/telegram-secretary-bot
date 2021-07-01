using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;

namespace TelegramBotTry1.Commands
{
    public class SendBotStatusCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public SendBotStatusCommand(ITgBotClientEx tgClient, ChatId chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var iAmAliveMessage = BotStatusProvider.GetIAmAliveMessage();
            await tgClient.SendTextMessageAsync(chatId, iAmAliveMessage);
        }
    }
}