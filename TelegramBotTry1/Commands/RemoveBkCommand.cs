using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class RemoveBkCommand : IBotCommand
    {
        public string BkName { get; }

        public RemoveBkCommand(string bkName)
        {
            BkName = bkName;
        }

        public CommandResult Process()
        {
            using (var context = new MsgContext())
            {
                var bkDataSets = context.Set<BookkeeperDataSet>();
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var user = messageDataSets.GetUserByUserName(BkName);
                bkDataSets.Remove(bkDataSets.First(x => x.UserId == user.UserId));
                context.SaveChanges();
            }
            return new CommandResult { Message = "Команда обработана" };
        }
    }
}