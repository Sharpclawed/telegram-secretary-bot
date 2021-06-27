using System;
using System.Linq;
using System.Net.Sockets;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Reporters;
using TelegramBotTry1.SecretsStore;

namespace TelegramBotTry1
{
    static class Program
    {
        private static readonly ITelegramBotClientAdapter BotClient = new BotClientAdapter(Secrets.MainBotToken);

        static void Main()
        {
            BotClient.OnMessage += BotOnMessageReceived;
            BotClient.OnMessageEdited += BotOnMessageReceived;
            BotClient.OnReceiveError += BotOnReceiveError;
            BotClient.OnReceiveGeneralError += BotOnOnReceiveGeneralError;
            BotClient.OnCallbackQuery += BotOnOnCallbackQuery;

            Console.Title = BotClient.GetMeAsync().Result.Username;

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
            try
            {
                //todo automapper
                var recievedDataSet = new MessageDataSet
                {
                    MessageId = message.MessageId,
                    Date = message.Date,
                    UserName = message.From.Username,
                    UserFirstName = message.From.FirstName,
                    UserLastName = message.From.LastName,
                    UserId = message.From.Id,
                    ChatId = message.Chat.Id,
                    ChatName = message.Chat.Title,
                    Message = message.Type switch
                    {
                        MessageType.Text => message.Text,
                        MessageType.Sticker => message.Sticker.Emoji,
                        MessageType.Contact => message.Contact.FirstName + " " + message.Contact.LastName + " (" +
                                               message.Contact.UserId + "): " + message.Contact.PhoneNumber,
                        _ => "MessageType: " + message.Type
                    }
                };
                if (message.Type == MessageType.Text && message.Text.First() == '/')
                    await CommandMessageProcessor.ProcessTextMessageAsync(BotClient, message);
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