using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1
{
    public static class CommandMessageProcessor
    {
        public static async void ProcessTextMessage(TelegramBotClient bot, Message message)
        {
            var isMessagePersonal = message.Chat.Title == null;
            if (!isMessagePersonal)
                return;

            //TODO парсинг комманд вынести сюда

            if (message.Text.StartsWith("/help"))
            {
                const string helperMsg =
                        "Получить историю:"
                        + "\r\n/history: \"Название чата\" \"Дата начала\" \"Кол-во дней\""
                        + "\r\n/historyall: \"Дата начала\" \"Кол-во дней\""
                        + "\r\n/historyof: \"id аккаунта\" \"Дата начала\" \"Кол-во дней\""
                        + "Работа с админами:"
                        + "\r\n/addadmin: \"username\""
                        + "\r\n/removeadmin: \"username\""
                        + "\r\n/viewadmins"
                        + "Работа с бухгалтерами:"
                        + "\r\n/addbk: \"username\""
                        + "\r\n/removebk: \"username\""
                        + "\r\n/viewbk"
                        + "\r\n/viewwaiters"
                        + "Работа с чатами:"
                        + "\r\n/addonetimechat: \"chatname\""
                        + "\r\n/removeonetimechat: \"chatname\""
                        + "\r\n/viewonetimechats"
                        + "\r\n/viewinactivechats"
                    ;
                await bot.SendTextMessageAsync(message.Chat.Id, helperMsg);
            }
            else if (message.Text.StartsWith("/history"))
            {
                try
                {
                    using (var context = new MsgContext())
                    {
                        var command = new HistoryCommand(message.Text);

                        if (command.Type == HistoryCommandType.Unknown)
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                            return;
                        }

                        if (!IsUserDatabaseAbsoluteAdmin(context, message.From.Id))
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                            return;
                        }

                        var messageDataSets = context.Set<MessageDataSet>().AsNoTracking().GetActualDates(command);
                        if (!messageDataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "В данном периоде нет сообщений");
                            return;
                        }

                        messageDataSets = messageDataSets.GetActualChats(command);
                        if (messageDataSets == null || !messageDataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "В выбранных чатах нет сообщений");
                            return;
                        }

                        messageDataSets = messageDataSets.GetActualUser(command);
                        if (!messageDataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "По данному пользователю нет сообщений");
                            return;
                        }

                        var messagesByChatname = messageDataSets
                            .ToList()
                            .GroupBy(x => x.ChatId)
                            .Select(gdc =>
                            {
                                var dataSets = gdc.OrderBy(z => z.Date).ToList();
                                return new KeyValuePair<string, List<IMessageDataSet>>(
                                    dataSets.LastOrDefault()?.ChatName ?? "Чат",
                                    dataSets);
                            });
                        
                        if (!messagesByChatname.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "Нет данных за указанный период");
                            return;
                        }

                        var report = ReportCreator.Create(messagesByChatname, message.From.Id);

                        using (var fileStream = new FileStream(report.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var fileToSend = new InputOnlineFile(fileStream, "History.xls");
                            await bot.SendDocumentAsync(message.Chat.Id, fileToSend, "Отчет подготовлен");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    await bot.SendTextMessageAsync(message.Chat.Id, ex.Message);
                }
            }
            else if (message.Text.StartsWith("/add") || message.Text.StartsWith("/remove"))
            {
                try
                {
                    var command = new ManagingCommand(message.Text);
                    if (command.ManagingType == ManagingType.Unknown || command.EntityType == EntityType.Unknown)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                        return;
                    }

                    using (var context = new MsgContext())
                    {
                        var adminDataSets = context.Set<AdminDataSet>();
                        var messageDataSets = context.Set<MessageDataSet>();

                        if (adminDataSets.IsAdmin(message.From.Id))
                        {
                            switch (command.EntityType)
                            {
                                case EntityType.Admin:
                                {
                                    var user = messageDataSets.GetUserByUserName(command.EntityName);
                                    if (command.ManagingType == ManagingType.Add && !adminDataSets.IsAdmin(user.UserId))
                                        adminDataSets.Add(new AdminDataSet
                                        {
                                            AddTime = DateTime.UtcNow,
                                            AddedUserId = message.From.Id,
                                            AddedUserName = message.From.Username,
                                            UserId = user.UserId,
                                            UserName = command.EntityName
                                        });
                                    else if (command.ManagingType == ManagingType.Remove && adminDataSets.IsAdmin(user.UserId))
                                    {
                                        var adminDataSet = adminDataSets.Single(x =>
                                            x.UserId == user.UserId && x.DeleteTime == null);
                                        adminDataSet.DeleteTime = DateTime.UtcNow;
                                        adminDataSet.DeletedUserId = message.From.Id;
                                        adminDataSet.DeletedUserName = message.From.Username;
                                    }

                                    break;
                                }
                                case EntityType.Bookkeeper:
                                {
                                    var user = messageDataSets.GetUserByUserName(command.EntityName);
                                    var bkDataSets = context.Set<BookkeeperDataSet>();
                                    if (command.ManagingType == ManagingType.Add && !bkDataSets.Any(x => x.UserId == user.UserId))
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
                                    if (command.ManagingType == ManagingType.Add && !onetimeChatDataSets.Any(x => x.ChatId == chat.Id))
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
                            await bot.SendTextMessageAsync(message.Chat.Id, "Команда обработана");
                        }
                        else
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    await bot.SendTextMessageAsync(message.Chat.Id, ex.Message);
                }
            }
            else if (message.Text.StartsWith("/view"))
            {
                try
                {
                    var command = new ManagingCommand(message.Text);
                    if (command.ManagingType == ManagingType.Unknown || command.EntityType == EntityType.Unknown)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                        return;
                    }

                    bool isAdminAsking;
                    using (var context = new MsgContext())
                    {
                        var adminDataSets = context.Set<AdminDataSet>().AsNoTracking();
                        isAdminAsking = adminDataSets.IsAdmin(message.From.Id);
                    }

                    if (isAdminAsking)
                    {
                        switch (command.EntityType)
                        {
                            case EntityType.Admin:
                            {
                                string result;
                                using (var context = new MsgContext())
                                {
                                    var adminDataSets = context.Set<AdminDataSet>().AsNoTracking();
                                    var values = adminDataSets.Where(x => x.DeleteTime == null).ToList()
                                        .Select(x => x.UserName + " " + x.AddTime.ToShortDateString());
                                    result = string.Join("\r\n", values);
                                }

                                await bot.SendTextMessageAsync(message.Chat.Id, "Список админов:\r\n" + result);

                                break;
                            }
                            case EntityType.Bookkeeper:
                            {
                                string result;
                                using (var context = new MsgContext())
                                {
                                    var bkDataSets = context.Set<BookkeeperDataSet>().AsNoTracking();
                                    var values = bkDataSets.ToList()
                                        .Select(x => x.UserFirstName + " " + x.UserLastName);
                                    result = string.Join("\r\n", values);
                                }

                                await bot.SendTextMessageAsync(message.Chat.Id, "Список бухгалтеров:\r\n" + result);
                                break;
                            }
                            case EntityType.Waiter:
                            {
                                var sinceDate = DateTime.UtcNow.Date.AddMonths(-1);
                                var untilDate = DateTime.UtcNow.Date.AddMinutes(-30);
                                var waitersMessages = ViewWaitersProvider.GetWaiters(sinceDate, untilDate);
                                foreach (var msg in waitersMessages)
                                {
                                    var timeWithoutAnswer = DateTime.UtcNow.Subtract(msg.Date);
                                    var result = string.Format(
                                        @"В чате {0} сообщение от {1} {2}, оставленное {3}, без ответа ({4}). Текст сообщения: ""{5}"""
                                        , msg.ChatName, msg.UserLastName, msg.UserFirstName
                                        , msg.Date.AddHours(10).AddHours(-8).ToString("dd.MM.yyyy H:mm")
                                        , timeWithoutAnswer.Days + " дней " + timeWithoutAnswer.Hours + " часов " + timeWithoutAnswer.Minutes + " минут"
                                        , msg.Message);
                                    await bot.SendTextMessageAsync(message.Chat.Id, result);
                                }
                                break;
                            }
                            case EntityType.InactiveChatException:
                            {
                                string result;
                                using (var context = new MsgContext())
                                {
                                    var dataSets = context.Set<OnetimeChatDataSet>().AsNoTracking();
                                    var values = dataSets.ToList()
                                        .Select(x => x.ChatName);
                                    result = string.Join("\r\n", values);
                                }

                                await bot.SendTextMessageAsync(message.Chat.Id, "Список исключений для просмотра неактивных чатов:\r\n" + result);
                                break;
                            }
                            case EntityType.InactiveChat:
                            {
                                var sinceDate = DateTime.UtcNow.AddDays(-365);
                                var untilDate = DateTime.UtcNow;
                                var messageDataSets = new Dictionary<string, List<IMessageDataSet>>
                                {
                                    {"Неактивные", ViewInactiveChatsProvider.GetInactive(sinceDate, untilDate, TimeSpan.FromDays(7))}
                                };

                                var report = ReportCreator.Create(messageDataSets, message.From.Id);

                                using (var fileStream = new FileStream(report.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    var fileToSend = new InputOnlineFile(fileStream, "InactiveChats.xls");
                                    await bot.SendDocumentAsync(message.Chat.Id, fileToSend, "Отчет подготовлен");
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    await bot.SendTextMessageAsync(message.Chat.Id, ex.Message);
                }
            }
        }

        private static bool IsUserDatabaseAbsoluteAdmin(MsgContext context, int askerId)
        {
            return context.Set<AdminDataSet>().AsNoTracking().IsAdmin(askerId);
        }
    }
}