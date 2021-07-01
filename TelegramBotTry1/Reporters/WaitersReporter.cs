using System;
using System.Net.Sockets;
using System.Timers;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1.Reporters
{
    public class WaitersReporter : IReporter
    {
        private readonly ITgBotClientEx tgClient;
        private readonly BotCommander botCommander;
        private Timer viewWaitersTimer;

        public WaitersReporter(ITgBotClientEx tgClient, BotCommander botCommander)
        {
            this.tgClient = tgClient;
            this.botCommander = botCommander;
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
                    var command = new ViewWaitersCommand(tgClient, ChatIds.Unanswered, sinceDate, untilDate);
                    await command.Process2Async();
                }
                else if (signalTime.Hour >= 7 && signalTime.Hour < 18 && signalTime.DayOfWeek != DayOfWeek.Saturday && signalTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    var sinceDate = signalTime.Hour == 7 && signalTime.Minute < 5
                        ? DateTime.UtcNow.AddHours(-13).AddMinutes(-125)
                        : DateTime.UtcNow.AddMinutes(-125);
                    var untilDate = DateTime.UtcNow.AddMinutes(-120);
                    var command = new ViewWaitersCommand(tgClient, ChatIds.Unanswered, sinceDate, untilDate);
                    await command.ProcessAsync();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await botCommander.SendMessageAsync(ChatIds.Botva,
                            "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vw\r\n" + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await botCommander.SendMessageAsync(ChatIds.Test125, exception.ToString());
                        break;
                }
            }
        }
    }
}