using System.Linq;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services
{
    public class OneTimeChatService
    {
        public void Make(string chatName)
        {
            using var context = new SecretaryContext();
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var onetimeChatDataSets = context.OnetimeChatDataSets;
            var chat = messageDataSets.GetChatByChatName(chatName);
            if (!onetimeChatDataSets.Any(x => x.ChatId == chat.Id))
            {
                onetimeChatDataSets.Add(new OnetimeChatDataSet
                {
                    ChatName = chat.Name,
                    ChatId = chat.Id
                });
                context.SaveChanges();
            }
        }

        public bool Unmake(string chatName)
        {
            using var context = new SecretaryContext();
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var onetimeChatDataSets = context.OnetimeChatDataSets;
            var chat = messageDataSets.GetChatByChatName(chatName);
            var chatToRemove = onetimeChatDataSets.FirstOrDefault(x => x.ChatId == chat.Id);
            if (chatToRemove != null)
            {
                onetimeChatDataSets.Remove(chatToRemove);
                context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}