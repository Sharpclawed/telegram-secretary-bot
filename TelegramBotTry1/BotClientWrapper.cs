using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramBotTry1
{
    //todo tests
    //todo extension implementation?
    public class BotClientWrapper
    {
        private readonly ITelegramBotClient client;

        private readonly SenderConfig defaultConfig = new SenderConfig
        {
            TotalMessagesLimit = 34,
            MessagesPerPackage = 17,
            IntervalBetweenPackages = TimeSpan.FromSeconds(5)
        };

        public BotClientWrapper(ITelegramBotClient client)
        {
            this.client = client;
        }

        public async Task SendTextMessagesAsListAsync(long chatId, IList<string> msgs, SenderConfig config = null)
        {
            config = config ?? defaultConfig;

            var preparedSet = msgs.Count <= config.TotalMessagesLimit
                ? msgs
                : msgs.Take(config.TotalMessagesLimit).ToList();

            for (var i = 1; i <= preparedSet.Count; i++)
            {
                //todo is Thread.Sleep bad in this case?
                if (i % config.MessagesPerPackage == 0)
                    Thread.Sleep((int) config.IntervalBetweenPackages.TotalSeconds);

                await client.SendTextMessageAsync(chatId, preparedSet[i]);
            }
        }

        public async Task SendTextMessagesAsSingleTextAsync(long chatId, IEnumerable<string> msgs, string caption)
        {
            var result = string.Join("\r\n", msgs);
            await client.SendTextMessageAsync(chatId, caption + result);
        }
    }

    public class SenderConfig
    {
        public int MessagesPerPackage { get; set; }
        public int TotalMessagesLimit { get; set; }
        public TimeSpan IntervalBetweenPackages { get; set; }
    }
}