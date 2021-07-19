using System.Threading.Tasks;
using DAL;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using TelegramBotTry1.Reporters;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1
{
    public interface ISecretaryBot
    {
        BotCommander BotCommander { get; }
        MessageProcessor MessageProcessor { get; }
        string Name { get; }
        void ConfigPolling();
        Task ConfigWebhookAsync(string url);
        void StartReceiving();
        void StartReporters();
    }

    public class SecretaryBot : ISecretaryBot
    {
        private ITgBotClientEx tgClient;
        private BotCommander botCommander;
        private MessageProcessor messageProcessor;
        private BotStateReporter botStateReporter;
        private WaitersReporter waitersViewReporter;
        private InactiveChatsReporter inactiveChatsReporter;
        private string name;

        public SecretaryBot(ITgBotClientEx tgClientEx)
        {
            tgClient = tgClientEx;
        }

        public async Task InitAsync()
        {
            await using (var context = new SecretaryContext())
            {
                await context.Database.MigrateAsync();
            }
            var adminService = new AdminService();
            var bkService = new BkService();
            var oneTimeChatService = new OneTimeChatService();
            var messageService = new MessageService();
            botCommander = new BotCommander(tgClient, messageService);
            messageProcessor = new MessageProcessor(tgClient, adminService, bkService, oneTimeChatService, messageService);
            botStateReporter = new BotStateReporter(botCommander);
            waitersViewReporter = new WaitersReporter(botCommander);
            inactiveChatsReporter = new InactiveChatsReporter(botCommander);
            name = (await tgClient.GetMeAsync()).Username;
        }

        public BotCommander BotCommander => botCommander;
        public MessageProcessor MessageProcessor => messageProcessor;
        public string Name => name;

        public void ConfigPolling()
        {
            tgClient.OnMessage += async (_, messageEventArgs) => await messageProcessor.ProcessTextMessageAsync(messageEventArgs.Message);
            tgClient.OnMessageEdited += async (_, messageEventArgs) => await messageProcessor.ProcessTextMessageAsync(messageEventArgs.Message);
            tgClient.OnReceiveError += async (_, receiveErrorEventArgs) =>
                await botCommander.SendMessageAsync(ChatIds.Test125, receiveErrorEventArgs.ApiRequestException.Message);
            tgClient.OnReceiveGeneralError += async (_, e) =>
                await botCommander.SendMessageAsync(ChatIds.Test125, e.Exception.Message + " \r\n" + e.Exception.InnerException);
            tgClient.OnCallbackQuery += async (_, e) => await botCommander.SendMessageAsync(ChatIds.Test125, e.CallbackQuery.Message.Text);
        }

        public async Task ConfigWebhookAsync(string url)
        {
            await tgClient.SetWebhookAsync(url).ConfigureAwait(false);
        }

        public void StartReceiving()
        {
            tgClient.StartReceiving();
        }

        public void StartReporters()
        {
            botStateReporter.Start();
            waitersViewReporter.Start();
            inactiveChatsReporter.Start();
        }
    }
}
