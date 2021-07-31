using System.Linq;
using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.Commands
{
    public class ViewAdminsCommand : IBotCommand
    {
        private readonly IAdminService adminService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public ViewAdminsCommand(IAdminService adminService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.adminService = adminService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var records = adminService.GetAllActiveWithAddedBy().Select(Formatter.AdminWithAddedBy).ToList();

            if (!records.Any())
                await tgClient.SendTextMessageAsync(chatId, "Список админов пуст");
            else
                await tgClient.SendTextMessagesAsSingleTextAsync(chatId, records, "Список админов:");
        }
    }
}