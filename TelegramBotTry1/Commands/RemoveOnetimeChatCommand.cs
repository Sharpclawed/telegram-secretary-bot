using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Domain;

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
            using (var context = new MsgContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                var chat = messageDataSets.GetChatByChatName(ChatName);
                onetimeChatDataSets.Remove(onetimeChatDataSets.First(x => x.ChatId == chat.Id));
                context.SaveChanges();
            }

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}