using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TrunkRings.Settings;

namespace TrunkRings.Commands
{
    class DistributeMessageCommand : IBotCommand<List<DistributeMessageResult>>
    {
        private readonly IMessageService messageService;
        private readonly ITgBotClientEx tgClient;
        private readonly IEnumerable<long> chatIds;
        private readonly string text;
        private readonly string caption;
        private readonly bool withMarkdown;
        
        public DistributeMessageCommand(IMessageService messageService, ITgBotClientEx tgClient, IEnumerable<long> chatIds, string text, string caption = null, bool withMarkdown = true)
        {
            this.messageService = messageService;
            this.tgClient = tgClient;
            this.chatIds = chatIds;
            this.text = text;
            this.caption = caption ?? $"Рассылка сообщения выполнена\r\n{text}";
            this.withMarkdown = withMarkdown;
        }

        public async Task<List<DistributeMessageResult>> ProcessAsync()
        {
            var result = await SendTextMessagesAsync();

            var chats = messageService.GetChatNames(result.Select(r => r.ChatId).ToArray());
            foreach (var distributingResult in result)
                distributingResult.ChatName = chats.TryGetValue(distributingResult.ChatId, out var chatName) ? chatName : "Чат не распознан";

            if (result.Count <= TgBotSettings.ReadableCountOfMessages)
            {
                var rows = result.Select(r => $"{r.ChatName} ({r.ChatId}) result: {r.Verdict} {r.ErrorMessage}");
                await tgClient.SendTextMessagesAsSingleTextAsync(ChatIds.LogDistributing, rows, caption, ParseMode.Html, true);
            }
            else
            {
                await tgClient.SendTextMessageAsync(ChatIds.LogDistributing, caption);
                await tgClient.SendTextMessagesAsExcelReportAsync(ChatIds.LogDistributing, result);
            }

            return result;
        }

        private async Task<List<DistributeMessageResult>> SendTextMessagesAsync()
        {
            var result = new List<DistributeMessageResult>();
            //todo avoid throttling
            foreach (var chatId in chatIds)
            {
                try
                {
                    await tgClient.SendTextMessageAsync(chatId, text, null, withMarkdown ? ParseMode.Markdown : ParseMode.Html, null, true);
                    result.Add(new DistributeMessageResult
                    {
                        ChatId = chatId,
                        Verdict = "Success"
                    });
                }
                catch (Exception e)
                {
                    result.Add(new DistributeMessageResult
                    {
                        ChatId = chatId,
                        Verdict = "Failed",
                        ErrorMessage = e.Message
                    });
                }
            }

            return result;
        }
    }

    public class DistributeMessageResult
    {
        public string ChatName { get; set; }
        public long ChatId { get; set; }
        public string Verdict { get; set; }
        public string ErrorMessage { get; set; }
    }
}