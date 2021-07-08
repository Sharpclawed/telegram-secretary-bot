using System.Linq;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.DataProviders
{
    public static class BookkeepersListProvider
    {
        public static CommandResult GetRows()
        {
            var result = new CommandResult();
            using (var context = new SecretaryContext())
            {
                var bkDataSets = context.Set<BookkeeperDataSet>().AsNoTracking();
                result.Records = bkDataSets.ToList()
                    .Select(x => x.UserFirstName + " " + x.UserLastName).ToList();
            }

            //TODO should return data only
            if (result.Records.Any())
                result.Caption = "Список бухгалтеров:\r\n";
            else
                result.Error = "Список бухгалтеров пуст\r\n";

            return result;
        }
    }
}