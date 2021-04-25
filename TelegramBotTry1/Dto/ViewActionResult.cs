using System.Collections.Generic;

namespace TelegramBotTry1.Dto
{
    public class ViewActionResult : ICommandResult<string>
    {
        //todo implicit operators
        public List<string> Records { get; set; }
        public string Error { get; set; }
        public string Caption { get; set; }
        public string Message { get; set; }
    }
}