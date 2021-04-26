using System;
using System.Linq;
using System.Net.Sockets;
using System.Timers;
using Telegram.Bot;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1.Reporters
{
    public class WaitersReporter : IReporter
    {
        private readonly ITelegramBotClient botClient;
        private readonly BotClientWrapper botClientWrapper;
        private Timer viewWaitersTimer;

        public WaitersReporter(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
            botClientWrapper = new BotClientWrapper(botClient);
            Init();
        }

        private void Init()
        {
            viewWaitersTimer = new Timer
            {
                Interval = 1000 * 60 * 5 //5 minutes
            };
            viewWaitersTimer.Elapsed += ViewWaitersAsync;
            viewWaitersTimer.AutoReset = true;
        }

        public void Start()
        {
            viewWaitersTimer.Start();
        }

        private async void ViewWaitersAsync(object sender, ElapsedEventArgs e)
        {
            try
            {
                var signalTime = e.SignalTime;

                if (signalTime.Hour == 7 && signalTime.Minute < 5 && signalTime.DayOfWeek == DayOfWeek.Monday)
                {
                    var sinceDate = DateTime.UtcNow.AddHours(-61).AddMinutes(-125);
                    var untilDate = DateTime.UtcNow.AddMinutes(-120);
                    var waitersReport = CommandProcessor.ProcessViewWaiters(sinceDate, untilDate);

                    await botClientWrapper.SendTextMessagesAsExcelReportAsync(
                        ChatIds.Unanswered,
                        waitersReport.Records,
                        waitersReport.Caption,
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
                }
                else if (signalTime.Hour >= 7 && signalTime.Hour < 18 && signalTime.DayOfWeek != DayOfWeek.Saturday && signalTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    var sinceDate = signalTime.Hour == 7 && signalTime.Minute < 5
                        ? DateTime.UtcNow.AddHours(-13).AddMinutes(-125)
                        : DateTime.UtcNow.AddMinutes(-125);
                    var untilDate = DateTime.UtcNow.AddMinutes(-120);
                    var waitersReport = CommandProcessor.ProcessViewWaiters(sinceDate, untilDate);
                    var formattedRecords = waitersReport.Records.Select(Formatter.Waiters).ToList();
                    await botClientWrapper.SendTextMessagesAsListAsync(ChatIds.Unanswered, formattedRecords, ChatType.Chat);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await botClient.SendTextMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vw\r\n"
                                                                                + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await botClient.SendTextMessageAsync(ChatIds.Test125, exception.ToString());
                        break;
                }
            }
        }
    }
}