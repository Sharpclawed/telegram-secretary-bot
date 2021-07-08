﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DAL.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

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

        //todo maybe all these methods should take List<IMessageDataSet> and lambda or maybe fileInfo instead
        public async Task SendTextMessagesAsExcelReportAsync(ChatId chatId, List<MessageDataSet> msgs, string caption, string[] columnNames, Func<MessageDataSet, string> groupBy = null)
        {
            //todo put to IMessageDataSetExtensions and add tests. But there's a problem - order and set of columns
            IEnumerable<KeyValuePair<string, List<MessageDataSet>>> listsWithRows;
            if (groupBy == null)
                listsWithRows = new Dictionary<string, List<MessageDataSet>>
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
                        return new KeyValuePair<string, List<MessageDataSet>>(
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
        Task SendTextMessagesAsExcelReportAsync(ChatId chatId, List<MessageDataSet> msgs, string caption,
            string[] columnNames, Func<MessageDataSet, string> groupBy = null);
    }
}