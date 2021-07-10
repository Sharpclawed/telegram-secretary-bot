using System.Linq;
using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class ViewBkCommand : IBotCommand
    {
        private readonly BkService bkService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public ViewBkCommand(BkService bkService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.bkService = bkService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var records = bkService.GetAll().Select(x => x.Name + " " + x.Surname).ToList();

            if (records.Any())
                await tgClient.SendTextMessagesAsSingleTextAsync(chatId, records, "Список бухгалтеров:\r\n");
            else
                await tgClient.SendTextMessageAsync(chatId, "Список бухгалтеров пуст");
        }
    }
}