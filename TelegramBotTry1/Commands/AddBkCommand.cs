using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class AddBkCommand : IBotCommand
    {
        public string BkName { get; }

        public AddBkCommand(string bkName)
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
            return new CommandResult { Message = "Команда обработана" };
        }
    }
}