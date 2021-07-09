using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class RemoveAdminCommand : IBotCommand
    {
        private readonly AdminService adminService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string AdminName { get; }
        public long UserId { get; }
        public string UserName { get; }

        public RemoveAdminCommand(AdminService adminService, ITgBotClientEx tgClient, ChatId chatId, string adminName, long removedByUserId, string removedByUsername)
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
            var succeeded = adminService.Unmake(AdminName, removedBy);
            var result = succeeded ? "Команда обработана" : "Пользователь не найден";

            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}