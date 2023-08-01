using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrunkRings.Settings;

namespace TrunkRings.Reporters
{
    class InactiveChatsReporter : IReporter
    {
        private readonly BotCommander botCommander;
        private readonly ILogger logger;
        private PeriodicTimer timer;
        private DateTime lastInactiveChatCheckUtc = DateTime.UtcNow.Date;

        public InactiveChatsReporter(BotCommander botCommander, ILogger logger)
        {
            this.botCommander = botCommander;
            this.logger = logger;
            timer = new PeriodicTimer(TimeSpan.FromMinutes(30));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("InactiveChatsReporter started");
            while (await timer.WaitForNextTickAsync(cancellationToken))
                await ViewInactiveChatsAsync();
        }

        private async Task ViewInactiveChatsAsync()
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
                        await botCommander.SendMessageAsync(ChatIds.Debug, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. vic\r\n"
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