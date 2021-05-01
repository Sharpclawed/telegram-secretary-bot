using System.Collections.Generic;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Dto
{
    public class ViewReportResult: ICommandResult<IMessageDataSet>
    {
        //todo implicit operators
        public List<IMessageDataSet> Records { get; set; }
        public string Error { get; set; }
        public string Caption { get; set; }
        public string Message { get; set; }
    }
}