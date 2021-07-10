using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Services;
using Telegram.Bot.Types;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1
{
    //TODO take away configuring tgClient
    public class BotCommander
    {
        private readonly ITgBotClientEx tgClient;
        private readonly MessageService messageService;

        public BotCommander(ITgBotClientEx tgClient, MessageService messageService)
        {
            this.tgClient = tgClient;
            this.messageService = messageService;
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
            await new ViewWaitersCommand(messageService, tgClient, chatId, sinceDate, untilDate).ProcessAsync();
        }

        public async Task ViewInactiveChatsAsync(ChatId chatId, DateTime sinceDate, DateTime untilDate)
        {
            await new ViewInactiveChatsCommand(messageService, tgClient, chatId, sinceDate, untilDate).ProcessAsync();
        }

        public async Task SendBotStatusAsync(ChatId chatId)
        {
            await new SendBotStatusCommand(messageService, tgClient, chatId).ProcessAsync();
        }
    }
}