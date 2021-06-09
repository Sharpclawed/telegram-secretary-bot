using System;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewInactiveChatsCommand : IBotCommand
    {
        public DateTime? SinceDate { get; }
        public DateTime? UntilDate { get; }

        public ViewInactiveChatsCommand()
        {
        }

        public ViewInactiveChatsCommand(DateTime sinceDate, DateTime untilDate)
        {
            SinceDate = sinceDate;
            UntilDate = untilDate;
        }

        public CommandResult Process()
        {
            var sinceDateValue = SinceDate ?? DateTime.UtcNow.AddDays(-365);
            var untilDateValue = UntilDate ?? DateTime.UtcNow;
            var report = ViewInactiveChatsProvider.GetInactive(sinceDateValue, untilDateValue, TimeSpan.FromDays(7));

            return new CommandResult { Messages = report, Caption = "Отчет по неактивным чатам" };
        }
    }
}