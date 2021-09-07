using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrunkRings;
using TrunkRings.WebAPI.Settings;

namespace TrunkRings.WebAPI.Services
{
    public class ConfigureWebhookService : IHostedService
    {
        private readonly ILogger<ConfigureWebhookService> logger;
        private readonly ISecretaryBot secretaryBot;

        public ConfigureWebhookService(ILogger<ConfigureWebhookService> logger, ISecretaryBot secretaryBot)
        {
            this.logger = logger;
            this.secretaryBot = secretaryBot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var webhookAddress = $"{WebhookSettings.Url}bot/{WebhookSettings.BotToken}";
            await using FileStream cert = File.OpenRead(WebhookSettings.PathToCert);
            logger.LogInformation("Setting webhook");
            await secretaryBot.ConfigWebhookAsync(webhookAddress, cert, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            //logger.LogInformation("Removing webhook");
            //await secretaryBot.DeleteWebhookAsync(cancellationToken);
        }
    }
}
