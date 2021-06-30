using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;

namespace TelegramBotTry1.Commands
{
    public class ViewBkCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public ViewBkCommand(ITgBotClientEx tgClient, ChatId chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var result = BookkeepersListProvider.GetRows();

            if (result.Error != null)
            {
                await tgClient.SendTextMessageAsync(chatId, result.Error);
                return;
            }

            await tgClient.SendTextMessagesAsSingleTextAsync(chatId, result.Records, result.Caption);
        }
    }
}