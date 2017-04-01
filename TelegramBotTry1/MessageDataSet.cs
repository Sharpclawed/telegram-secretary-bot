using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotTry1
{
    public class MessageDataSet
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //TODO Разбить на разные таблицы
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
                case MessageType.TextMessage:
                    Message = message.Text;
                    break;
                case MessageType.StickerMessage:
                    Message = message.Sticker.Emoji;
                    break;
                case MessageType.ContactMessage:
                    Message = message.Contact.FirstName + " " +
                              message.Contact.LastName + " (" +
                              message.Contact.UserId + "): " +
                              message.Contact.PhoneNumber;
                    break;
                case MessageType.VoiceMessage:
                    Message = ContentSaver.GetPath(message.Voice.FileId, message.Voice.FileId + ".ogg");
                    break;
                case MessageType.DocumentMessage:
                    Message = ContentSaver.GetPath(message.Document.FileId, message.Document.FileName);
                    break;
                case MessageType.PhotoMessage:
                    foreach (var photo in message.Photo.Where(x => x.FilePath != null))
                    {
                        Message = ContentSaver.GetPath(photo.FileId, photo.FilePath.Replace("/",""));
                    }
                    break;
                case MessageType.UnknownMessage:
                case MessageType.AudioMessage:
                case MessageType.VideoMessage:
                case MessageType.LocationMessage:
                case MessageType.ServiceMessage:
                case MessageType.VenueMessage:
                default:
                    break;
            }
            //MessageType = message.Type;
        }
    }
}
