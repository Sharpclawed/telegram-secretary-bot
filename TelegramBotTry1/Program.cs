using System;
using System.IO;
using System.Linq;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace TelegramBotTry1
{
    static class Program
    { 
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient("361040811:AAGQlsM84JwDIRtcztbMMboKLXWqbPwW4VI");  //kontakt bot
            //new TelegramBotClient("245135166:AAEYEEsWjQmN_wLENwnA84Wb9xkgQJ-TLFE");   //my bot

        private static DateTime lastIAmAliveCheckUtc = DateTime.UtcNow.Date;

        static void Main()
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;
            Bot.OnReceiveGeneralError += BotOnOnReceiveGeneralError;
            Bot.OnUpdate += BotOnUpdate;
            Bot.OnCallbackQuery += BotOnOnCallbackQuery;
            
            var botId = Bot.BotId;
            Console.Title = "Secretary bot " + botId;

            using (var context = new MsgContext())
            {
                context.Database.CreateIfNotExists();
            }

            Console.WriteLine(DateTime.Now + " Start working");
            ConfigureIAmAliveTimer();

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            Console.WriteLine(DateTime.Now + " " + e.CallbackQuery.Message);
        }

        private static void BotOnOnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            Console.WriteLine(DateTime.Now + " " + e.Exception.Message + " \r\n" + e.Exception.InnerException + " \r\n" + e.Exception.StackTrace);
        }

        private static void BotOnUpdate(object sender, UpdateEventArgs updateEventArgs)
        {
            //Console.WriteLine(updateEventArgs.Update.Type.ToString());
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine(DateTime.Now + " " + receiveErrorEventArgs.ApiRequestException.Message);
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            //Console.WriteLine(DateTime.Now + " jstrcv " + messageEventArgs.Message);

            if (message == null)
                return;

            try
            {
                var recievedDataSet = new MessageDataSet(message);

                ProcessIfCommand(message);
                SaveContent(message, recievedDataSet);

                SaveToDatabase(recievedDataSet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.InnerException);
            }
        }

        private static void ProcessIfCommand(Message message)
        {
            if (message.Type == MessageType.Text)
                CommandMessageProcessor.ProcessTextMessage(Bot, message);
        }

        private static void SaveContent(Message message, IMessageDataSet messageDataSet)
        {
            switch (message.Type)
            {
                case MessageType.Document:
                    messageDataSet.Message =
                        ContentSaver.SaveDocument(Bot, message.Document.FileId, message.Document.FileName);
                    break;
                case MessageType.Voice:
                    messageDataSet.Message =
                        ContentSaver.SaveDocument(Bot, message.Voice.FileId, message.Voice.FileId + ".ogg");
                    break;
                case MessageType.Photo:
                    var photoToSave = message.Photo.OrderByDescending(x => x.FileSize).First();
                    messageDataSet.Message = ContentSaver.SaveDocument(Bot, photoToSave.FileId, photoToSave.FileId);
                    break;
            }
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

        //todo вынести в класс, возвращать таску
        private static async void ProcessTextMessage(Message message)
        {
            var isMessagePersonal = message.Chat.Title == null;
            const string helperMsg =
                "Получить историю:\r\n"
                + @"\r\n/history: ""Название чата"" ""Дата начала"" ""Кол-во дней"""
                + @"\r\n/historyall: ""Дата начала"" ""Кол-во дней"""
                + @"\r\n/historyof: ""id аккаунта"" ""Дата начала"" ""Кол-во дней"""
                ;

            if (message.Text.StartsWith("/help"))
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, helperMsg);
            }
            else if (message.Text.StartsWith("/history"))
            {
                if (!isMessagePersonal)
                    return;

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
                            .ToList()
                            .GroupBy(x => x.ChatId)
                            .ToDictionary(gdc => gdc.Key, gdc => gdc.ToList())
                            .CheckAskerRights(Bot, message.From.Id);
                        
                        if (!dataSets.Any())
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав на получение этой информации");
                            return;
                        }

                        var report = ReportCreator.Create(dataSets, message.From.Id);

                        using (var fileStream = new FileStream(report.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var fileToSend = new InputOnlineFile(fileStream, "History.xls");
                            await Bot.SendDocumentAsync(message.Chat.Id, fileToSend, "Отчет подготовлен");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Bot.SendTextMessageAsync(message.Chat.Id, ex.Message);
                    //await Bot.SendTextMessageAsync(message.Chat.Id, "Неверно введены параметры. Необходимо:\n" + helperMsg2);
                }
            }
        }
        
        private static void ConfigureIAmAliveTimer()
        {
            var iAmAliveTimer = new Timer
            {
                Interval = 1000 * 60 * 60 * 0.5 //30 минут
            };
            iAmAliveTimer.Elapsed += ShowASignEvent;
            iAmAliveTimer.AutoReset = true;
            iAmAliveTimer.Start();
        }

        private static void ShowASignEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(10).AddHours(-8); //7 часов относительно Гринвича, около того
                if (DateTime.UtcNow > scheduledRunUtc)
                {
                    if (scheduledRunUtc.Date > lastIAmAliveCheckUtc.Date)
                    {
                        ShowASign();
                        lastIAmAliveCheckUtc = DateTime.UtcNow;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        private static void ShowASign()
        {
            //var chatId = -219324188; //чат 125
            long chatId = -1001100176543; //чат БотВажное
            Bot.SendTextMessageAsync(new ChatId(chatId), "Работаю в штатном режиме");
            Console.WriteLine("sm Работаю в штатном режиме");
        }
    }
}