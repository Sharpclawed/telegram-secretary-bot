using System;
using TrunkRings.Domain.Models;

namespace TrunkRings.DomainExtensions
{
    public static class Formatter
    {
        public static Func<DomainMessage, string> Waiters { get; } = msg =>
        {
            var timeWithoutAnswer = DateTime.UtcNow.Subtract(msg.Date.AddHours(-3));
            return string.Format(
                @"В чате {0} сообщение от {1} {2}, оставленное {3}, без ответа ({4}). Текст сообщения: ""{5}"""
                , msg.ChatName, msg.UserLastName, msg.UserFirstName
                , msg.Date.AddHours(5).ToString("dd.MM.yyyy H:mm")
                , timeWithoutAnswer.Days + " дней " + timeWithoutAnswer.Hours + " часов " +
                  timeWithoutAnswer.Minutes + " минут"
                , msg.Message);
        };

        public static Func<DomainMessage, string> InactiveChats { get; } = msg =>
        {
            var untilDate = DateTime.UtcNow;
            var mark = string.Empty;
            switch (msg.Date)
            {
                case DateTime date when (untilDate - date).TotalDays < 7:
                    break;
                case DateTime date when (untilDate - date).TotalDays < 14:
                    mark = " больше недели 😐";
                    break;
                case DateTime date when (untilDate - date).TotalDays < 21:
                    mark = " больше двух недель 😕";
                    break;
                default:
                    mark = " больше трех недель 😟";
                    break;
            }

            return string.Format(
                @"В чате {0} нет активной переписки{1}. Последнее сообщение от клиента было {2}. Текст сообщения: ""{3}"""
                , msg.ChatName
                , mark
                , msg.Date.AddHours(5).ToString("dd.MM.yyyy в H:mm")
                , msg.Message);
        };

        public static Func<(Admin, Admin), string> AdminWithAddedBy { get; } = pair => pair.Item1.UserName + " добавлен " + pair.Item2.UserName;
    }
}