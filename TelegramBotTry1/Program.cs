﻿using System;
using System.IO;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotTry1
{
    static class Program
    { 
        private static readonly TelegramBotClient Bot =
            //new TelegramBotClient("361040811:AAGQlsM84JwDIRtcztbMMboKLXWqbPwW4VI"); 
            new TelegramBotClient("245135166:AAEYEEsWjQmN_wLENwnA84Wb9xkgQJ-TLFE");

        static void Main()
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;
            Bot.OnUpdate += BotOnUpdate;

            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            using (var context = new MsgContext())
            {
                context.Database.CreateIfNotExists();
            }

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnUpdate(object sender, UpdateEventArgs updateEventArgs)
        {
            //Console.WriteLine(updateEventArgs.Update.Type.ToString());
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            //Debugger.Break();
            Console.WriteLine(DateTime.Now + " " + receiveErrorEventArgs.ApiRequestException.Message);
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null) return;

            try
            {
                var messageDataSet = new MessageDataSet(message);

                var processStatus = ProcessMessage(message, messageDataSet);

                SaveToDatabase(messageDataSet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static bool ProcessMessage(Message message, MessageDataSet messageDataSet)
        {
            try
            {
                switch (message.Type)
                {
                    case MessageType.TextMessage:
                        ProcessTextMessage(message);
                        break;
                    case MessageType.DocumentMessage:
                        ContentSaver.SaveDocument(Bot, message.Document.FileId, messageDataSet.Message);
                        break;
                    case MessageType.VoiceMessage:
                        ContentSaver.SaveDocument(Bot, message.Voice.FileId, messageDataSet.Message);
                        break;
                    case MessageType.PhotoMessage:
                        if (messageDataSet.Message != null)
                            ContentSaver.SaveDocument(Bot, message.Photo[0].FileId, messageDataSet.Message);
                        break;
                    case MessageType.AudioMessage:
                    case MessageType.UnknownMessage:
                    case MessageType.VideoMessage:
                    case MessageType.StickerMessage:
                    case MessageType.LocationMessage:
                    case MessageType.ContactMessage:
                    case MessageType.ServiceMessage:
                    case MessageType.VenueMessage:
                    default:
                        Console.WriteLine(message.Type.ToString());
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return false;
        }

        private static void SaveToDatabase(MessageDataSet messageDataSet)
        {
            using (var context = new MsgContext())
            {
                context.MessageDataSets.Add(messageDataSet);
                context.SaveChanges();
                Console.WriteLine(messageDataSet.ToString());
            }
        }

        private static async void ProcessTextMessage(Message message)
        {
            var isMessagePersonal = message.Chat.Title == null;
            var helperMsg = "Протоколирую чат. По всем вопросам обращаться к @VictoriaBushueva";
            const string helperMsg2 =
                "\nПолучить историю:\n" + @"/history: Название_чата Дата(dd.MM.yyyy) Количество_дней";
            if (isMessagePersonal)
                helperMsg += helperMsg2;

            if (message.Text.StartsWith("/start"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, helperMsg);
            }
            else if (message.Text.StartsWith("/help"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, helperMsg);
            }
            else if (message.Text.StartsWith("/history"))
            {
                if (!isMessagePersonal) return;
                try
                {
                    using (var context = new MsgContext())
                    {
                        //TODO записать, не разрывая флуент
                        var commandConfig = new HistoryCommandConfig(message.Text);

                        if (commandConfig.Type == HistoryCommandType.Unknown)
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда");
                            return;
                        }

                        var messageDataSets = context.Set<MessageDataSet>().GetActualDates(commandConfig);
                        if (!messageDataSets.Any())
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "В данном периоде нет сообщений");
                            return;
                        }

                        messageDataSets = messageDataSets.GetActualChats(commandConfig);
                        if (messageDataSets == null || !messageDataSets.Any())
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "В выбранных чатах нет сообщений");
                            return;
                        }

                        messageDataSets = messageDataSets.GetActualUser(commandConfig);
                        if (!messageDataSets.Any())
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "По данному пользователю нет сообщений");
                            return;
                        }

                        var dataSets = messageDataSets
                            .GroupBy(x => x.ChatId)
                            .ToDictionary(gdc => gdc.Key, gdc => gdc.ToList())
                            .CheckAskerRights(Bot, message.From.Id);
                        
                        if (!dataSets.Any())
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав на получение этой информации");
                            return;
                        }

                        var file = FileCreator.SendFeedback(dataSets, message.From.Id);

                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var fts = new FileToSend("History.xls", fileStream);
                            await Bot.SendDocumentAsync(message.Chat.Id, fts, "catch");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Bot.SendTextMessageAsync(message.Chat.Id, ex.Message);
                    //await Bot.SendTextMessageAsync(message.Chat.Id, "Неверно введены параметры. Необходимо:\n" + helperMsg2);
                }
            }
        }
    }
}