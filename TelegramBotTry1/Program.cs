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

                MonitorMessage(message, recievedDataSet);

                SaveToDatabase(recievedDataSet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private static void MonitorMessage(Message message, IMessageDataSet messageDataSet)
        {
            try
            {
                switch (message.Type)
                {
                    case MessageType.Text:
                        ProcessTextMessage(message);
                        break;
                    case MessageType.Document:
                        messageDataSet.Message =
                            ContentSaver.SaveDocument(Bot, message.Document.FileId, message.Document.FileName).Result;
                        break;
                    case MessageType.Voice:
                        messageDataSet.Message =
                            ContentSaver.SaveDocument(Bot, message.Voice.FileId, message.Voice.FileId + ".ogg").Result;
                        break;
                    case MessageType.Photo:
                        var photoToSave = message.Photo
                                .OrderByDescending(x => x.FileSize)
                                .First();

                        messageDataSet.Message = ContentSaver.SaveDocument(Bot, photoToSave.FileId, photoToSave.FileId)
                            .Result;
                        break;
                    case MessageType.Unknown:
                    case MessageType.Audio:
                    case MessageType.Video:
                    case MessageType.Sticker:
                    case MessageType.Location:
                    case MessageType.Contact:
                    case MessageType.Venue:
                    case MessageType.Game:
                    case MessageType.VideoNote:
                    case MessageType.Invoice:
                    case MessageType.SuccessfulPayment:
                    case MessageType.WebsiteConnected:
                    case MessageType.ChatMembersAdded:
                    case MessageType.ChatMemberLeft:
                    case MessageType.ChatTitleChanged:
                    case MessageType.ChatPhotoChanged:
                    case MessageType.MessagePinned:
                    case MessageType.ChatPhotoDeleted:
                    case MessageType.GroupCreated:
                    case MessageType.SupergroupCreated:
                    case MessageType.ChannelCreated:
                    case MessageType.MigratedToSupergroup:
                    case MessageType.MigratedFromGroup:
                    case MessageType.Animation:
                    case MessageType.Poll:
                    default:
                        Console.WriteLine(message.Type.ToString());
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

                        var file = FileCreator.SendFeedback(dataSets, message.From.Id);

                        using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            var fts = new InputOnlineFile(fileStream, "History.xls");
                            await Bot.SendDocumentAsync(message.Chat.Id, fts, "catch");
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
                Interval = 1000 * 60 * 60 * 2 //2 часа
            };
            iAmAliveTimer.Elapsed += ShowASignEvent;
            iAmAliveTimer.AutoReset = true;
            iAmAliveTimer.Start();
        }

        private static void ShowASignEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(10).AddHours(-5); //10 часов относительно Гринвича, около того
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
            var chatId = 1100176543; //чат БотВажное
            Bot.SendTextMessageAsync(new ChatId(chatId), "Работаю в штатном режиме");
            Console.WriteLine("sm Работаю в штатном режиме");
        }
    }
}