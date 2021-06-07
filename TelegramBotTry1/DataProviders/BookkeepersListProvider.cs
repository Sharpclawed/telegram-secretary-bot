using System.Linq;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.DataProviders
{
    public static class BookkeepersListProvider
    {
        public static ViewReportResult GetRows()
        {
            var result = new ViewReportResult();
            using (var context = new MsgContext())
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