using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1
{
    public static class CommandMessageProcessor
    {
        public static async Task ProcessTextMessage(TelegramBotClient bot, Message message)
        {
            var botClientWrapper = new BotClientWrapper(bot);

            var isMessagePersonal = message.Chat.Title == null;
            if (!isMessagePersonal)
                return;

            //TODO парсинг комманд вынести сюда
            if (message.Text.StartsWith("/help"))
            {
                const string helperMsg =
                        "*Получить историю:*"
                        + "\r\n/history: \"Название чата\" \"Дата начала\" \"Кол-во дней\" — _выдает в Эксель-файл историю переписки в указанном чате с даты начала на количество дней вперед_"
                        + "\r\n/historyall: \"Дата начала\" \"Кол-во дней\" — _выдает выдает в Эксель-файл историю переписки всех частов, где есть бот с даты начала на количество дней вперед_"
                        + "\r\n/historyof: \"id аккаунта\" \"Дата начала\" \"Кол-во дней\" — _выдает в Эксель-файл историю переписки по чатам бухгалтера с указанным id аккаунта с даты начала на количество дней вперед_"
                        + "\r\n\r\n*Работа с админами:*"
                        + "\r\n/addadmin: \"username\" — _добавить пользователя телеграмм в админы чат-бота - добавляем по юзернейм, могут запускать только админы_"
                        + "\r\n/removeadmin: \"username\" — _убрать пользователя телеграмм из админов чат-бота, убрать по юзернейм, могут запускать только админы_"
                        + "\r\n/viewadmins — _выводит список пользователей телеграмм, которые являются админами чат-бота, могут запускать только админы_"
                        + "\r\n\r\n*Работа с бухгалтерами:*"
                        + "\r\n/addbk: \"username\" — _добавить пользователя телеграмм в бухгалтеры для  чат-бота - добавляем по юзернейм, команду запускает админ. Нужно обозначить бухгалтеров, чтоб неотвеченные сообщения только от директоров проверять_"
                        + "\r\n/removebk: \"username\" — _убрать пользователя телеграмм из бухгалтеров для  чат-бота - добавляем по юзернейм, команду запускает админ_"
                        + "\r\n/viewbk — _выводит список пользователей телеграмм, которые помеченные  бухгалтерами для чат-бота, могут запускать только админы_"
                        + "\r\n/viewwaiters — _выдает в чат список чатов, где есть неотвеченные больше 2 часов сообщения от директора_"
                        + "\r\n\r\n*Работа с чатами:*"
                        + "\r\n/addonetimechat: \"chatname\" — _пометить чат как неактивный, по которому не надо контролировать молчание директора в течении недели_"
                        + "\r\n/removeonetimechat: \"chatname\" — _убрать пометку неактивности с чата_"
                        + "\r\n/viewonetimechats — _вывести список чатов, помеченными неактивными_"
                        + "\r\n/viewinactivechats — _вывести в Эксель список чатов, в которых не было сообщений от директора больше недели_"
                    ;
                await bot.SendTextMessageAsync(message.Chat.Id, helperMsg, ParseMode.Markdown);
            }
            else if (message.Text.StartsWith("/history"))
            {
                var command = new HistoryCommand(message.Text);
                var isAdminAsking = DbRepository.IsAdmin(message.From.Id);
                var historyResult = HistoryProvider.GetRows(command, isAdminAsking);

                if (historyResult.Error != null)
                    await bot.SendTextMessageAsync(message.Chat.Id, historyResult.Error);
                else
                    await botClientWrapper.SendTextMessagesAsExcelReportAsync(
                        message.Chat.Id,
                        historyResult.Records,
                        "История сообщений",
                        new[]
                        {
                            nameof(IMessageDataSet.Date),
                            nameof(IMessageDataSet.Message),
                            nameof(IMessageDataSet.UserFirstName),
                            nameof(IMessageDataSet.UserLastName),
                            nameof(IMessageDataSet.UserName),
                            nameof(IMessageDataSet.UserId)
                        },
                        msg => msg.ChatName);
            }
            else if (message.Text.StartsWith("/add") || message.Text.StartsWith("/remove"))
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
                                else if (command.ManagingType == ManagingType.Remove &&
                                         adminDataSets.IsAdmin(user.UserId))
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
                        await bot.SendTextMessageAsync(message.Chat.Id, "Команда обработана");
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                    }
                }
            }
            else if (message.Text.StartsWith("/view"))
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
                            IEnumerable<string> result;
                            using (var context = new MsgContext())
                            {
                                var adminDataSets = context.Set<AdminDataSet>().AsNoTracking();
                                result = adminDataSets.Where(x => x.DeleteTime == null).ToList()
                                    .Select(x => x.UserName + " " + x.AddTime.ToShortDateString());
                            }

                            await botClientWrapper.SendTextMessagesAsSingleTextAsync(message.Chat.Id, result,
                                "Список админов:\r\n");
                            break;
                        }
                        case EntityType.Bookkeeper:
                        {
                            IEnumerable<string> result;
                            using (var context = new MsgContext())
                            {
                                var bkDataSets = context.Set<BookkeeperDataSet>().AsNoTracking();
                                result = bkDataSets.ToList()
                                    .Select(x => x.UserFirstName + " " + x.UserLastName);
                            }

                            await botClientWrapper.SendTextMessagesAsSingleTextAsync(message.Chat.Id, result,
                                "Список бухгалтеров:\r\n");
                            break;
                        }
                        case EntityType.Waiter:
                        {
                            var sinceDate = DateTime.UtcNow.Date.AddMonths(-1);
                            var untilDate = DateTime.UtcNow.Date.AddMinutes(-30);
                            var waitersReport = ViewWaitersProvider.GetWaitersFormatted(sinceDate, untilDate);
                            await botClientWrapper.SendTextMessagesAsListAsync(message.Chat.Id, waitersReport,
                                ChatType.Personal);
                            break;
                        }
                        case EntityType.InactiveChatException:
                        {
                            IEnumerable<string> result;
                            using (var context = new MsgContext())
                            {
                                var dataSets = context.Set<OnetimeChatDataSet>().AsNoTracking();
                                result = dataSets.ToList()
                                    .Select(x => x.ChatName);
                            }

                            await botClientWrapper.SendTextMessagesAsSingleTextAsync(message.Chat.Id, result
                                , "Список исключений для просмотра неактивных чатов:\r\n");
                            break;
                        }
                        case EntityType.InactiveChat:
                        {
                            var sinceDate = DateTime.UtcNow.AddDays(-365);
                            var untilDate = DateTime.UtcNow;

                            await botClientWrapper.SendTextMessagesAsExcelReportAsync(
                                message.Chat.Id,
                                ViewInactiveChatsProvider.GetInactive(sinceDate, untilDate, TimeSpan.FromDays(7)),
                                "Отчет по неактивным чатам",
                                new[]
                                {
                                    nameof(IMessageDataSet.Date),
                                    nameof(IMessageDataSet.ChatName),
                                    nameof(IMessageDataSet.Message),
                                    nameof(IMessageDataSet.UserFirstName),
                                    nameof(IMessageDataSet.UserLastName),
                                    nameof(IMessageDataSet.UserName),
                                    nameof(IMessageDataSet.UserId)
                                });

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
    }
}