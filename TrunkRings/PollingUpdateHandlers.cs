using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrunkRings.Settings;


namespace TrunkRings
{
    internal class PollingUpdateHandlers
    {
        private BotCommander botCommander;
        private MessageProcessor messageProcessor;

        public PollingUpdateHandlers(BotCommander botCommander, MessageProcessor messageProcessor)
        {
            this.botCommander = botCommander;
            this.messageProcessor = messageProcessor;
        }

        public async Task ErrorHandlerAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            await botCommander.SendMessageAsync(ChatIds.Debug, errorMessage);
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => messageProcessor.ProcessMessageAsync(update.Message),
                UpdateType.EditedMessage => messageProcessor.ProcessMessageAsync(update.EditedMessage),
                _ => Task.CompletedTask
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await ErrorHandlerAsync(botClient, exception, cancellationToken);
            }
        }
    }
}
