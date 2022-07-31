using System.Threading.Tasks;
using TrunkRings.Domain.Models;
using TrunkRings.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class RemoveAdminCommand : IBotCommand
    {
        private readonly IAdminService adminService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string AdminName { get; }
        public long UserId { get; }
        public string UserName { get; }

        public RemoveAdminCommand(IAdminService adminService, ITgBotClientEx tgClient, ChatId chatId, string adminName, long removedByUserId, string removedByUsername)
        {
            this.adminService = adminService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            AdminName = adminName;
            UserId = removedByUserId;
            UserName = removedByUsername;
        }

        public async Task ProcessAsync()
        {
            var removedBy = new Admin { UserId = UserId, UserName = UserName };
            var resultMessage = adminService.TryUnmake(AdminName, removedBy, out string message) ? "Команда обработана" : message;

            await tgClient.SendTextMessageAsync(chatId, resultMessage);
        }
    }
}