﻿using System;
using System.Globalization;
using System.Net.Sockets;
using System.Timers;
using Microsoft.Extensions.Logging;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1.Reporters
{
    public class InactiveChatsReporter : IReporter
    {
        private readonly BotCommander botCommander;
        private readonly ILogger logger;
        private Timer timer;
        private DateTime lastInactiveChatCheckUtc = DateTime.UtcNow.Date;

        public InactiveChatsReporter(BotCommander botCommander, ILogger logger)
        {
            this.botCommander = botCommander;
            this.logger = logger;
            Init();
        }

        private void Init()
        {
            timer = new Timer
            {
                Interval = 1000 * 60 * 5 //5 minutes
            };
            timer.Elapsed += ViewInactiveChatsAsync;
            timer.AutoReset = true;
        }

        public void Start()
        {
            timer.Start();
            logger.LogInformation("InactiveChatsReporter started");
        }

        private async void ViewInactiveChatsAsync(object sender, ElapsedEventArgs e)
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(4); //9 часов по-нашему
                logger.LogInformation("InactiveChatsReporter tick\r\nscheduledRunUtc:{1}\r\nDateTime.UtcNow:{2}\r\nlastIAmAliveCheckUtc.Date:{3}"
                    , scheduledRunUtc.ToString(CultureInfo.InvariantCulture)
                    , DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                    , lastInactiveChatCheckUtc.Date);
                if (DateTime.UtcNow > scheduledRunUtc
                    && scheduledRunUtc.Date > lastInactiveChatCheckUtc.Date
                    && scheduledRunUtc.DayOfWeek == DayOfWeek.Sunday)
                {
                    logger.LogInformation("InactiveChatsReporter actually works");
                    var sinceDate = scheduledRunUtc.AddDays(-28);
                    var untilDate = scheduledRunUtc;
                    await botCommander.ViewInactiveChatsAsync(ChatIds.Unanswered, sinceDate, untilDate);

                    lastInactiveChatCheckUtc = scheduledRunUtc;
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await botCommander.SendMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vic\r\n"
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