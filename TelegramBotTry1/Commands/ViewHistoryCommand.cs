using System;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Dto;

namespace TelegramBotTry1.Commands
{
    public class ViewHistoryCommand : IBotCommand
    {
        public DateTime Begin { get; }
        public DateTime End { get; }
        public string ChatName { get; }

        public ViewHistoryCommand(DateTime begin, DateTime end, string chatName)
        {
            Begin = begin;
            End = end;
            ChatName = chatName;
        }

        public CommandResult Process()
        {
            return HistoryProvider.GetRows(Begin, End, ChatName, null);
        }
    }
}