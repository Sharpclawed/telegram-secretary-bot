using System;
using System.Threading.Tasks;
using DAL;
using Domain.Services;
using Microsoft.EntityFrameworkCore;
using TelegramBotTry1.Reporters;
using TelegramBotTry1.SecretsStore;

namespace TelegramBotTry1
{
    static class Program
    {
        static async Task Main()
        {
            await using (var context = new SecretaryContext())
            {
                await context.Database.MigrateAsync();
            }

            var tgClient = new TgBotClientEx(Secrets.MainBotToken);
            var adminService = new AdminService();
            var bkService = new BkService();
            var oneTimeChatService = new OneTimeChatService();
            var messageService = new MessageService();
            var commandMessageProcessor = new CommandMessageProcessor(tgClient, adminService, bkService, oneTimeChatService, messageService);
            var botCommander = new BotCommander(tgClient, messageService);
            var botStateReporter = new BotStateReporter(botCommander);
            var waitersViewReporter = new WaitersReporter(botCommander);
            var inactiveChatsReporter = new InactiveChatsReporter(botCommander);

            tgClient.OnMessage += async (_, messageEventArgs) => await commandMessageProcessor.ProcessTextMessageAsync(messageEventArgs.Message);
            tgClient.OnMessageEdited += async (_, messageEventArgs) => await commandMessageProcessor.ProcessTextMessageAsync(messageEventArgs.Message);
            tgClient.OnReceiveError += async (_, receiveErrorEventArgs) =>
                await botCommander.SendMessageAsync(ChatIds.Test125, receiveErrorEventArgs.ApiRequestException.Message);
            tgClient.OnReceiveGeneralError += async (_, e) =>
                await botCommander.SendMessageAsync(ChatIds.Test125, e.Exception.Message + " \r\n" + e.Exception.InnerException);
            tgClient.OnCallbackQuery += async (_, e) => await botCommander.SendMessageAsync(ChatIds.Test125, e.CallbackQuery.Message.Text);

            Console.Title = (await tgClient.GetMeAsync()).Username;
            Console.WriteLine(DateTime.Now + " Start working");
            botStateReporter.Start();
            waitersViewReporter.Start();
            inactiveChatsReporter.Start();

            tgClient.StartReceiving();
            Console.ReadLine();
            tgClient.StopReceiving();
        }
    }
}