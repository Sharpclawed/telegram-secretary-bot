using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TrunkRings.Settings;
using TrunkRings.WebAPI.Models;

namespace TrunkRings.WebAPI.Services
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
                logger.LogInformation("Recieved message distribution: {1}", distributeMessages.Text);
                logger.LogInformation("\tfor chats: {1}", string.Join(',', distributeMessages.ChatIds));
                logger.LogInformation("\tfrom: {1}", callerIp);
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
