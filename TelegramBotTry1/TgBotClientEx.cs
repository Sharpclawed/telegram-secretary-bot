using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1
{
    public class TgBotClientEx : TelegramBotClient, ITgBotClientEx
    {
        public TgBotClientEx(string token, HttpClient httpClient = null) : base(token, httpClient)
        {
        }

        public TgBotClientEx(string token, IWebProxy webProxy) : base(token, webProxy)
        {
        }

        public async Task SendTextMessagesAsListAsync(ChatId chatId, IList<string> msgs, СorrespondenceType сorrespondenceType)
        {
            var totalMessagesLimit = сorrespondenceType == СorrespondenceType.Personal 
                ? TgBotSettings.SendLimitForAllChatsPerSecond
                : TgBotSettings.SendLimitForOneChatPerMinute;
            var preparedSet = msgs.Take(totalMessagesLimit);

            foreach (var msg in preparedSet)
                await SendTextMessageAsync(chatId, msg);
        }

        public async Task SendTextMessagesAsSingleTextAsync(ChatId chatId, IEnumerable<string> msgs, string caption)
        {
            var result = string.Join("\r\n", msgs);
            await SendTextMessageAsync(chatId, caption + result);
        }
        
        public async Task SendTextMessagesAsExcelReportAsync(ChatId chatId, List<DomainMessage> msgs, string caption, string[] columnNames, Func<DomainMessage, string> groupBy = null)
        {
            //todo add tests
            IEnumerable<KeyValuePair<string, List<DomainMessage>>> listsWithRows;
            if (groupBy == null)
                listsWithRows = new Dictionary<string, List<DomainMessage>>
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
                        return new KeyValuePair<string, List<DomainMessage>>(
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
    }

    public enum СorrespondenceType
    {
        Chat,
        Personal
    }

    public interface ITgBotClientEx : ITelegramBotClient
    {
        Task SendTextMessagesAsListAsync(ChatId chatId, IList<string> msgs, СorrespondenceType сorrespondenceType);
        Task SendTextMessagesAsSingleTextAsync(ChatId chatId, IEnumerable<string> msgs, string caption);
        Task SendTextMessagesAsExcelReportAsync(ChatId chatId, List<DomainMessage> msgs, string caption,
            string[] columnNames, Func<DomainMessage, string> groupBy = null);
    }
}