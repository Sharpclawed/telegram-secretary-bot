using System;
using System.Linq;
using System.Net.Sockets;
using System.Timers;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    static class Program
    { 
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient("361040811:AAGQlsM84JwDIRtcztbMMboKLXWqbPwW4VI");  //kontakt bot
            //new TelegramBotClient("245135166:AAEYEEsWjQmN_wLENwnA84Wb9xkgQJ-TLFE");   //my bot

        private static readonly long chat125Id = -219324188; //чат 125
        private static readonly long chatBotvaId = -1001100176543; //чат БотВажное
        private static readonly long chatUnasweredId = -1001469821060;

        private static DateTime lastIAmAliveCheckUtc = DateTime.UtcNow.Date;
        private static DateTime lastInactiveChatCheckUtc = DateTime.UtcNow.Date;

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
            ConfigureViewWaitersTimer();
            ConfigureViewInactiveChatsTimer();

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

            if (message == null)
                return;

            try
            {
                var recievedDataSet = new MessageDataSet(message);

                ProcessIfCommand(message);

                SaveToDatabase(recievedDataSet);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        Bot.SendTextMessageAsync(new ChatId(chatBotvaId), "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. mr\r\n"
                                                                          + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        Bot.SendTextMessageAsync(new ChatId(chat125Id), exception.ToString());
                        break;
                }
            }
        }

        private static void ProcessIfCommand(Message message)
        {
            if (message.Type == MessageType.Text)
                CommandMessageProcessor.ProcessTextMessage(Bot, message);
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
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(12).AddHours(-8); //9 часов по-нашему
                if (DateTime.UtcNow > scheduledRunUtc
                    && scheduledRunUtc.Date > lastIAmAliveCheckUtc.Date
                    && scheduledRunUtc.DayOfWeek == DayOfWeek.Saturday)
                {
                    ShowASign();
                    lastIAmAliveCheckUtc = DateTime.UtcNow;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        Bot.SendTextMessageAsync(new ChatId(chatBotvaId), "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. sas\r\n"
                                                                          + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        Bot.SendTextMessageAsync(new ChatId(chat125Id), exception.ToString());
                        break;
                }
            }
        }

        private static void ShowASign()
        {
            DateTime lastMessageDate;
            string lastMessageChat;
            
            using (var context = new MsgContext())
            {
                var lastMessage = context.MessageDataSets.OrderByDescending(message => message.Date).FirstOrDefault();
                lastMessageDate = lastMessage?.Date.AddHours(5) ?? DateTime.MinValue;
                lastMessageChat = lastMessage?.ChatName;
            }

            var signMessage = "Работаю в штатном режиме\r\nПоследнее сообщение от " + lastMessageDate.ToString("dd.MM.yyyy H:mm") + " в \"" + lastMessageChat + "\"";
            Bot.SendTextMessageAsync(new ChatId(chatUnasweredId), signMessage);
        }
        
        private static void ConfigureViewWaitersTimer()
        {
            var viewWaitersTimer = new Timer
            {
                Interval = 1000 * 60 * 5 //5 минут
            };
            viewWaitersTimer.Elapsed += ViewWaitersEvent;
            viewWaitersTimer.AutoReset = true;
            viewWaitersTimer.Start();
        }

        private static void ConfigureViewInactiveChatsTimer()
        {
            var timer = new Timer
            {
                Interval = 1000 * 60 * 5 //5 минут
            };
            timer.Elapsed += ViewInactiveChatsEvent;
            timer.AutoReset = true;
            timer.Start();
        }

        private static void ViewWaitersEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                var signalTime = e.SignalTime;

                if (signalTime.Hour >= 5 && signalTime.Hour < 18 && signalTime.DayOfWeek != DayOfWeek.Saturday && signalTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    var sinceDate = signalTime.Hour == 5 && signalTime.Minute < 5
                        ? DateTime.UtcNow.AddHours(signalTime.DayOfWeek != DayOfWeek.Monday
                            ? -11
                            : -59).AddMinutes(-125)
                        : DateTime.UtcNow.AddMinutes(-125);
                    var untilDate = DateTime.UtcNow.AddMinutes(-120);
                    var waitersReport = ViewWaitersProvider.GetWaitersFormatted(sinceDate, untilDate);
                    var botClientWrapper = new BotClientWrapper(Bot);
                    botClientWrapper.SendTextMessagesAsListAsync(chatUnasweredId, waitersReport).ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        Bot.SendTextMessageAsync(new ChatId(chatBotvaId), "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vw\r\n"
                                                                          + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        Bot.SendTextMessageAsync(new ChatId(chat125Id), exception.ToString());
                        break;
                }
            }
        }

        private static void ViewInactiveChatsEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(4); //9 часов по-нашему
                if (DateTime.UtcNow > scheduledRunUtc
                    && scheduledRunUtc.Date > lastInactiveChatCheckUtc.Date
                    && scheduledRunUtc.DayOfWeek == DayOfWeek.Sunday)
                {
                    var sinceDate = scheduledRunUtc.AddDays(-28);
                    var untilDate = scheduledRunUtc;
                    var inactiveChatsReport = ViewInactiveChatsProvider.GetInactiveFormatted(sinceDate, untilDate, TimeSpan.FromDays(7));
                    var botClientWrapper = new BotClientWrapper(Bot);
                    botClientWrapper.SendTextMessagesAsListAsync(chatUnasweredId, inactiveChatsReport).ConfigureAwait(false);
                    lastInactiveChatCheckUtc = scheduledRunUtc;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        Bot.SendTextMessageAsync(new ChatId(chatBotvaId), "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vic\r\n"
                                                                          + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        Bot.SendTextMessageAsync(new ChatId(chat125Id), exception.ToString());
                        break;
                }
            }
        }
    }
}