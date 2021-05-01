using System.Collections.Generic;

namespace TelegramBotTry1.Dto
{
    public interface ICommandResult<TItem>
    {
        List<TItem> Records { get; set; }
        string Error { get; set; }
        string Caption { get; set; }
        string Message { get; set; }
    }
}