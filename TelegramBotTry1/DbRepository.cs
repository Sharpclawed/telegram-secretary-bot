using TelegramBotTry1.Domain;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1
{
    public static class DbRepository
    {
        public static void SaveToDatabase(MessageDataSet messageDataSet)
        {
            using (var context = new MsgContext())
            {
                context.MessageDataSets.Add(messageDataSet);
                context.SaveChanges();
            }
        }

        public static bool IsAdmin(int askerId)
        {
            using (var context = new MsgContext())
            {
                return context.Set<AdminDataSet>().AsNoTracking().IsAdmin(askerId);
            }
        }
    }
}