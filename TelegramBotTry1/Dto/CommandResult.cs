using System.Collections.Generic;
using DAL.Models;

namespace TelegramBotTry1.Dto
{
    public class CommandResult
    {
        public List<MessageDataSet> Messages { get; set; }
        public List<string> Records { get; set; }
        public string Error { get; set; }
        public string Caption { get; set; }
    }
}