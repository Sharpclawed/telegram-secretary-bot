using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DAL;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TelegramBotTry1.Reporters;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1
{
    public interface ISecretaryBot
    {
        Task InitAsync();
        BotCommander BotCommander { get; }
        MessageProcessor MessageProcessor { get; }
        string Name { get; }
        void ConfigPolling();
        Task ConfigWebhookAsync(string url, InputFileStream cert = null, CancellationToken cancellationToken = default);
        Task DeleteWebhookAsync(CancellationToken cancellationToken = default);
        void StartReceiving();
        void StartReporters();
    }

    public class SecretaryBot : ISecretaryBot
    {
        private ITgBotClientEx tgClient;
        private readonly ILogger logger;
        private BotCommander botCommander;
        private MessageProcessor messageProcessor;
        private BotStateReporter botStateReporter;
        private WaitersReporter waitersViewReporter;
        private InactiveChatsReporter inactiveChatsReporter;
        private string name;

        public SecretaryBot(ITgBotClientEx tgClientEx, ILogger logger)
        {
            tgClient = tgClientEx;
            this.logger = logger;
        }

        public async Task InitAsync()
        {
            logger.Log(LogLevel.Information, "Initialization start");
            await using (var context = new SecretaryContext())
            {
                await context.Database.MigrateAsync();
            }
            logger.Log(LogLevel.Information, "Db migration finished");
            var adminService = new AdminService();
            var bkService = new BkService();
            var oneTimeChatService = new OneTimeChatService();
            var messageService = new MessageService();
            botCommander = new BotCommander(tgClient, messageService);
            messageProcessor = new MessageProcessor(tgClient, adminService, bkService, oneTimeChatService, messageService);
            botStateReporter = new BotStateReporter(botCommander, logger);
            waitersViewReporter = new WaitersReporter(botCommander);
            inactiveChatsReporter = new InactiveChatsReporter(botCommander, logger);
            name = (await tgClient.GetMeAsync()).Username;
            logger.Log(LogLevel.Information, "Init's completed");
        }

        public BotCommander BotCommander => botCommander;
        public MessageProcessor MessageProcessor => messageProcessor;
        public string Name => name;

        public void ConfigPolling()
        {
            tgClient.OnMessage += async (_, messageEventArgs) => await messageProcessor.ProcessMessageAsync(messageEventArgs.Message);
            tgClient.OnMessageEdited += async (_, messageEventArgs) => await messageProcessor.ProcessMessageAsync(messageEventArgs.Message);
            tgClient.OnReceiveError += async (_, receiveErrorEventArgs) =>
                await botCommander.SendMessageAsync(ChatIds.Debug, receiveErrorEventArgs.ApiRequestException.Message);
            tgClient.OnReceiveGeneralError += async (_, e) =>
                await botCommander.SendMessageAsync(ChatIds.Debug, e.Exception.Message + " \r\n" + e.Exception.InnerException);
            tgClient.OnCallbackQuery += async (_, e) => await botCommander.SendMessageAsync(ChatIds.Debug, e.CallbackQuery.Message.Text);
        }

        public async Task ConfigWebhookAsync(string url, InputFileStream cert = null, CancellationToken cancellationToken = default)
        {
            await tgClient.SetWebhookAsync(url, cert, cancellationToken: cancellationToken, allowedUpdates: new List<UpdateType> {UpdateType.Message});
        }

        public async Task DeleteWebhookAsync(CancellationToken cancellationToken = default)
        {
            await tgClient.DeleteWebhookAsync(cancellationToken);
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
