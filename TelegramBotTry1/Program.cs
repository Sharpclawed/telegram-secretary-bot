using System;
using Telegram.Bot.Args;
using TelegramBotTry1.Reporters;
using TelegramBotTry1.SecretsStore;

namespace TelegramBotTry1
{
    static class Program
    {
        private static readonly ITgBotClientEx TgClient = new TgBotClientEx(Secrets.MainBotToken);
        private static CommandMessageProcessor CommandMessageProcessor;

        static void Main()
        {
            TgClient.OnMessage += BotOnMessageReceived;
            TgClient.OnMessageEdited += BotOnMessageReceived;
            TgClient.OnReceiveError += BotOnReceiveError;
            TgClient.OnReceiveGeneralError += BotOnOnReceiveGeneralError;
            TgClient.OnCallbackQuery += BotOnOnCallbackQuery;

            Console.Title = TgClient.GetMeAsync().Result.Username;

            using (var context = new MsgContext())
            {
                context.Database.CreateIfNotExists();
            }
            CommandMessageProcessor = new CommandMessageProcessor(TgClient);
            var botStateReporter = new BotStateReporter(TgClient);
            var waitersViewReporter = new WaitersReporter(TgClient);
            var inactiveChatsReporter = new InactiveChatsReporter(TgClient);

            Console.WriteLine(DateTime.Now + " Start working");
            botStateReporter.Start();
            waitersViewReporter.Start();
            inactiveChatsReporter.Start();

            TgClient.StartReceiving();
            Console.ReadLine();
            TgClient.StopReceiving();
        }

        private static async void BotOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            await TgClient.SendTextMessageAsync(ChatIds.Test125, e.CallbackQuery.Message.Text);
        }

        private static async void BotOnOnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            await TgClient.SendTextMessageAsync(ChatIds.Test125, e.Exception.Message + " \r\n" + e.Exception.InnerException);
        }

        private static async void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            await TgClient.SendTextMessageAsync(ChatIds.Test125, receiveErrorEventArgs.ApiRequestException.Message);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            await CommandMessageProcessor.ProcessTextMessageAsync(message);
        }
    }
}