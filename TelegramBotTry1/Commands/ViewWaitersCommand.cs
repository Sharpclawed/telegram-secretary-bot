using System;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewWaitersCommand : IBotCommand
    {
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewWaitersCommand()
        {
        }

        public ViewWaitersCommand(DateTime sinceDate, DateTime untilDate)
        {
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }

        public CommandResult Process()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.Date.AddMonths(-1);
            var untilDateValue = UntilDate ?? DateTime.UtcNow.Date.AddMinutes(-30);
            var waitersReport = ViewWaitersProvider.GetWaiters(sinceDateValue, untilDateValue);

            return new CommandResult { Messages = waitersReport, Caption = "Отчет по неотвеченным сообщениям" };
        }
    }
}