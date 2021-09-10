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
        public bool TryMake(string chatName, out string message)
        {
            using var context = new SecretaryContext();
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var onetimeChatDataSets = context.OnetimeChatDataSets;
            var chat = messageDataSets.FindChatByChatName(chatName);
            if (chat == null)
            {
                message = "Чат не найден";
                return false;
            }

            if (onetimeChatDataSets.Any(x => x.ChatId == chat.Id))
            {
                message = "Чат уже в списке";
                return false;
            }

            onetimeChatDataSets.Add(new OnetimeChatDataSet {ChatName = chat.Name, ChatId = chat.Id});
            context.SaveChanges();
            message = "Чат добавлен";
            return true;
        }

        public bool TryUnmake(string chatName, out string message)
        {
            using var context = new SecretaryContext();
            var messageDataSets = context.MessageDataSets.AsNoTracking();
            var onetimeChatDataSets = context.OnetimeChatDataSets;
            var chat = messageDataSets.FindChatByChatName(chatName);
            if (chat == null)
            {
                message = "Чат не найден";
                return false;
            }

            var chatToRemove = onetimeChatDataSets.FirstOrDefault(x => x.ChatId == chat.Id);
            if (chatToRemove == null)
            {
                message = "Чат отсутствует в списке";
                return false;
            }

            onetimeChatDataSets.Remove(chatToRemove);
            context.SaveChanges();
            message = "Чат успешно удален";
            return true;
        }

        public IEnumerable<Chat> GetAll()
        {
            using var context = new SecretaryContext();
            return context.OnetimeChatDataSets.AsNoTracking().ToList().Select(z => new Chat{ Id = z.ChatId, Name = z.ChatName});
        }
    }

    public interface IOneTimeChatService
    {
        bool TryMake(string chatName, out string message);
        bool TryUnmake(string chatName, out string message);
        IEnumerable<Chat> GetAll();
    }
}