using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TrunkRings.WebAPI.Services
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
                var secretaryBotConfig = new SecretaryBotConfig
                {
                    ConnectionString = configuration.GetConnectionString("SecretaryDatabase"),
                    DebugChatId = configuration.GetValue<long>("TgBotSettings:ChatIds:Debug"),
                    LogDistributingChatId = configuration.GetValue<long>("TgBotSettings:ChatIds:DistributedMessages"),
                    UnansweredChatId = configuration.GetValue<long>("TgBotSettings:ChatIds:UnasweredMessages")
                };
                await secretaryBot.InitAsync(secretaryBotConfig);
                logger.LogInformation("Starting reporters");
                await secretaryBot.StartReportersAsync(cancellationToken);
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
