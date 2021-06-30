﻿using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.DataProviders;

namespace TelegramBotTry1.Commands
{
    public class ViewOneTimeChatsCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public ViewOneTimeChatsCommand(ITgBotClientEx tgClient, ChatId chatId)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var result = IgnoreChatsListProvider.GetRows();

            if (result.Error != null)
            {
                await tgClient.SendTextMessageAsync(chatId, result.Error);
                return;
            }

            await tgClient.SendTextMessagesAsSingleTextAsync(chatId, result.Records, result.Caption);
        }
    }
}