using System;
using System.Globalization;
using System.Text.RegularExpressions;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1.Dto
{
    public class HistoryCommand
    {
        public DateTime Begin { get; }
        public DateTime End { get; }
        public HistoryCommandType Type { get; }
        public string NameOrId { get; }

        public HistoryCommand(string messageText)
        {
            var regex = new Regex(@"^(/history.*)[:]\s*(.*)\s\b(\d{2}[.]\d{2}[.]\d{4})[ ](\d+)$");
            var match = regex.Match(messageText);
            if (match == Match.Empty)
            {
                Type = HistoryCommandType.Unknown;
                return;
            }
            Begin = DateTime.ParseExact(match.Groups[3].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            NameOrId = match.Groups[2].Value;
            End = Begin.AddDays(double.Parse(match.Groups[4].Value));

            switch (match.Groups[1].Value)
            {
                case "/history":
                    Type = HistoryCommandType.SingleChat;
                    break;
                case "/historyall":
                    Type = HistoryCommandType.AllChats;
                    break;
                case "/historyof":
                    Type = HistoryCommandType.SingleUser;
                    break;
                default:
                    Type = HistoryCommandType.Unknown;
                    break;
            }
        }
    }
}