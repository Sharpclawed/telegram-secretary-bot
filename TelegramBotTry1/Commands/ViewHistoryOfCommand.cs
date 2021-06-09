using System;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewHistoryOfCommand : IBotCommand
    {
        public DateTime Begin { get; }
        public DateTime End { get; }
        public string UserId { get; }

        public ViewHistoryOfCommand(DateTime begin, DateTime end, string userName)
        {
            Begin = begin;
            End = end;
            UserId = userName;
        }

        public CommandResult Process()
        {
            return HistoryProvider.GetRows(Begin, End, null, UserId);
        }
    }
}