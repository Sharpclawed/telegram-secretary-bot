using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class RemoveBkCommand : IBotCommand
    {
        private readonly IBkService bkService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string BkName { get; }

        public RemoveBkCommand(IBkService bkService, ITgBotClientEx tgClient, ChatId chatId, string bkName)
        {
            this.bkService = bkService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            BkName = bkName;
        }

        public async Task ProcessAsync()
        {
            var succeeded = bkService.Unmake(BkName);
            var result = succeeded ? "Команда обработана" : "Пользователь не найден";

            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}