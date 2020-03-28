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

            if (message.Text.StartsWith("/help"))
            {
                const string helperMsg =
                        "Получить историю:\r\n"
                        + @"\r\n/history: ""Название чата"" ""Дата начала"" ""Кол-во дней"""
                        + @"\r\n/historyall: ""Дата начала"" ""Кол-во дней"""
                        + @"\r\n/historyof: ""id аккаунта"" ""Дата начала"" ""Кол-во дней"""
                    ;
                await bot.SendTextMessageAsync(message.Chat.Id, helperMsg);
            }
            else if (message.Text.StartsWith("/history"))
            {
                try
                {
                    using (var context = new MsgContext())
                    {
                        //TODO Вынести в Command Provider
                        var commandConfig = new HistoryCommand(message.Text);

                        //TODO записать, не разрывая флуент
                        if (commandConfig.Type == HistoryCommandType.Unknown)
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                            return;
                        }

                        var messageDataSets = context.Set<MessageDataSet>().GetActualDates(commandConfig);
                        if (!messageDataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "В данном периоде нет сообщений");
                            return;
                        }

                        messageDataSets = messageDataSets.GetActualChats(commandConfig);
                        if (messageDataSets == null || !messageDataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "В выбранных чатах нет сообщений");
                            return;
                        }

                        messageDataSets = messageDataSets.GetActualUser(commandConfig);
                        if (!messageDataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "По данному пользователю нет сообщений");
                            return;
                        }

                        var dataSets = messageDataSets
                            .ToList()
                            .GroupBy(x => x.ChatId)
                            .ToDictionary(gdc => gdc.Key, gdc => gdc.ToList())
                            .CheckAskerRights(bot, message.From.Id);

                        if (!dataSets.Any())
                        {
                            await bot.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав на получение этой информации");
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
                    Console.WriteLine("/history " + ex.Message);
                    await bot.SendTextMessageAsync(message.Chat.Id, ex.Message);
                }
            }
        }
    }
}