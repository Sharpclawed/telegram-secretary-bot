using System;
using System.Net.Sockets;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Reporters;

namespace TelegramBotTry1
{
    static class Program
    {
        //todo hide and change tokens
        private static readonly TelegramBotClient Bot =
            new TelegramBotClient("361040811:AAGQlsM84JwDIRtcztbMMboKLXWqbPwW4VI");  //kontakt bot
            //new TelegramBotClient("245135166:AAEYEEsWjQmN_wLENwnA84Wb9xkgQJ-TLFE");   //my bot

        static void Main()
        {
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnReceiveError += BotOnReceiveError;
            Bot.OnReceiveGeneralError += BotOnOnReceiveGeneralError;
            Bot.OnCallbackQuery += BotOnOnCallbackQuery;

            Console.Title = "Secretary bot " + Bot.BotId;

            using (var context = new MsgContext())
            {
                context.Database.CreateIfNotExists();
            }
            var botStateInformer = new BotStateReporter(Bot);
            var waitersViewInformer = new WaitersReporter(Bot);
            var inactiveChatsReporter = new InactiveChatsReporter(Bot);

            Console.WriteLine(DateTime.Now + " Start working");
            botStateInformer.Start();
            waitersViewInformer.Start();
            inactiveChatsReporter.Start();

            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static void BotOnOnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            Bot.SendTextMessageAsync(ChatIds.Test125, e.CallbackQuery.Message.Text);
        }

        private static void BotOnOnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            Bot.SendTextMessageAsync(ChatIds.Test125, e.Exception.Message + " \r\n" + e.Exception.InnerException);
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Bot.SendTextMessageAsync(ChatIds.Test125, receiveErrorEventArgs.ApiRequestException.Message);
        }

        private static void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;

            if (message == null)
                return;

            try
            {
                var recievedDataSet = new MessageDataSet(message);
                if (message.Type == MessageType.Text)
                    CommandMessageProcessor.ProcessTextMessage(Bot, message);
                DbRepository.SaveToDatabase(recievedDataSet); //todo sql injection protection
                Console.WriteLine(recievedDataSet.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        Bot.SendTextMessageAsync(ChatIds.Botva, "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. mr\r\n"
                                                                          + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        Bot.SendTextMessageAsync(ChatIds.Test125, exception.ToString());
                        break;
                }
            }
        }
    }
}