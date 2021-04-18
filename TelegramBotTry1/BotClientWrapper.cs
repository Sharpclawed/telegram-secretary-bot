﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1
{
    //todo tests
    //todo extension implementation? adapter implementation?
    public class BotClientWrapper
    {
        private readonly ITelegramBotClient client;
        private readonly ListSenderConfig defaultConfigPersonal = new ListSenderConfig
        {
            TotalMessagesLimit = 34,
            MessagesPerPackage = 17,
            IntervalBetweenPackages = TimeSpan.FromSeconds(5)
        };
        private readonly ListSenderConfig defaultConfigChat = new ListSenderConfig
        {
            TotalMessagesLimit = 20,
            MessagesPerPackage = 4,
            IntervalBetweenPackages = TimeSpan.FromSeconds(3)
        };

        public BotClientWrapper(ITelegramBotClient client)
        {
            this.client = client;
        }

        public async Task SendTextMessagesAsListAsync(ChatId chatId, IList<string> msgs, ChatType destinationChatType)
        {
            var config = destinationChatType == ChatType.Personal
                ? defaultConfigPersonal
                : defaultConfigChat;

            var preparedSet = msgs.Count <= config.TotalMessagesLimit
                ? msgs
                : msgs.Take(config.TotalMessagesLimit);

            var i = 0;
            foreach (var msg in preparedSet)
            {
                i++;
                //todo is Thread.Sleep bad in this case?
                if (i % config.MessagesPerPackage == 0)
                    Thread.Sleep((int)config.IntervalBetweenPackages.TotalMilliseconds);

                await client.SendTextMessageAsync(chatId, msg);
            }
        }

        public async Task SendTextMessagesAsSingleTextAsync(ChatId chatId, IEnumerable<string> msgs, string caption)
        {
            var result = string.Join("\r\n", msgs);
            await client.SendTextMessageAsync(chatId, caption + result);
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

            var report = ReportCreator.Create(listsWithRows, "temp" + chatId.Identifier, columnNames);
            using (var fileStream = new FileStream(report.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var fileToSend = new InputOnlineFile(fileStream, caption + ".xls");
                await client.SendDocumentAsync(chatId, fileToSend, caption);
            }
        }
    }

    public class ListSenderConfig
    {
        public int MessagesPerPackage { get; set; }
        public int TotalMessagesLimit { get; set; }
        public TimeSpan IntervalBetweenPackages { get; set; }
    }

    public enum ChatType
    {
        Chat,
        Personal
    }
}