using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrunkRings.Settings;

namespace TrunkRings.Reporters
{
    class WaitersReporter : IReporter
    {
        private readonly BotCommander botCommander;
        private readonly ILogger logger;
        private PeriodicTimer timer;

        public WaitersReporter(BotCommander botCommander, ILogger logger)
        {
            this.botCommander = botCommander;
            this.logger = logger;
            timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("WaitersReporter started");
            while (await timer.WaitForNextTickAsync(cancellationToken))
                await ViewWaitersAsync();
        }

        private async Task ViewWaitersAsync()
        {
            try
            {
                var signalTime = DateTime.UtcNow;

                if (signalTime.Hour == 4 && signalTime.Minute < 5 && signalTime.DayOfWeek == DayOfWeek.Monday)
                {
                    var sinceDate = DateTime.UtcNow.AddHours(-61).AddMinutes(-125);
                    var untilDate = DateTime.UtcNow.AddMinutes(-120);
                    await botCommander.ViewWaitersAsync(ChatIds.Unanswered, sinceDate, untilDate);
                }
                else if (signalTime.Hour is >= 4 and < 15 && signalTime.DayOfWeek != DayOfWeek.Saturday && signalTime.DayOfWeek != DayOfWeek.Sunday)
                {
                    var sinceDate = signalTime.Hour == 4 && signalTime.Minute < 5
                        ? DateTime.UtcNow.AddHours(-13).AddMinutes(-125)
                        : DateTime.UtcNow.AddMinutes(-125);
                    var untilDate = DateTime.UtcNow.AddMinutes(-120);
                    await botCommander.ViewWaitersAsync(ChatIds.Unanswered, sinceDate, untilDate);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await botCommander.SendMessageAsync(ChatIds.Debug,
                            "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vw\r\n" + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await botCommander.SendMessageAsync(ChatIds.Debug, exception.ToString());
                        break;
                }
            }
        }
    }
}