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
                //var senderInfo = $"Рассылка сообщения\r\n{distributeMessages.Text}\r\nвыполнена с ip: {callerIp ?? "unknown"}";
                await bot.BotCommander.DistributeMessageAsync(distributeMessages.ChatIds, distributeMessages.Text);
            }
            catch (Exception exception)
            {
                await bot.BotCommander.SendMessageAsync(ChatIds.Debug, exception.ToString());
            }
        }
    }
}
