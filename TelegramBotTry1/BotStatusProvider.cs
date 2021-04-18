using System;
using System.Linq;

namespace TelegramBotTry1
{
    public static class BotStatusProvider
    {
        public static string GetIAmAliveMessage()
        {
            DateTime lastMessageDate;
            string lastMessageChat;

            using (var context = new MsgContext())
            {
                var lastMessage = context.MessageDataSets.OrderByDescending(message => message.Date).FirstOrDefault();
                lastMessageDate = lastMessage?.Date.AddHours(5) ?? DateTime.MinValue;
                lastMessageChat = lastMessage?.ChatName;
            }

            return "Работаю в штатном режиме\r\nПоследнее сообщение от " + lastMessageDate.ToString("dd.MM.yyyy H:mm") +
                   " в \"" + lastMessageChat + "\"";
        }
    }
}