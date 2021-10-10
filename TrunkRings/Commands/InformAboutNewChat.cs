using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class InformAboutNewChat : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatToReport;
        private readonly Chat fromChat;

        public InformAboutNewChat(ITgBotClientEx tgClient, ChatId chatToReport, Chat fromChat)
        {
            this.tgClient = tgClient;
            this.chatToReport = chatToReport;
            this.fromChat = fromChat;
        }

        public async Task ProcessAsync()
        {
            await tgClient.SendTextMessageAsync(chatToReport, $"Создан новый чат {fromChat.Title} (Id: {fromChat.Id})");
        }
    }
}