using System.Linq;
using DAL;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.DataProviders
{
    public static class AdminsListProvider
    {
        public static CommandResult GetRows()
        {
            var result = new CommandResult();
            using (var context = new SecretaryContext())
            {
                result.Records = context.Set<AdminDataSet>().AsNoTracking()
                    .Where(x => x.DeleteTime == null).ToList()
                    .Select(x => x.UserName + " добавлен " + x.AddTime.ToShortDateString() + " " + x.AddedUserName)
                    .ToList();
            }
            //Packing result is another responsibility
            if (result.Records.Any())
                result.Caption = "Список админов:\r\n";
            else
                result.Error = "Список админов пуст\r\n";
            
            return result;
        }
    }
}