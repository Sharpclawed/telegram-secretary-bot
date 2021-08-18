using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TelegramBotTry1;

namespace SecretaryWebAPI.Services
{
    public class InitializeBotService : IHostedService
    {
        private readonly ILogger<InitializeBotService> logger;
        private readonly IServiceProvider services;
        private readonly IConfiguration configuration;

        public InitializeBotService(ILogger<InitializeBotService> logger,
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
            logger.LogInformation("Initializing Secretary bot");
            await bot.InitAsync();
            logger.LogInformation("Starting reporters");
            bot.StartReporters();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogCritical("Failed to init bot");
        }
    }
}
