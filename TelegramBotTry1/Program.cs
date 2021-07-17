using System;
using System.Threading.Tasks;

namespace TelegramBotTry1
{
    static class Program
    {
        static async Task Main()
        {
            var bot = await SecretaryBot.GetAsync();
            bot.SetPolling();
            bot.StartPolling();
            bot.StartReporters();
            Console.Title = bot.Name;
            Console.WriteLine(DateTime.Now + " Start working");

            Console.ReadLine();
        }
    }
}