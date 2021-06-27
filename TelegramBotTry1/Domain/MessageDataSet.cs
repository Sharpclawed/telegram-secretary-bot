using System;
using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotTry1.Domain
{
    public class MessageDataSet : IMessageDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MessageDataSetId { get; set; }
        public long MessageId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string UserName { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string ChatName { get; set; }

        public override string ToString()
        {
            return  ChatName + " " +
                    UserFirstName + " " +
                    UserLastName + " " +
                    UserName + "| " +
                    Date + " " +
                    Message;
        }

        public MessageDataSet()
        {
        }

        public MessageDataSet(Message message)
        {
            MessageId = message.MessageId;
            Date = message.Date;
            UserName = message.From.Username;
            UserFirstName = message.From.FirstName;
            UserLastName = message.From.LastName;
            UserId = message.From.Id;
            ChatId = message.Chat.Id;
            ChatName = message.Chat.Title;
            Message = message.Type switch
            {
                MessageType.Text => message.Text,
                MessageType.Sticker => message.Sticker.Emoji,
                MessageType.Contact => message.Contact.FirstName + " " + message.Contact.LastName + " (" + message.Contact.UserId + "): " + message.Contact.PhoneNumber,
                _ => "MessageType: " + message.Type
            };
        }
    }
}