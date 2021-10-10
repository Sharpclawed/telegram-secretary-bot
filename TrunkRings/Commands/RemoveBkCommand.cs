using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class RemoveBkCommand : IBotCommand
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
            var result = bkService.TryUnmake(BkName, out string message) ? "Команда обработана" : message;

            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}