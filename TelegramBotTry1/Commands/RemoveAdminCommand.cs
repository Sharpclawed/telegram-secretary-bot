using System;
using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class RemoveAdminCommand : IBotCommand
    {
        public string AdminName { get; }
        public long UserId { get; }
        public string UserName { get; }

        public RemoveAdminCommand(string adminName, long addedUserId, string addedUsername)
        {
            AdminName = adminName;
            UserId = addedUserId;
            UserName = addedUsername;
        }

        public CommandResult Process()
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
            return new CommandResult { Message = "Команда обработана" };
        }
    }
}