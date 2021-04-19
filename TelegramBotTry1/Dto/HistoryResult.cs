using System.Collections.Generic;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Dto
{
    public class HistoryResult
    {
        //todo implicit operators
        public List<IMessageDataSet> Records { get; set; }
        public string Error { get; set; }
    }
}