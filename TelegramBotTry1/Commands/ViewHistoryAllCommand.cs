using System;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewHistoryAllCommand : IBotCommand
    {
        public DateTime Begin { get; }
        public DateTime End { get; }

        public ViewHistoryAllCommand(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }

        public CommandResult Process()
        {
            return HistoryProvider.GetRows(Begin, End, null, null);
        }
    }
}