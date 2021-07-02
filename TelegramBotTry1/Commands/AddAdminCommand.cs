using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Domain;
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
            using (var context = new MsgContext())
            {
                var adminDataSets = context.Set<AdminDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(AdminName);
                if (!adminDataSets.IsAdmin(user.UserId))
                {
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
            }

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}