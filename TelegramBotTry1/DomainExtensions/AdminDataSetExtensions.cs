using System.Linq;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.DomainExtensions
{
    public static class AdminDataSetExtensions
    {
        public static bool IsAdmin(this IQueryable<AdminDataSet> dataSets, long userId)
        {
            return dataSets.Where(x => x.UserId == userId).SingleOrDefault(x => x.DeleteTime == null) != null;
        }
    }
}