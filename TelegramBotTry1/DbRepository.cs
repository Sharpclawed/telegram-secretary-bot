using TelegramBotTry1.Domain;

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
    }
}