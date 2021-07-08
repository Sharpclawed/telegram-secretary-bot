using System;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.Commands
{
    public class RemoveAdminCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string AdminName { get; }
        public long UserId { get; }
        public string UserName { get; }

        public RemoveAdminCommand(ITgBotClientEx tgClient, ChatId chatId, string adminName, long addedUserId, string addedUsername)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            AdminName = adminName;
            UserId = addedUserId;
            UserName = addedUsername;
        }

        public async Task ProcessAsync()
        {
            string result;
            using (var context = new SecretaryContext())
            {
                var adminDataSets = context.Set<AdminDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(AdminName);
                var adminToRemove = adminDataSets.FirstOrDefault(x => x.UserId == user.UserId && x.DeleteTime == null);
                if (adminToRemove != null)
                {
                    adminToRemove.DeleteTime = DateTime.UtcNow;
                    adminToRemove.DeletedUserId = UserId;
                    adminToRemove.DeletedUserName = UserName;
                    context.SaveChanges();
                    result = "Команда обработана";
                }
                else
                    result = "Пользователь не найден";
            }

            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}