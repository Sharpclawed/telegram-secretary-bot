using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1
{
    //TODO take away commands from Program and Reporters
    //TODO take away configuring tgClient
    public class BotCommander
    {
        private readonly ITgBotClientEx tgClient;

        public BotCommander(ITgBotClientEx tgClient)
        {
            this.tgClient = tgClient;
        }

        public async Task SendMessageAsync(ChatId chatId, string text)
        {
            await new SendMessageCommand(tgClient, chatId, text).ProcessAsync();
        }

        public async Task SendMessagesAsync(IEnumerable<ChatId> chatIds, string text)
        {
            await new DistributeMessageCommand(tgClient, chatIds, text).ProcessAsync();
        }
    }
}