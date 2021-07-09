using DAL;
using DAL.Models;
using Domain.Models;

namespace Domain.Services
{
    public class MessageService
    {
        public void Save(DomainMessage message)
        {
            using var context = new SecretaryContext();
            var messageDataSet = new MessageDataSet
            {
                MessageId = message.MessageId,
                Date = message.Date,
                UserName = message.UserName,
                UserFirstName = message.UserFirstName,
                UserLastName = message.UserLastName,
                UserId = message.UserId,
                ChatId = message.ChatId,
                ChatName = message.ChatName,
                Message = message.Message
            };
            context.MessageDataSets.Add(messageDataSet); //todo sql injection protection
            context.SaveChanges();
        }
    }
}