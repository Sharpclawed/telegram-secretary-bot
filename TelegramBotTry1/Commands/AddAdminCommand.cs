using System;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class AddAdminCommand : IBotCommand
    {
        public string AdminName { get; }
        public long UserId { get; }
        public string UserName { get; }

        public AddAdminCommand(string adminName, long addedUserId, string addedUsername)
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
            return new CommandResult { Message = "Команда обработана" };
        }
    }
}