using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Dto;
using TelegramBotTry1.Enums;

namespace TelegramBotTry1
{
    public static class MessageDataSetExtensions
    {
        public static IQueryable<IMessageDataSet> GetActualDates(this IQueryable<IMessageDataSet> dataSets,
            HistoryCommand command)
        {
            return dataSets.Where(x => x.Date >= command.Begin && x.Date < command.End);
        }

        public static IQueryable<IMessageDataSet> GetActualChats(this IQueryable<IMessageDataSet> dataSets,
            HistoryCommand command)
        {
            switch (command.Type)
            {
                case HistoryCommandType.SingleChat:
                    //TODO по названию могут определяться разные чаты в разные моменты времени. Нам нужен актуальный или все?
                    return dataSets
                        .Where(x => x.ChatName.Equals(command.NameOrId, StringComparison.InvariantCultureIgnoreCase));
                case HistoryCommandType.AllChats:
                case HistoryCommandType.SingleUser:
                    return dataSets.Where(x => x.ChatName != null);
                default:
                    return null;
            }
        }

        public static IQueryable<IMessageDataSet> GetActualUser(this IQueryable<IMessageDataSet> dataSets,
            HistoryCommand command)
        {
            switch (command.Type)
            {
                case HistoryCommandType.SingleUser:
                    return from dataset in dataSets
                        join chatId in dataSets.Where(x => x.UserId.ToString() == command.NameOrId)
                            .Select(x => x.ChatId).Distinct() on dataset.ChatId equals chatId
                        select dataset;
                default:
                    return dataSets;
            }
        }

        public static User GetUserByUserName(this IQueryable<IMessageDataSet> dataSets, string userName)
        {
            var message = dataSets.Where(x => x.UserName.ToLower() == userName.ToLower()).OrderByDescending(x => x.Date).FirstOrDefault();
            if (message == null)
                throw new ArgumentException("Пользователь не найден");
            return new User
            {
                UserId = message.UserId,
                Username = message.UserName,
                Name = message.UserFirstName,
                Surname = message.UserLastName
            };
        }

        public static Chat GetChatByChatName(this IQueryable<IMessageDataSet> dataSets, string chatName)
        {
            var message = dataSets.Where(x => x.ChatName.ToLower() == chatName.ToLower()).OrderByDescending(x => x.Date).FirstOrDefault();
            if (message == null)
                throw new ArgumentException("Чат не найден");
            return new Chat
            {
                Id = message.ChatId,
                Name = message.ChatName,
            };
        }

        public static IEnumerable<MessageDataSet> FilterObviouslySuperfluous(this IEnumerable<MessageDataSet> dataSets)
        {
            var ignoreUnanswered = new List<string>
            {
                "",
                "ага",
                "большое",
                "вам",
                "вас",
                "вроде бы все",
                "все",
                "да",
                "да конечно",
                "нет",
                "ну да",
                "ну",
                "ок",
                "ок спс",
                "окей",
                "окей спс",
                "попробую",
                "примем",
                "принято",
                "сделаю",
                "сделаю сегодня",
                "спс",
                "ура",
                "ясно",
                "jr",
                "ok",
            };

            return
                dataSets.Select(z => new
                    {
                        msg = z,
                        txt = new string(
                            Regex.Replace(z.Message, @"\p{Cs}", "")
                            .ToLower()
                            .Replace(new[] {')', '(', '+' }, "")
                            .Replace("айгюль", "")
                            .Replace("благодарю", "")
                            .Replace("благодарим", "")
                            .Replace("вероника", "")
                            .Replace("взаимно", "")
                            .Replace("виктория", "")
                            .Replace("добрый вечер", "")
                            .Replace("добрый день", "")
                            .Replace("доброе утро", "")
                            .Replace("здравствуйте", "")
                            .Replace("екатерина", "")
                            .Replace("ирина", "")
                            .Replace("марина", "")
                            .Replace("насть", "")
                            .Replace("ольга", "")
                            .Replace("отлично", "")
                            .Replace("поняла", "")
                            .Replace("понял", "")
                            .Replace("понятно", "")
                            .Replace("светлана", "")
                            .Replace("спаибо", "")
                            .Replace("спасибо", "")
                            .Replace("супер", "")
                            .Replace("спсаибо", "")
                            .Replace("татьяна", "")
                            .Replace("увидела", "")
                            .Replace("увидел", "")
                            .Replace("хорлшо", "")
                            .Replace("хорошо", "")
                            .Where(c => !char.IsPunctuation(c)).ToArray())
                            .Trim()
                })
                    .Where(z => !ignoreUnanswered.Contains(z.txt))
                    .Select(z => z.msg)
                    .ToList();
        }
    }
}