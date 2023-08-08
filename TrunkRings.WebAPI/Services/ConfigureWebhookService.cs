using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace TrunkRings.WebAPI.Services
{
    public class ConfigureWebhookService : IHostedService
    {
        private readonly ILogger<ConfigureWebhookService> logger;
        private readonly IConfiguration configuration;
        private readonly ISecretaryBot secretaryBot;

        public ConfigureWebhookService(ISecretaryBot secretaryBot, ILogger<ConfigureWebhookService> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.secretaryBot = secretaryBot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
#if DEBUG
            logger.LogInformation("Setting polling");
            secretaryBot.ConfigPolling(cancellationToken);
#else
            var url = configuration.GetValue<string>("TgBotSettings:Webhook:Url");
            var pathToCert = configuration.GetValue<string>("TgBotSettings:Webhook:PathToCert");
            var token = configuration.GetValue<string>("TgBotSettings:Token");
            var webhookAddress = $"{url}bot/{token}";
            await using FileStream cert = System.IO.File.OpenRead(pathToCert);
            var c = new InputFileStream(cert);
            logger.LogInformation("Setting webhook");
            await secretaryBot.ConfigWebhookAsync(webhookAddress, c, cancellationToken);
#endif
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //logger.LogInformation("Removing webhook");
            //await secretaryBot.DeleteWebhookAsync(cancellationToken);
        }
    }
}
