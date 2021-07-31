﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1
{
    public class BotCommander
    {
        private readonly ITgBotClientEx tgClient;
        private readonly IMessageService messageService;

        public BotCommander(ITgBotClientEx tgClient, IMessageService messageService)
        {
            this.tgClient = tgClient;
            this.messageService = messageService;
        }

        public async Task SendBotStatusAsync(ChatId chatId)
        {
            await new SendBotStatusCommand(messageService, tgClient, chatId).ProcessAsync();
        }

        public async Task SendMessageAsync(ChatId chatId, string text)
        {
            await new SendMessageCommand(tgClient, chatId, text).ProcessAsync();
        }

        public async Task DistributeMessageAsync(IEnumerable<long> chatIds, string text, string caption = null)
        {
            await new DistributeMessageCommand(tgClient, chatIds, text, caption).ProcessAsync();
        }

        public async Task ViewInactiveChatsAsync(ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            await new ViewInactiveChatsCommand(messageService, tgClient, chatId, sinceDate, untilDate).ProcessAsync();
        }

        public async Task ViewWaitersAsync(ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            await new ViewWaitersCommand(messageService, tgClient, chatId, sinceDate, untilDate).ProcessAsync();
        }
    }
}