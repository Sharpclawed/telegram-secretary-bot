using System;
using System.Net.Sockets;
using System.Timers;
using Telegram.Bot;
using TelegramBotTry1.DataProviders;

namespace TelegramBotTry1.Reporters
{
    public class BotStateReporter : IReporter
    {
        private readonly ITelegramBotClient botClient;
        private Timer timer;
        private DateTime lastIAmAliveCheckUtc = DateTime.UtcNow.Date;

        public BotStateReporter(ITelegramBotClient botClient)
        {
            this.botClient = botClient;
            Init();
        }

        private void Init()
        {
            timer = new Timer
            {
                Interval = 1000 * 60 * 60 * 0.5 //30 minutes
            };
            timer.Elapsed += ShowASign;
            timer.AutoReset = true;
        }

        public void Start()
        {
            timer.Start();
        }

        private async void ShowASign(object sender, ElapsedEventArgs e)
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(4); //9 часов по-нашему
                if (DateTime.UtcNow > scheduledRunUtc
                    && scheduledRunUtc.Date > lastIAmAliveCheckUtc.Date
                    && scheduledRunUtc.DayOfWeek == DayOfWeek.Saturday)
                {
                    var iAmAliveMessage = BotStatusProvider.GetIAmAliveMessage();
                    await botClient.SendTextMessageAsync(ChatIds.Unanswered, iAmAliveMessage);
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
                        await botClient.SendTextMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. sas\r\n"
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