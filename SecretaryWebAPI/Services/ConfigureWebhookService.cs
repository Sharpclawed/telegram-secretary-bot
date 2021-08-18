using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecretaryWebAPI.Settings;
using Telegram.Bot;
using TelegramBotTry1;

namespace SecretaryWebAPI.Services
{
    public class ConfigureWebhookService : IHostedService
    {
        private readonly ILogger<ConfigureWebhookService> logger;
        private readonly IServiceProvider services;

        public ConfigureWebhookService(ILogger<ConfigureWebhookService> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            services = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var bot = scope.ServiceProvider.GetRequiredService<ISecretaryBot>();
            
            var webhookAddress = WebhookSettings.Url + "bot/" + WebhookSettings.BotToken;
            await using FileStream cert = File.OpenRead(WebhookSettings.PathToCert);
            logger.LogInformation("Setting webhook");
            await bot.ConfigWebhookAsync(webhookAddress, cert, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            logger.LogCritical("Failed to set webhook");
            logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken);
        }
    }
}
