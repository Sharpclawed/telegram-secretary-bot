using System;
using System.Threading.Tasks;
using SecretaryWebAPI.Models;
using TelegramBotTry1;
using TelegramBotTry1.Settings;

namespace SecretaryWebAPI.Services
{
    public class HandleDistributeMessagesService
    {
        private readonly ISecretaryBot bot;

        public HandleDistributeMessagesService(ISecretaryBot bot)
        {
            this.bot = bot;
        }

        public async Task EchoAsync(DistributeMessages distributeMessages, string callerIp)
        {
            try
            {
                //todo store commands
                //todo check sending
                //todo return result
                await bot.BotCommander.SendMessagesAsync(distributeMessages.ChatId, distributeMessages.Text);
            }
            catch (Exception exception)
            {
                await bot.BotCommander.SendMessageAsync(ChatIds.Debug, exception.ToString());
            }

        }
    }
}
