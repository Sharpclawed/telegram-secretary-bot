using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using TelegramBotTry1.DomainExtensions;

namespace TelegramBotTry1
{
    public static class DbRepository
    {
        public static void SaveToDatabase(MessageDataSet messageDataSet)
        {
            using (var context = new SecretaryContext())
            {
                context.MessageDataSets.Add(messageDataSet);
                context.SaveChanges();
            }
        }

        public static bool IsAdmin(int askerId)
        {
            using (var context = new SecretaryContext())
            {
                //todo отвязаться от efcore, когда это уедет отсюда
                return context.AdminDataSets.AsNoTracking().IsAdmin(askerId);
            }
        }
    }
}