using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    public class BotClientAdapter : TelegramBotClient, ITelegramBotClientAdapter
    {
        public BotClientAdapter(string token, HttpClient httpClient = null) : base(token, httpClient)
        {
        }

        public BotClientAdapter(string token, IWebProxy webProxy) : base(token, webProxy)
        {
        }

        public async Task SendTextMessagesAsListAsync(ChatId chatId, IList<string> msgs, ChatType destinationChatType)
        {
            var config = destinationChatType == ChatType.Personal
                ? defaultConfigPersonal
                : defaultConfigChat;

            var preparedSet = msgs.Take(config.TotalMessagesLimit);

            foreach (var msg in preparedSet)
                await SendTextMessageAsync(chatId, msg);
        }

        public async Task SendTextMessagesAsSingleTextAsync(ChatId chatId, IEnumerable<string> msgs, string caption)
        {
            var result = string.Join("\r\n", msgs);
            await SendTextMessageAsync(chatId, caption + result);
        }

        //todo maybe all these methods should take List<IMessageDataSet> and lambda or maybe fileInfoInstead
        public async Task SendTextMessagesAsExcelReportAsync(ChatId chatId, List<IMessageDataSet> msgs, string caption, string[] columnNames, Func<IMessageDataSet, string> groupBy = null)
        {
            //todo put to IMessageDataSetExtensions and add tests
            IEnumerable<KeyValuePair<string, List<IMessageDataSet>>> listsWithRows;
            if (groupBy == null)
                listsWithRows = new Dictionary<string, List<IMessageDataSet>>
                {
                    {caption, msgs}
                };
            else
            {
                listsWithRows = msgs
                    .GroupBy(groupBy)
                    .Select(gdc =>
                    {
                        var dataSets = gdc.OrderBy(z => z.Date).ToList();
                        return new KeyValuePair<string, List<IMessageDataSet>>(
                            groupBy(dataSets.LastOrDefault()) ?? "default",
                            dataSets);
                    });
            }

            using (var fileStream = ReportCreator.Create(listsWithRows, columnNames))
            {
                var fileToSend = new InputOnlineFile(fileStream, caption + ".xls");
                await SendDocumentAsync(chatId, fileToSend, caption);
            }
        }

        private readonly ListSenderConfig defaultConfigPersonal = new()
        {
            TotalMessagesLimit = 34
        };
        private readonly ListSenderConfig defaultConfigChat = new()
        {
            TotalMessagesLimit = 20
        };
    }

    public class ListSenderConfig
    {
        public int TotalMessagesLimit { get; set; }
    }

    public enum ChatType
    {
        Chat,
        Personal
    }

    public interface ITelegramBotClientAdapter : ITelegramBotClient
    {
        Task SendTextMessagesAsListAsync(ChatId chatId, IList<string> msgs, ChatType destinationChatType);
        Task SendTextMessagesAsSingleTextAsync(ChatId chatId, IEnumerable<string> msgs, string caption);
        Task SendTextMessagesAsExcelReportAsync(ChatId chatId, List<IMessageDataSet> msgs, string caption,
            string[] columnNames, Func<IMessageDataSet, string> groupBy = null);
    }
}