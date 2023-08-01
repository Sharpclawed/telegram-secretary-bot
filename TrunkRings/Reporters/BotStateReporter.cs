using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrunkRings.Settings;

namespace TrunkRings.Reporters
{
    class BotStateReporter : IReporter
    {
        private readonly BotCommander botCommander;
        private readonly ILogger logger;
        private PeriodicTimer timer;
        private DateTime lastIAmAliveCheckUtc = DateTime.UtcNow.Date;

        public BotStateReporter(BotCommander botCommander, ILogger logger)
        {
            this.botCommander = botCommander;
            this.logger = logger;
            timer = new PeriodicTimer(TimeSpan.FromMinutes(30));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("BotStateReporter started");
            while (await timer.WaitForNextTickAsync(cancellationToken))
                await ReportAsync();
        }

        private async Task ReportAsync()
        {
            try
            {
                var scheduledRunUtc = DateTime.UtcNow.Date.AddHours(4); //9 часов по-нашему
                if (DateTime.UtcNow > scheduledRunUtc
                    && scheduledRunUtc.Date > lastIAmAliveCheckUtc.Date
                    && scheduledRunUtc.DayOfWeek == DayOfWeek.Saturday)
                {
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
                        await botCommander.SendMessageAsync(ChatIds.Debug, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. sas\r\n"
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