using System;
using DAL;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Args;
using TelegramBotTry1.Reporters;
using TelegramBotTry1.SecretsStore;

namespace TelegramBotTry1
{
    static class Program
    {
        private static readonly ITgBotClientEx tgClient = new TgBotClientEx(Secrets.MainBotToken);
        private static CommandMessageProcessor commandMessageProcessor;
        private static BotCommander botCommander;

        static void Main()
        {
            tgClient.OnMessage += BotOnMessageReceived;
            tgClient.OnMessageEdited += BotOnMessageReceived;
            tgClient.OnReceiveError += BotOnReceiveError;
            tgClient.OnReceiveGeneralError += BotOnOnReceiveGeneralError;
            tgClient.OnCallbackQuery += BotOnOnCallbackQuery;

            Console.Title = tgClient.GetMeAsync().Result.Username;

            using (var context = new SecretaryContext())
            {
                context.Database.Migrate();
            }

            var adminService = new AdminService();
            var bkService = new BkService();
            var oneTimeChatService = new OneTimeChatService();
            var messageService = new MessageService();
            commandMessageProcessor = new CommandMessageProcessor(tgClient, adminService, bkService, oneTimeChatService, messageService);
            botCommander = new BotCommander(tgClient);
            var botStateReporter = new BotStateReporter(botCommander);
            var waitersViewReporter = new WaitersReporter(botCommander);
            var inactiveChatsReporter = new InactiveChatsReporter(botCommander);

            Console.WriteLine(DateTime.Now + " Start working");
            botStateReporter.Start();
            waitersViewReporter.Start();
            inactiveChatsReporter.Start();

            tgClient.StartReceiving();
            Console.ReadLine();
            tgClient.StopReceiving();
        }

        private static async void BotOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            await botCommander.SendMessageAsync(ChatIds.Test125, e.CallbackQuery.Message.Text);
        }

        private static async void BotOnOnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            await botCommander.SendMessageAsync(ChatIds.Test125, e.Exception.Message + " \r\n" + e.Exception.InnerException);
        }

        private static async void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            await botCommander.SendMessageAsync(ChatIds.Test125, receiveErrorEventArgs.ApiRequestException.Message);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            await commandMessageProcessor.ProcessTextMessageAsync(message);
        }
    }
}