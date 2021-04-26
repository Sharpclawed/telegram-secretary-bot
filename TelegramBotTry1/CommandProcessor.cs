using System;
using System.Linq;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1
{
    //returns dto for bot or whatever while providers return result data only
    public static class CommandProcessor
    {
        //todo Command на вход, а не пустота
        public static ViewReportResult ProcessHistoryCommand(HistoryCommand command)
        {
            return HistoryProvider.GetRows(command);
        }

        public static ViewEntitiesResult ProcessViewAdminsList()
        {
            return AdminsListProvider.GetRows();
        }

        public static ViewEntitiesResult ProcessViewBookkeepersList()
        {
            return BookkeepersListProvider.GetRows();
        }

        public static ViewEntitiesResult ProcessViewInactiveChatExceptions()
        {
            return IgnoreChatsListProvider.GetRows();
        }

        public static ViewEntitiesResult ProcessViewWaiters()
        {
            var sinceDate = DateTime.UtcNow.Date.AddMonths(-1);
            var untilDate = DateTime.UtcNow.Date.AddMinutes(-30);
            //todo must return GetWaiters and use formatting wrap later choosing type of result call
            //var waitersReport = ViewWaitersProvider.GetWaiters(sinceDate, untilDate);
            var waitersReport = ViewWaitersProvider.GetWaitersFormatted(sinceDate, untilDate);
            return new ViewEntitiesResult { Records = waitersReport };
        }

        public static ViewReportResult ProcessViewInactiveChats()
        {
            var sinceDate = DateTime.UtcNow.AddDays(-365);
            var untilDate = DateTime.UtcNow;
            var report = ViewInactiveChatsProvider.GetInactive(sinceDate, untilDate, TimeSpan.FromDays(7));

            return new ViewReportResult {Records = report, Caption = "Отчет по неактивным чатам"};
        }

        public static ViewActionResult ProcessSetDataCommand(ManagingCommand command)
        {
            using (var context = new MsgContext())
            {
                var adminDataSets = context.Set<AdminDataSet>();
                var messageDataSets = context.Set<MessageDataSet>();

                switch (command.EntityType)
                {
                    case EntityType.Admin:
                    {
                        var user = messageDataSets.GetUserByUserName(command.EntityName);
                        if (command.ManagingType == ManagingType.Add && !adminDataSets.IsAdmin(user.UserId))
                            adminDataSets.Add(new AdminDataSet
                            {
                                AddTime = DateTime.UtcNow,
                                AddedUserId = command.UserId.Value,
                                AddedUserName = command.UserName,
                                UserId = user.UserId,
                                UserName = command.EntityName
                            });
                        else if (command.ManagingType == ManagingType.Remove &&
                                 adminDataSets.IsAdmin(user.UserId))
                        {
                            var adminDataSet = adminDataSets.Single(x =>
                                x.UserId == user.UserId && x.DeleteTime == null);
                            adminDataSet.DeleteTime = DateTime.UtcNow;
                            adminDataSet.DeletedUserId = command.UserId.Value;
                            adminDataSet.DeletedUserName = command.UserName;
                        }

                        break;
                    }
                    case EntityType.Bookkeeper:
                    {
                        var user = messageDataSets.GetUserByUserName(command.EntityName);
                        var bkDataSets = context.Set<BookkeeperDataSet>();
                        if (command.ManagingType == ManagingType.Add &&
                            !bkDataSets.Any(x => x.UserId == user.UserId))
                            bkDataSets.Add(new BookkeeperDataSet
                            {
                                UserId = user.UserId,
                                UserName = command.EntityName,
                                UserFirstName = user.Name,
                                UserLastName = user.Surname
                            });
                        else if (command.ManagingType == ManagingType.Remove)
                            bkDataSets.Remove(bkDataSets.First(x => x.UserId == user.UserId));

                        break;
                    }
                    case EntityType.InactiveChatException:
                    {
                        var chat = messageDataSets.GetChatByChatName(command.EntityName);
                        var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                        if (command.ManagingType == ManagingType.Add &&
                            !onetimeChatDataSets.Any(x => x.ChatId == chat.Id))
                            onetimeChatDataSets.Add(new OnetimeChatDataSet
                            {
                                ChatName = chat.Name,
                                ChatId = chat.Id
                            });
                        else if (command.ManagingType == ManagingType.Remove)
                            onetimeChatDataSets.Remove(onetimeChatDataSets.First(x => x.ChatId == chat.Id));

                        break;
                    }
                }

                context.SaveChanges();
                return new ViewActionResult {Message = "Команда обработана"};
            }
        }
    }
}