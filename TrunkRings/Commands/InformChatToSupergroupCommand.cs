using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    internal class InformChatToSupergroupCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatToReport;
        private readonly Chat group;
        private readonly long migrateToChatId;

        public InformChatToSupergroupCommand(ITgBotClientEx tgClient, ChatId chatToReport, Chat group, long migrateToChatId)
        {
            this.tgClient = tgClient;
            this.chatToReport = chatToReport;
            this.group = group;
            this.migrateToChatId = migrateToChatId;
        }

        public async Task ProcessAsync()
        {
            await tgClient.SendTextMessageAsync(chatToReport,
                $"Чат {group.Title} сменил Id: {group.Id} на {migrateToChatId}");
        }
    }
}
