using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    class CheckAddedMemberCommand :IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatToReport;
        private readonly User[] newMembers;
        private readonly Chat fromChat;

        public CheckAddedMemberCommand(ITgBotClientEx tgClient, ChatId chatToReport, User[] newMembers, Chat fromChat)
        {
            this.tgClient = tgClient;
            this.chatToReport = chatToReport;
            this.newMembers = newMembers;
            this.fromChat = fromChat;
        }

        public async Task ProcessAsync()
        {
            var secretaryBot = await tgClient.GetMeAsync();
            var botItselfAdded = newMembers.Select(z => z.Id).Contains(secretaryBot.Id);
            if (botItselfAdded)
                await tgClient.SendTextMessageAsync(chatToReport,
                    $"{secretaryBot.FirstName} добавлен в чат {fromChat.Title} (Id: {fromChat.Id})");
        }
    }
}
