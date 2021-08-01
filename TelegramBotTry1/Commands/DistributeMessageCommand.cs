﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1.Commands
{
    public class DistributeMessageCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly IEnumerable<long> chatIds;
        private readonly string text;
        private readonly string caption;
        private readonly bool withMarkdown;
        
        public DistributeMessageCommand(ITgBotClientEx tgClient, IEnumerable<long> chatIds, string text, string caption = null, bool withMarkdown = false)
        {
            this.tgClient = tgClient;
            this.chatIds = chatIds;
            this.text = text;
            this.caption = caption ?? $"Рассылка сообщения выполнена\r\n{text}";
            this.withMarkdown = withMarkdown;
        }

        public async Task ProcessAsync()
        {
            var result = await SendTextMessagesAsync();
            var rows = result.Select(z => $"ChatId: {z.Item1} Result: {z.Item2}");
            //todo send as excel
            await tgClient.SendTextMessagesAsSingleTextAsync(ChatIds.LogDistributing, rows, caption);
        }

        private async Task<IEnumerable<(long, string)>> SendTextMessagesAsync()
        {
            var result = new List<(long, string)>();
            //todo avoid throttling
            foreach (var chatId in chatIds)
            {
                try
                {
                    await SendTextMessageAsync(chatId);
                    result.Add((chatId, "Success"));
                }
                catch (Exception e)
                {
                    result.Add((chatId, e.Message));
                }
            }

            return result;
        }

        private async Task SendTextMessageAsync(long chatId)
        {
            var permittedChats = new List<long> {ChatIds.Botva.Identifier, ChatIds.Debug.Identifier, ChatIds.LogDistributing.Identifier};
            if (!permittedChats.Contains(chatId))
                throw new Exception("Unallowed chat");

            await tgClient.SendTextMessageAsync(chatId, text, withMarkdown ? ParseMode.Markdown : ParseMode.Default);
        }
    }
}