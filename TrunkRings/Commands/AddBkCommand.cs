using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    public class AddBkCommand : IBotCommand
    {
        private readonly IBkService bkService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string BkName { get; }

        public AddBkCommand(IBkService bkService, ITgBotClientEx tgClient, ChatId chatId, string bkName)
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