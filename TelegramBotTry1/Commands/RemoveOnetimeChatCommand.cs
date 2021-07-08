using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.Commands
{
    public class RemoveOnetimeChatCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string ChatName { get; }

        public RemoveOnetimeChatCommand(ITgBotClientEx tgClient, ChatId chatId, string chatName)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            ChatName = chatName;
        }

        public async Task ProcessAsync()
        {
            string result;
            using (var context = new SecretaryContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                var chat = messageDataSets.GetChatByChatName(ChatName);
                var chatToRemove = onetimeChatDataSets.FirstOrDefault(x => x.ChatId == chat.Id);
                if (chatToRemove != null)
                {
                    onetimeChatDataSets.Remove(chatToRemove);
                    result = "Команда обработана";
                    context.SaveChanges();
                }
                else
                    result = "Чат не найден";
            }

            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}