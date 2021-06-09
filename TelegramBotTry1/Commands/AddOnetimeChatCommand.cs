using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class AddOnetimeChatCommand : IBotCommand
    {
        public string ChatName { get; }

        public AddOnetimeChatCommand(string chatName)
        {
            ChatName = chatName;
        }

        public CommandResult Process()
        {
            using (var context = new MsgContext())
            {
                var messageDataSets = context.Set<MessageDataSet>().AsNoTracking();
                var onetimeChatDataSets = context.Set<OnetimeChatDataSet>();
                var chat = messageDataSets.GetChatByChatName(ChatName);
                if (!onetimeChatDataSets.Any(x => x.ChatId == chat.Id))
                {
                    onetimeChatDataSets.Add(new OnetimeChatDataSet
                    {
                        ChatName = chat.Name,
                        ChatId = chat.Id
                    });
                    context.SaveChanges();
                }
            }

            return new CommandResult { Message = "Команда обработана" };
        }
    }
}