using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TrunkRings.Domain;
using TrunkRings.Reporters;
using TrunkRings.Settings;

[assembly: InternalsVisibleTo("TrunkRings.UnitTests")]

namespace TrunkRings
{
    public interface ISecretaryBot
    {
        Task InitAsync(ISecretaryBotConfig botConfig);
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

        public async Task InitAsync(ISecretaryBotConfig botConfig)
        {
            logger.Log(LogLevel.Information, "Initialization start");
            var domainServices = new DomainServices(botConfig.ConnectionString);
            logger.Log(LogLevel.Information, "Db migration finished");
            SetChatIds(botConfig);
            var adminService = domainServices.GetAdminService();
            var bkService = domainServices.GetBookkeeperService();
            var oneTimeChatService = domainServices.GetOneTimeChatService();
            var messageService = domainServices.GetMessageService();
            botCommander = new BotCommander(tgClient, messageService);
            messageProcessor = new MessageProcessor(tgClient, adminService, bkService, oneTimeChatService, messageService, logger);
            botStateReporter = new BotStateReporter(botCommander, logger);
            waitersViewReporter = new WaitersReporter(botCommander, logger);
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

        private void SetChatIds(ISecretaryBotConfig botConfig)
        {
            ChatIds.Debug = botConfig.DebugChatId;
            ChatIds.LogDistributing = botConfig.LogDistributingChatId;
            ChatIds.Unanswered = botConfig.UnansweredChatId;
            ChatIds.AllowedForDistribution = botConfig.AllowedForDistributionChatIds;
        }
    }
}
