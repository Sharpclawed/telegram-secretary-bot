using System.Collections.Generic;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Dto
{
    public class CommandResult
    {
        public List<IMessageDataSet> Messages { get; set; }
        public List<string> Records { get; set; }
        public string Error { get; set; }
        public string Caption { get; set; }
    }
}