﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TrunkRings.Settings;

namespace TrunkRings.WebAPI.Services
{
    public class HandleUpdateService
    {
        private readonly ISecretaryBot bot;
        private readonly ILogger<HandleUpdateService> logger;

        public HandleUpdateService(ISecretaryBot bot, ILogger<HandleUpdateService> logger)
        {
            this.bot = bot;
            this.logger = logger;
        }

        public async Task EchoAsync(Update update)
        {
            var handler = update.Type switch
            {
                UpdateType.Message => BotOnMessageReceived(update.Message),
                UpdateType.EditedMessage => BotOnMessageReceived(update.EditedMessage),
                _ => Task.CompletedTask
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }
        }

        private async Task BotOnMessageReceived(Message message)
        {
            await bot.MessageProcessor.ProcessMessageAsync(message);
        }

        public async Task HandleErrorAsync(Exception exception)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            logger.LogError(errorMessage);
            await bot.BotCommander.SendMessageAsync(ChatIds.Debug, errorMessage);
        }
    }
}
