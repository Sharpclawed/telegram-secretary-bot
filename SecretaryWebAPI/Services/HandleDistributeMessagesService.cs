using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SecretaryWebAPI.Models;
using TelegramBotTry1;
using TelegramBotTry1.Settings;

namespace SecretaryWebAPI.Services
{
    public class HandleDistributeMessagesService
    {
        private readonly ISecretaryBot bot;
        private readonly ILogger<HandleDistributeMessagesService> logger;

        public HandleDistributeMessagesService(ISecretaryBot bot, ILogger<HandleDistributeMessagesService> logger)
        {
            this.bot = bot;
            this.logger = logger;
        }

        public async Task EchoAsync(DistributeMessages distributeMessages, string callerIp)
        {
            try
            {
                //var senderInfo = $"Рассылка сообщения\r\n{distributeMessages.Text}\r\nвыполнена с ip: {callerIp ?? "unknown"}";
                await bot.BotCommander.DistributeMessageAsync(distributeMessages.ChatIds, distributeMessages.Text);
            }
            catch (Exception exception)
            {
                logger.LogError(exception.ToString());
                await bot.BotCommander.SendMessageAsync(ChatIds.Debug, exception.ToString());
            }
        }
    }
}
