using System;
using System.Globalization;
using System.Net.Sockets;
using System.Timers;
using Microsoft.Extensions.Logging;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1.Reporters
{
    public class BotStateReporter : IReporter
    {
        private readonly BotCommander botCommander;
        private readonly ILogger logger;
        private Timer timer;
        private DateTime lastIAmAliveCheckUtc = DateTime.UtcNow.Date;

        public BotStateReporter(BotCommander botCommander, ILogger logger)
        {
            this.botCommander = botCommander;
            this.logger = logger;
            Init();
        }

        private void Init()
        {
            timer = new Timer
            {
                Interval = 1000 * 60 * 30 //30 minutes
            };
            timer.Elapsed += ShowASign;
            timer.AutoReset = true;
        }

        public void Start()
        {
            timer.Start();
            logger.LogInformation("BotStateReporter started");
        }

        private async void ShowASign(object sender, ElapsedEventArgs e)
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(4); //9 часов по-нашему
                logger.LogInformation("BotStateReporter tick\r\nscheduledRunUtc:{1}\r\nDateTime.UtcNow:{2}\r\nlastIAmAliveCheckUtc.Date:{3}"
                , scheduledRunUtc.ToString(CultureInfo.InvariantCulture)
                , DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                , lastIAmAliveCheckUtc.Date);
                if (DateTime.UtcNow > scheduledRunUtc
                    && scheduledRunUtc.Date > lastIAmAliveCheckUtc.Date
                    && (scheduledRunUtc.DayOfWeek == DayOfWeek.Saturday || scheduledRunUtc.DayOfWeek == DayOfWeek.Sunday))
                {
                    logger.LogInformation("BotStateReporter actually works");
                    await botCommander.SendBotStatusAsync(ChatIds.Unanswered);
                    lastIAmAliveCheckUtc = DateTime.UtcNow;
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await botCommander.SendMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. sas\r\n"
                                                                           + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await botCommander.SendMessageAsync(ChatIds.Debug, exception.ToString());
                        break;
                }
            }
        }
    }
}