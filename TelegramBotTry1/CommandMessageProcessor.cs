using System;
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

                        //TODO записать, не разрывая флуент
                        if (command.Type == HistoryCommandType.Unknown)
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
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

                        var dataSets = messageDataSets
                            .ToList()
                            .GroupBy(x => x.ChatId)
                            .ToDictionary(gdc => gdc.Key, gdc => gdc.ToList());

                        if (IsUserDatabaseAbsoluteAdmin(context, message.From.Id))
                            dataSets = dataSets.CheckAskerRights(bot, message.From.Id);

                        if (!dataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "Нет данных за указанный период");
                            return;
                        }

                        var report = ReportCreator.Create(dataSets, message.From.Id);

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
                    var command = new UserManagingCommand(message.Text);
                    //TODO записать, не разрывая флуент
                    if (command.ManagingType == ManagingType.Unknown || command.UserType == UserEntityType.Unknown)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                        return;
                    }

                    using (var context = new MsgContext())
                    {
                        var adminDataSets = context.Set<AdminDataSet>();
                        var messageDataSets = context.Set<MessageDataSet>();
                        var bkDataSets = context.Set<BookkeeperDataSet>();
                        
                        if (adminDataSets.IsAdmin(message.From.Id))
                        {
                            var user = messageDataSets.GetUserByUserName(command.UserUserName);
                            if (command.UserType == UserEntityType.Admin)
                            {
                                if (command.ManagingType == ManagingType.Add && !adminDataSets.IsAdmin(user.UserId))
                                    adminDataSets.Add(new AdminDataSet
                                    {
                                        AddTime = DateTime.UtcNow,
                                        AddedUserId = message.From.Id,
                                        AddedUserName = message.From.Username,
                                        UserId = user.UserId,
                                        UserName = command.UserUserName
                                    });
                                else if (command.ManagingType == ManagingType.Remove && adminDataSets.IsAdmin(user.UserId))
                                {
                                    var adminDataSet = adminDataSets.Single(x => x.UserId == user.UserId && x.DeleteTime == null);
                                    adminDataSet.DeleteTime = DateTime.UtcNow;
                                    adminDataSet.DeletedUserId = message.From.Id;
                                    adminDataSet.DeletedUserName = message.From.Username;
                                }
                            }
                            else if (command.UserType == UserEntityType.Bookkeeper)
                            {
                                if (command.ManagingType == ManagingType.Add && !bkDataSets.Any(x => x.UserId == user.UserId))
                                    bkDataSets.Add(new BookkeeperDataSet
                                    {
                                        UserId = user.UserId,
                                        UserName = command.UserUserName,
                                        UserFirstName = user.Name,
                                        UserLastName = user.Surname
                                    });
                                else if (command.ManagingType == ManagingType.Remove)
                                {
                                    bkDataSets.Remove(bkDataSets.First(x => x.UserId == user.UserId));
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
                    var command = new UserViewCommand(message.Text);
                    //TODO записать, не разрывая флуент
                    if (command.ManagingType == ManagingType.Unknown || command.ContentType == UserEntityType.Unknown)
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                        return;
                    }

                    using (var context = new MsgContext())
                    {
                        var adminDataSets = context.Set<AdminDataSet>().AsNoTracking();
                        var bkDataSets = context.Set<BookkeeperDataSet>().AsNoTracking();
                        var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();

                        if (adminDataSets.IsAdmin(message.From.Id))
                        {
                            switch (command.ContentType)
                            {
                                case UserEntityType.Admin:
                                {
                                    var values = adminDataSets.Where(x => x.DeleteTime == null).ToList().Select(x => x.UserName + " " + x.AddTime.ToShortDateString());
                                    var result = string.Join("\r\n", values);
                                    await bot.SendTextMessageAsync(message.Chat.Id, "Список админов:\r\n" + result);
                                    break;
                                }
                                case UserEntityType.Bookkeeper:
                                {
                                    var values = bkDataSets.ToList().Select(x => x.UserFirstName + " " + x.UserLastName);
                                    var result = string.Join("\r\n", values);
                                    await bot.SendTextMessageAsync(message.Chat.Id, "Список бухгалтеров:\r\n" + result);
                                    break;
                                }
                                case UserEntityType.Waiter:
                                {
                                    //TODO Может в память выгрузить перед join?
                                    var allLastMessages =
                                            from msg in messageDataSets
                                            group msg by msg.ChatId
                                            into groups
                                            select groups.OrderByDescending(p => p.Date).FirstOrDefault()
                                        ;
                                    var lastMessagesFromDirectors = (
                                        from msg in allLastMessages
                                        from bookkeeper in bkDataSets
                                            .Where(bk => bk.UserId == msg.UserId).DefaultIfEmpty()
                                        from admin in adminDataSets.Where(x => x.DeleteTime == null)
                                            .Where(adm => adm.UserId == msg.UserId).DefaultIfEmpty()
                                        where bookkeeper == null && admin == null
                                                                 && msg.ChatName != null
                                        select new
                                        {
                                            msg.ChatName,
                                            msg.UserFirstName,
                                            msg.UserLastName,
                                            msg.Date,
                                            msg.Message
                                        }
                                    ).ToList();
                                    var lastMessages30 = lastMessagesFromDirectors.Take(30).ToList();
                                    foreach (var msg in lastMessages30)
                                    {
                                        var result = string.Format(
                                            @"В чате {0} сообщение от {1} {2}, оставленное в {3}, без ответа ({4}). Текст сообщения: ""{5}"""
                                            , msg.ChatName, msg.UserLastName, msg.UserFirstName,
                                            msg.Date.AddHours(10).AddHours(-8)
                                            , DateTime.UtcNow.Subtract(msg.Date).ToString(@"dd\.hh\:mm\:ss"),
                                            msg.Message);
                                        await bot.SendTextMessageAsync(message.Chat.Id, result);
                                    }

                                    if (lastMessagesFromDirectors.Count > 30)
                                        await bot.SendTextMessageAsync(message.Chat.Id,
                                            "Всего таких чатов " + lastMessagesFromDirectors.Count + ", показано 30");
                                    break;
                                }
                            }
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
        }

        private static bool IsUserDatabaseAbsoluteAdmin(MsgContext context, int askerId)
        {
            return context.Set<AdminDataSet>().AsNoTracking().IsAdmin(askerId);
        }
    }
}