using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Domain;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.Commands
{
    public class RemoveBkCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string BkName { get; }

        public RemoveBkCommand(ITgBotClientEx tgClient, ChatId chatId, string bkName)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            BkName = bkName;
        }

        public async Task ProcessAsync()
        {
            string result;
            using (var context = new MsgContext())
            {
                var bkDataSets = context.Set<BookkeeperDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(BkName);
                var bkToRemove = bkDataSets.FirstOrDefault(x => x.UserId == user.UserId);
                if (bkToRemove != null)
                {
                    bkDataSets.Remove(bkToRemove);
                    context.SaveChanges();
                    result = "Команда обработана";
                }
                else
                    result = "Пользователь не найден";
            }
            
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}