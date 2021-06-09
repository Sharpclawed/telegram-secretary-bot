using System;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewHistoryOfCommand : IBotCommand
    {
        public DateTime Begin { get; }
        public DateTime End { get; }
        public long UserId { get; }

        public ViewHistoryOfCommand(DateTime begin, DateTime end, long userId)
        {
            Begin = begin;
            End = end;
            UserId = userId;
        }

        public CommandResult Process()
        {
            return HistoryProvider.GetRows(Begin, End, null, UserId);
        }
    }
}