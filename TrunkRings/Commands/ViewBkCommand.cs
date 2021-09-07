using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    public class ViewBkCommand : IBotCommand
    {
        private readonly IBkService bkService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public ViewBkCommand(IBkService bkService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.bkService = bkService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var records = bkService.GetAll().Select(x => x.Name + " " + x.Surname).ToList();

            if (records.Any())
                await tgClient.SendTextMessagesAsSingleTextAsync(chatId, records, "Список бухгалтеров:");
            else
                await tgClient.SendTextMessageAsync(chatId, "Список бухгалтеров пуст");
        }
    }
}