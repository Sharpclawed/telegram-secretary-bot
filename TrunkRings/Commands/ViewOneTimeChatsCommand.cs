using System.Linq;
using System.Threading.Tasks;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;

namespace TrunkRings.Commands
{
    public class ViewOneTimeChatsCommand : IBotCommand
    {
        private readonly IOneTimeChatService oneTimeChatService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;

        public ViewOneTimeChatsCommand(IOneTimeChatService oneTimeChatService, ITgBotClientEx tgClient, ChatId chatId)
        {
            this.oneTimeChatService = oneTimeChatService;
            this.tgClient = tgClient;
            this.chatId = chatId;
        }

        public async Task ProcessAsync()
        {
            var records = oneTimeChatService.GetAll().ToList();

            if (records.Any())
            {
                var caption = "Список исключений для просмотра неактивных чатов:\r\n";
                await tgClient.SendTextMessagesAsSingleTextAsync(chatId, records.Select(x => x.Name).ToList(), caption);
            }
            else
            {
                var caption = "Список исключений для просмотра неактивных чатов пуст\r\n";
                await tgClient.SendTextMessageAsync(chatId, caption);
            }
        }
    }
}