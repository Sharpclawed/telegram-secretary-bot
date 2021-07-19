using System;
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
    public class ConfigureWebhook : IHostedService
    {
        private readonly ILogger<ConfigureWebhook> logger;
        private readonly IServiceProvider services;
        private readonly IConfiguration configuration;

        public ConfigureWebhook(ILogger<ConfigureWebhook> logger,
                                IServiceProvider serviceProvider,
                                IConfiguration configuration)
        {
            this.logger = logger;
            services = serviceProvider;
            this.configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var bot = scope.ServiceProvider.GetRequiredService<ISecretaryBot>();
            
            var webhookAddress = WebhookSettings.Url + "bot/" + WebhookSettings.BotToken;
            logger.LogInformation("Setting webhook: ", webhookAddress);
            await bot.ConfigWebhookAsync(webhookAddress);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = services.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            // Remove webhook upon app shutdown
            logger.LogInformation("Removing webhook");
            await botClient.DeleteWebhookAsync(cancellationToken);
        }
    }
}
