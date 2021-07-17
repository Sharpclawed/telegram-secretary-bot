using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotTry1.Commands
{
    public class DistributeMessageCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly IEnumerable<ChatId> chatIds;
        private readonly string text;
        private readonly bool withMarkdown;
        
        public DistributeMessageCommand(ITgBotClientEx tgClient, IEnumerable<ChatId> chatIds, string text, bool withMarkdown = false)
        {
            this.tgClient = tgClient;
            this.chatIds = chatIds;
            this.text = text;
            this.withMarkdown = withMarkdown;
        }

        public async Task ProcessAsync()
        {
            //todo avoid throttling
            foreach (var chatId in chatIds)
            {
                await tgClient.SendTextMessageAsync(chatId, text, withMarkdown ? ParseMode.Markdown : ParseMode.Default);
            }
        }
    }
}