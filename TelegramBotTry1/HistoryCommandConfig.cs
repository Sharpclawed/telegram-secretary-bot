using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TelegramBotTry1
{
    public class HistoryCommandConfig
    {
        public DateTime Begin { get; private set; }
        public DateTime End { get; private set; }
        public HistoryCommandType Type { get; private set; }
        public string Argument { get; private set; }

        public HistoryCommandConfig(string messageText)
        {
            var regex = new Regex(@"^(/history.*)[:]\s*(.*)\s\b(\d{2}[.]\d{2}[.]\d{4})[ ](\d+)$");
            var match = regex.Match(messageText);
            if (match == Match.Empty)
            {
                Type = HistoryCommandType.Unknown;
                return;
            }
            Begin = DateTime.ParseExact(match.Groups[3].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            Argument = match.Groups[2].Value;
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