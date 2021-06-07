using System;
using System.Linq;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Dto
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

        public ViewReportResult Process()
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
            return new ViewReportResult { Message = "Команда обработана" };
        }
    }

    public class AddBkCommand : IBotCommand
    {
        public string BkName { get; }

        public AddBkCommand(string bkName)
        {
            BkName = bkName;
        }

        public ViewReportResult Process()
        {
            using (var context = new MsgContext())
            {
                var bkDataSets = context.Set<BookkeeperDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(BkName);
                if (!bkDataSets.Any(x => x.UserId == user.UserId))
                {
                    bkDataSets.Add(new BookkeeperDataSet
                    {
                        UserId = user.UserId,
                        UserName = user.Username,
                        UserFirstName = user.Name,
                        UserLastName = user.Surname
                    });
                    context.SaveChanges();
                }
            }
            return new ViewReportResult { Message = "Команда обработана" };
        }
    }

    public class AddOnetimeChatCommand : IBotCommand
    {
        public string ChatName { get; }

        public AddOnetimeChatCommand(string chatName)
        {
            ChatName = chatName;
        }

        public ViewReportResult Process()
        {
            using (var context = new MsgContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                var chat = messageDataSets.GetChatByChatName(ChatName);
                if (!onetimeChatDataSets.Any(x => x.ChatId == chat.Id))
                {
                    onetimeChatDataSets.Add(new OnetimeChatDataSet
                    {
                        ChatName = chat.Name,
                        ChatId = chat.Id
                    });
                    context.SaveChanges();
                }
            }

            return new ViewReportResult { Message = "Команда обработана" };
        }
    }

    public class ViewAdminsCommand : IBotCommand
    {
        public ViewReportResult Process()
        {
            return AdminsListProvider.GetRows();
        }
    }

    public class ViewBkCommand : IBotCommand
    {
        public ViewReportResult Process()
        {
            return BookkeepersListProvider.GetRows();
        }
    }

    public class ViewWaitersCommand : IBotCommand
    {
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewWaitersCommand()
        {
        }

        public ViewWaitersCommand(DateTime sinceDate, DateTime untilDate)
        {
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }

        public ViewReportResult Process()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.Date.AddMonths(-1);
            var untilDateValue = UntilDate ?? DateTime.UtcNow.Date.AddMinutes(-30);
            var waitersReport = ViewWaitersProvider.GetWaiters(sinceDateValue, untilDateValue);

            return new ViewReportResult { Messages = waitersReport, Caption = "Отчет по неотвеченным сообщениям" };
        }
    }

    public class ViewInactiveChatsCommand : IBotCommand
    {
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewInactiveChatsCommand()
        {
        }

        public ViewInactiveChatsCommand(DateTime sinceDate, DateTime untilDate)
        {
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }

        public ViewReportResult Process()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.AddDays(-365);
            var untilDateValue = UntilDate ?? DateTime.UtcNow;
            var report = ViewInactiveChatsProvider.GetInactive(sinceDateValue, untilDateValue, TimeSpan.FromDays(7));

            return new ViewReportResult { Messages = report, Caption = "Отчет по неактивным чатам" };
        }
    }

    public class ViewOneTimeChatsCommand : IBotCommand
    {
        public ViewReportResult Process()
        {
            return IgnoreChatsListProvider.GetRows();
        }
    }

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

        public ViewReportResult Process()
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
            return new ViewReportResult { Message = "Команда обработана" };
        }
    }

    public class RemoveBkCommand : IBotCommand
    {
        public string BkName { get; }

        public RemoveBkCommand(string bkName)
        {
            BkName = bkName;
        }

        public ViewReportResult Process()
        {
            using (var context = new MsgContext())
            {
                var bkDataSets = context.Set<BookkeeperDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(BkName);
                bkDataSets.Remove(bkDataSets.First(x => x.UserId == user.UserId));
                context.SaveChanges();
            }
            return new ViewReportResult { Message = "Команда обработана" };
        }
    }

    public class RemoveOnetimeChatCommand : IBotCommand
    {
        public string ChatName { get; }

        public RemoveOnetimeChatCommand(string chatName)
        {
            ChatName = chatName;
        }

        public ViewReportResult Process()
        {
            using (var context = new MsgContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                var chat = messageDataSets.GetChatByChatName(ChatName);
                onetimeChatDataSets.Remove(onetimeChatDataSets.First(x => x.ChatId == chat.Id));
                context.SaveChanges();
            }

            return new ViewReportResult { Message = "Команда обработана" };
        }
    }
}