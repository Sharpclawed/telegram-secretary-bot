using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Domain;

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
            using (var context = new MsgContext())
            {
                var adminDataSets = context.Set<AdminDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(AdminName);
                if (adminDataSets.IsAdmin(user.UserId))
                {
                    var adminDataSet = adminDataSets.First(x =>
                        x.UserId == user.UserId && x.DeleteTime == null);
                    adminDataSet.DeleteTime = DateTime.UtcNow;
                    adminDataSet.DeletedUserId = UserId;
                    adminDataSet.DeletedUserName = UserName;
                    context.SaveChanges();
                }
            }

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}