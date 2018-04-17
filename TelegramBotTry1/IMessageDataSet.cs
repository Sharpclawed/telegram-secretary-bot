using System;

namespace TelegramBotTry1
{
    public interface IMessageDataSet
    {
        long ChatId { get; }
        string ChatName { get; }
        DateTime Date { get; }
        string Message { get; set; }
        Guid MessageDataSetId { get; }
        long MessageId { get; }
        string UserFirstName { get; }
        long UserId { get; }
        string UserLastName { get; }
        string UserName { get; }

        string ToString();
    }
}