﻿using System;
using System.Threading.Tasks;
using TelegramBotTry1.Settings;

namespace TelegramBotTry1
{
    static class Program
    {
        static async Task Main()
        {
            var tgClient = new TgBotClientEx(Secrets.TgBotToken);
            var bot = new SecretaryBot(tgClient, null);
            await bot.InitAsync();
            bot.ConfigPolling();
            bot.StartReceiving();
            bot.StartReporters();
            Console.Title = bot.Name;
            Console.WriteLine(DateTime.Now + " Start working");

            Console.ReadLine();
        }
    }
}