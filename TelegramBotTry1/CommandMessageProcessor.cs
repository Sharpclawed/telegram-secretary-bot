using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using DAL.Models;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotTry1
{
    public class CommandMessageProcessor
    {
        private readonly ITgBotClientEx tgClient;
        private readonly CommandDetector commandDetector;

        public CommandMessageProcessor(ITgBotClientEx tgClient)
        {
            this.tgClient = tgClient;
            commandDetector = new CommandDetector(tgClient);
        }

        public async Task ProcessTextMessageAsync(Message message)
        {
            try
            {
                SaveToDatabase(message);
                if (message.Type != MessageType.Text || message.Text.First() != '/')
                    return;

                var isMessagePersonal = message.Chat.Title == null;
                if (!isMessagePersonal)
                    return;

                var isAdminAsking = DbRepository.IsAdmin(message.From.Id);
                if (!isAdminAsking)
                {
                    await tgClient.SendTextMessageAsync(message.Chat.Id, "У вас не хватает прав");
                    return;
                }

                var command = commandDetector.Parse(message);
                await command.ProcessAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.InnerException);
                switch (exception)
                {
                    case SocketException _:
                    case ObjectDisposedException _:
                        await tgClient.SendTextMessageAsync(ChatIds.Botva,
                            "Пропала коннекция к базе. Отключаюсь, чтобы не потерялись данные. mr\r\n"
                            + "Пожалуйста, включите меня в течение суток");
                        throw;
                    default:
                        await tgClient.SendTextMessageAsync(ChatIds.Test125, exception.ToString());
                        break;
                }
            }
        }

        private void SaveToDatabase(Message message)
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

            DbRepository.SaveToDatabase(recievedDataSet); //todo sql injection protection
            Console.WriteLine(recievedDataSet.ToString());
        }
    }
}