using System;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.Commands
{
    public class AddAdminCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string AdminName { get; }
        public long UserId { get; }
        public string UserName { get; }

        public AddAdminCommand(ITgBotClientEx tgClient, ChatId chatId, string adminName, long addedUserId, string addedUsername)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            AdminName = adminName;
            UserId = addedUserId;
            UserName = addedUsername;
        }

        public async Task ProcessAsync()
        {
            using (var context = new SecretaryContext())
            {
                var adminDataSets = context.Set<AdminDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(AdminName);
                adminDataSets.Add(new AdminDataSet
                {
                    AddTime = DateTime.UtcNow,
                    AddedUserId = UserId,
                    AddedUserName = UserName,
                    UserId = user.UserId,
                    UserName = AdminName
                });
                context.SaveChanges();
            }

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}