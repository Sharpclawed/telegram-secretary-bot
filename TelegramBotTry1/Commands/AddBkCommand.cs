using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class AddBkCommand : IBotCommand
    {
        private readonly BkService bkService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string BkName { get; }

        public AddBkCommand(BkService bkService, ITgBotClientEx tgClient, ChatId chatId, string bkName)
        {
            this.bkService = bkService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            BkName = bkName;
        }

        public async Task ProcessAsync()
        {
            bkService.Make(BkName);

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}