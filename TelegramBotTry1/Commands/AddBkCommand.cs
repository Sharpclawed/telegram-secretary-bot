using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotTry1.Domain;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1.Commands
{
    public class AddBkCommand : IBotCommand
    {
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string BkName { get; }

        public AddBkCommand(ITgBotClientEx tgClient, ChatId chatId, string bkName)
        {
            this.tgClient = tgClient;
            this.chatId = chatId;
            BkName = bkName;
        }

        public async Task ProcessAsync()
        {
            using (var context = new MsgContext())
            {
                var bkDataSets = context.Set<BookkeeperDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(BkName);
                if (!bkDataSets.Any(x => x.UserId == user.UserId))
                {
                    bkDataSets.Add(new BookkeeperDataSet
                    {
                        UserId = user.UserId,
                        UserName = user.Username,
                        UserFirstName = user.Name,
                        UserLastName = user.Surname
                    });
                    context.SaveChanges();
                }
            }

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}