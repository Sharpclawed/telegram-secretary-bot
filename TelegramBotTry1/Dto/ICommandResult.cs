using System.Collections.Generic;
using TelegramBotTry1.Domain;

namespace TelegramBotTry1.Dto
{
    //Подумать над разделением
    public interface ICommandResult
    {
        List<IMessageDataSet> Messages { get; set; }
        List<string> Records { get; set; }
        string Error { get; set; }
        string Caption { get; set; }
        string Message { get; set; }
    }
}