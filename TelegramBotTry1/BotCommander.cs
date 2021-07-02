﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1
{
    //TODO take away configuring tgClient
    public class BotCommander
    {
        private readonly ITgBotClientEx tgClient;

        public BotCommander(ITgBotClientEx tgClient)
        {
            this.tgClient = tgClient;
        }

        public async Task SendMessageAsync(ChatId chatId, string text)
        {
            await new SendMessageCommand(tgClient, chatId, text).ProcessAsync();
        }

        public async Task SendMessagesAsync(IEnumerable<ChatId> chatIds, string text)
        {
            await new DistributeMessageCommand(tgClient, chatIds, text).ProcessAsync();
        }

        public async Task ViewWaitersAsync(ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            await new ViewWaitersCommand(tgClient, chatId, sinceDate, untilDate).ProcessAsync();
        }

        public async Task ViewInactiveChatsAsync(ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            await new ViewInactiveChatsCommand(tgClient, chatId, sinceDate, untilDate).ProcessAsync();
        }

        public async Task SendBotStatusAsync(ChatId chatId)
        {
            await new SendBotStatusCommand(tgClient, chatId).ProcessAsync();
        }
    }
}