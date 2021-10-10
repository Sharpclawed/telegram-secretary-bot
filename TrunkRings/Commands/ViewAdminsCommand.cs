using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;
using TrunkRings.DomainExtensions;

namespace TrunkRings.Commands
{
    class ViewAdminsCommand : IBotCommand
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