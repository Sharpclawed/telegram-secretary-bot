using System.Threading.Tasks;
using DAL;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using TelegramBotTry1.Reporters;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1
{
    public class SecretaryBot
    {
        private static readonly string mainBotToken = Secrets.MainBotToken;

        private static readonly SecretaryBot instance = new ();
        private static BotCommander botCommander;
        private static ITgBotClientEx tgClient;
        private static MessageProcessor messageProcessor;
        private static BotStateReporter botStateReporter;
        private static WaitersReporter waitersViewReporter;
        private static InactiveChatsReporter inactiveChatsReporter;
        private static string name;
        private static bool initialized;

        private SecretaryBot()
        {
        }

        public static async Task<SecretaryBot> GetAsync()
        {
            if (!initialized)
                await InitAsync();

            return instance;
        }

        private static async Task InitAsync()
        {
            await using (var context = new SecretaryContext())
            {
                await context.Database.MigrateAsync();
            }
            var adminService = new AdminService();
            var bkService = new BkService();
            var oneTimeChatService = new OneTimeChatService();
            var messageService = new MessageService();
            tgClient = new TgBotClientEx(mainBotToken);
            botCommander = new BotCommander(tgClient, messageService);
            messageProcessor = new MessageProcessor(tgClient, adminService, bkService, oneTimeChatService, messageService);
            botStateReporter = new BotStateReporter(botCommander);
            waitersViewReporter = new WaitersReporter(botCommander);
            inactiveChatsReporter = new InactiveChatsReporter(botCommander);
            name = (await tgClient.GetMeAsync()).Username;
            initialized = true;
        }

        public BotCommander BotCommander => botCommander;
        public MessageProcessor MessageProcessor => messageProcessor;
        public string Name => name;

        public void SetPolling()
        {
            tgClient.OnMessage += async (_, messageEventArgs) => await messageProcessor.ProcessTextMessageAsync(messageEventArgs.Message);
            tgClient.OnMessageEdited += async (_, messageEventArgs) => await messageProcessor.ProcessTextMessageAsync(messageEventArgs.Message);
            tgClient.OnReceiveError += async (_, receiveErrorEventArgs) =>
                await botCommander.SendMessageAsync(ChatIds.Test125, receiveErrorEventArgs.ApiRequestException.Message);
            tgClient.OnReceiveGeneralError += async (_, e) =>
                await botCommander.SendMessageAsync(ChatIds.Test125, e.Exception.Message + " \r\n" + e.Exception.InnerException);
            tgClient.OnCallbackQuery += async (_, e) => await botCommander.SendMessageAsync(ChatIds.Test125, e.CallbackQuery.Message.Text);
        }

        public void StartPolling()
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
