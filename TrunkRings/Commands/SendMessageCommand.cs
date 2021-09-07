using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TrunkRings.Commands
{
    public class SendMessageCommand :IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        private readonly string text;
        private readonly bool withMarkdown;

        public SendMessageCommand(ITgBotClientEx tgClient, ChatId chatId, string text, bool withMarkdown = false)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            this.text = text;
            this.withMarkdown = withMarkdown;
        }

        public async Task ProcessAsync()
        {
            await tgClient.SendTextMessageAsync(chatId, text, withMarkdown ? ParseMode.Markdown : ParseMode.Default);
        }
    }
}