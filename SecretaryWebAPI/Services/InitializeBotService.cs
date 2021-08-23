using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelegramBotTry1;

namespace SecretaryWebAPI.Services
{
    public class InitializeBotService : IHostedService
    {
        private readonly ILogger<InitializeBotService> logger;
        private readonly ISecretaryBot secretaryBot;
        private readonly IConfiguration configuration;

        public InitializeBotService(ILogger<InitializeBotService> logger,
            ISecretaryBot secretaryBot,
            IConfiguration configuration)
        {
            this.logger = logger;
            this.secretaryBot = secretaryBot;
            this.configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Initializing Secretary bot");
                await secretaryBot.InitAsync();
                logger.LogInformation("Starting reporters");
                secretaryBot.StartReporters();
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogWarning("Bot shutdown");
        }
    }
}
