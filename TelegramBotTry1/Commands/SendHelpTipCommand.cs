using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.DataProviders;

namespace TelegramBotTry1.Commands
{
    public class SendHelpTipCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public SendHelpTipCommand(ITgBotClientEx tgClient, ChatId chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var helpTip = TipProvider.GetHelpTip();

            await tgClient.SendTextMessageAsync(chatId, helpTip, ParseMode.Markdown);
        }
    }
}