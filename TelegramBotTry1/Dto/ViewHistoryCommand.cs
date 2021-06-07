using System;
using TelegramBotTry1.DataProviders;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1.Dto
{
    public class ViewHistoryCommand : IBotCommand
    {
        public EntityType EntityType => EntityType.History;
        public DateTime Begin { get; }
        public DateTime End { get; }
        public HistoryCommandType Type => HistoryCommandType.SingleChat;
        public string ChatName { get; }

        public ViewHistoryCommand(DateTime begin, DateTime end, string chatName)
        {
            Begin = begin;
            End = end;
            ChatName = chatName;
        }

        public ViewReportResult Process()
        {
            return HistoryProvider.GetRows(Begin, End, ChatName, null);
        }
    }

    public class ViewHistoryAllCommand : IBotCommand
    {
        public EntityType EntityType => EntityType.History;
        public DateTime Begin { get; }
        public DateTime End { get; }
        public HistoryCommandType Type => HistoryCommandType.AllChats;

        public ViewHistoryAllCommand(DateTime begin, DateTime end)
        {
            Begin = begin;
            End = end;
        }

        public ViewReportResult Process()
        {
            return HistoryProvider.GetRows(Begin, End, null, null);
        }
    }

    public class ViewHistoryOfCommand : IBotCommand
    {
        public EntityType EntityType => EntityType.History;
        public DateTime Begin { get; }
        public DateTime End { get; }
        public HistoryCommandType Type => HistoryCommandType.SingleUser;
        public string UserId { get; }

        public ViewHistoryOfCommand(DateTime begin, DateTime end, string userName)
        {
            Begin = begin;
            End = end;
            UserId = userName;
        }

        public ViewReportResult Process()
        {
            return HistoryProvider.GetRows(Begin, End, null, UserId);
        }
    }

    public interface IBotCommand
    {
        EntityType EntityType { get; }
        ViewReportResult Process();
    }
}