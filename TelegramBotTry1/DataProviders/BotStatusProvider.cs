using System;
using System.Linq;
using DAL;

namespace TelegramBotTry1.DataProviders
{
    public static class BotStatusProvider
    {
        public static string GetIAmAliveMessage()
        {
            DateTime lastMessageDate;
            string lastMessageChat;

            using (var context = new SecretaryContext())
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