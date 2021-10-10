using System.Threading.Tasks;
using TrunkRings.Domain.Models;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class AddAdminCommand : IBotCommand
    {
        private readonly IAdminService adminService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string AdminName { get; }
        public long AddedUserId { get; }
        public string AddedUserName { get; }

        public AddAdminCommand(IAdminService adminService, ITgBotClientEx tgClient, ChatId chatId, string adminName, long addedUserId, string addedUsername)
        {
            this.adminService = adminService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            AdminName = adminName;
            AddedUserId = addedUserId;
            AddedUserName = addedUsername;
        }

        public async Task ProcessAsync()
        {
            var addedBy = new Admin {UserId = AddedUserId, UserName = AddedUserName};
            if (adminService.TryMake(AdminName, addedBy, out string resultMessage))
                await tgClient.SendTextMessageAsync(chatId, "Команда обработана");
            else
                await tgClient.SendTextMessageAsync(chatId, resultMessage);
        }
    }
}