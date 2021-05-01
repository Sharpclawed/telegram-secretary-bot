using System;
using System.Linq;
using System.Net.Sockets;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Reporters;

namespace TelegramBotTry1
{
    static class Program
    {
        //todo hide and change tokens
        private static readonly ITelegramBotClientAdapter BotClient =
            new BotClientAdapter("361040811:AAGQlsM84JwDIRtcztbMMboKLXWqbPwW4VI");  //kontakt bot
          //new BotClientAdapter("245135166:AAEYEEsWjQmN_wLENwnA84Wb9xkgQJ-TLFE");   //my bot

        static void Main()
        {
            BotClient.OnMessage += BotOnMessageReceived;
            BotClient.OnMessageEdited += BotOnMessageReceived;
            BotClient.OnReceiveError += BotOnReceiveError;
            BotClient.OnReceiveGeneralError += BotOnOnReceiveGeneralError;
            BotClient.OnCallbackQuery += BotOnOnCallbackQuery;

            Console.Title = "Secretary bot " + BotClient.BotId;

            using (var context = new MsgContext())
            {
                context.Database.CreateIfNotExists();
            }
            var botStateReporter = new BotStateReporter(BotClient);
            var waitersViewReporter = new WaitersReporter(BotClient);
            var inactiveChatsReporter = new InactiveChatsReporter(BotClient);

            Console.WriteLine(DateTime.Now + " Start working");
            botStateReporter.Start();
            waitersViewReporter.Start();
            inactiveChatsReporter.Start();

            BotClient.StartReceiving();
            Console.ReadLine();
            BotClient.StopReceiving();
        }

        private static async void BotOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            await BotClient.SendTextMessageAsync(ChatIds.Test125, e.CallbackQuery.Message.Text);
        }

        private static async void BotOnOnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            await BotClient.SendTextMessageAsync(ChatIds.Test125, e.Exception.Message + " \r\n" + e.Exception.InnerException);
        }

        private static async void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            await BotClient.SendTextMessageAsync(ChatIds.Test125, receiveErrorEventArgs.ApiRequestException.Message);
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null)
                return;

            try
            {
                var recievedDataSet = new MessageDataSet(message);
                if (message.Type == MessageType.Text && message.Text.First() == '/')
                    await CommandMessageProcessor.ProcessTextMessage(BotClient, message);
                DbRepository.SaveToDatabase(recievedDataSet); //todo sql injection protection
                Console.WriteLine(recievedDataSet.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.InnerException);
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await BotClient.SendTextMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. mr\r\n"
                                                                          + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await BotClient.SendTextMessageAsync(ChatIds.Test125, exception.ToString());
                        break;
                }
            }
        }
    }
}