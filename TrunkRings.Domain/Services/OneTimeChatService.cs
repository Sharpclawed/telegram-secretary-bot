using System.Collections.Generic;
using System.Linq;
using TrunkRings.DAL;
using TrunkRings.DAL.Models;
using Microsoft.EntityFrameworkCore;
using TrunkRings.Domain.Models;

namespace TrunkRings.Domain.Services
{
    public class OneTimeChatService : IOneTimeChatService
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

        public IEnumerable<Chat> GetAll()
        {
            using var context = new SecretaryContext();
            return context.OnetimeChatDataSets.AsNoTracking().ToList().Select(z => new Chat{ Id = z.ChatId, Name = z.ChatName});
        }
    }

    public interface IOneTimeChatService
    {
        void Make(string chatName);
        bool Unmake(string chatName);
        IEnumerable<Chat> GetAll();
    }
}