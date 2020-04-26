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
        //public MessageType MessageType { get; set; }

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
            switch (message.Type)
            {
                case MessageType.Text:
                    Message = message.Text;
                    break;
                case MessageType.Sticker:
                    Message = message.Sticker.Emoji;
                    break;
                case MessageType.Contact:
                    Message = message.Contact.FirstName + " " +
                              message.Contact.LastName + " (" +
                              message.Contact.UserId + "): " +
                              message.Contact.PhoneNumber;
                    break;

                case MessageType.Document:
                case MessageType.Voice:
                case MessageType.Photo:
                case MessageType.Unknown:
                case MessageType.Audio:
                case MessageType.Video:
                case MessageType.Location:
                case MessageType.Venue:
                case MessageType.Game:
                case MessageType.VideoNote:
                case MessageType.Invoice:
                case MessageType.SuccessfulPayment:
                case MessageType.WebsiteConnected:
                case MessageType.ChatMembersAdded:
                case MessageType.ChatMemberLeft:
                case MessageType.ChatTitleChanged:
                case MessageType.ChatPhotoChanged:
                case MessageType.MessagePinned:
                case MessageType.ChatPhotoDeleted:
                case MessageType.GroupCreated:
                case MessageType.SupergroupCreated:
                case MessageType.ChannelCreated:
                case MessageType.MigratedToSupergroup:
                case MessageType.MigratedFromGroup:
                case MessageType.Poll:
                    Message = "MessageType: " + message.Type;
                    break;
            }
            //MessageType = message.Type;
        }
    }
}
