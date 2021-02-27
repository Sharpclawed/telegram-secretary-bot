using System;
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
                "а",
                "ага",
                "вам",
                "вас",
                "вроде бы все",
                "все",
                "да",
                "договорились",
                "за",
                "и вам",
                "и вас",
                "нет",
                "ну да",
                "ну",
                "ок",
                "окей",
                "ура",
                "ясно",
                "jr",
                "ok",
            };

            var msgsWithDebugData = dataSets.Select(z => new
                {
                    msg = z,
                    txt = new string(
                            Regex.Replace(z.Message, @"\p{Cs}", " ")
                                .ToLower()
                                .Replace("ё", "е")
                                .Replace("☺️", string.Empty)
                                .Replace("✅", string.Empty)
                                .Replace("❤️", string.Empty)
                                .Replace(new[] {')', '(', '+'}, "")
                                .Replace("  ", " ")
                                .Replace("ааа", "а")
                                .Replace("аа", "а")
                                .Replace("айгуль", "")
                                .Replace("айгюль", "")
                                .Replace("анна", "")
                                .Replace("благодарю", "")
                                .Replace("благодарим", "")
                                .Replace("большое", "")
                                .Replace("валентина", "")
                                .Replace("вероника", "")
                                .Replace("вечер", "")
                                .Replace("взаимно", "")
                                .Replace("виктория", "")
                                .Replace("гляну", "")
                                .Replace("готово", "")
                                .Replace("громадное", "")
                                .Replace("гузель", "")
                                .Replace("девочки", "")
                                .Replace("день", "")
                                .Replace("добрый", "")
                                .Replace("доброе", "")
                                .Replace("дорогие коллеги", "")
                                .Replace("здравствуйте", "")
                                .Replace("екатерина", "")
                                .Replace("ирина", "")
                                .Replace("коллеги", "")
                                .Replace("конечно", "")
                                .Replace("марина", "")
                                .Replace("насть", "")
                                .Replace("наталья", "")
                                .Replace("огонь", "")
                                .Replace("огромное", "")
                                .Replace("ольга", "")
                                .Replace("оля", "")
                                .Replace("оплатила", "")
                                .Replace("оплатил", "")
                                .Replace("отлично", "")
                                .Replace("отправила", "")
                                .Replace("отправил", "")
                                .Replace("очень приятно", "")
                                .Replace("подписала", "")
                                .Replace("подписал", "")
                                .Replace("подумаю", "")
                                .Replace("поздравление", "")
                                .Replace("поздравления", "")
                                .Replace("поняла", "")
                                .Replace("понял", "")
                                .Replace("понятно", "")
                                .Replace("попробую", "")
                                .Replace("примем", "")
                                .Replace("приняла", "")
                                .Replace("принял", "")
                                .Replace("принято", "")
                                .Replace("пришлю", "")
                                .Replace("света", "")
                                .Replace("светлана", "")
                                .Replace("сделаю", "")
                                .Replace("спаибо", "")
                                .Replace("спасибо", "")
                                .Replace("спасиб", "")
                                .Replace("спасиьо", "")
                                .Replace("спасио", "")
                                .Replace("спассибо", "")
                                .Replace("спсб", "")
                                .Replace("спс", "")
                                .Replace("спсаибо", "")
                                .Replace("супер", "")
                                .Replace("татьяна", "")
                                .Replace("точно", "")
                                .Replace("увидела", "")
                                .Replace("увидел", "")
                                .Replace("утро", "")
                                .Replace("хорлшо", "")
                                .Replace("хорошо", "")
                                .Replace("шпасиба", "")
                                .Replace("okay", "")
                                .Where(c => c == '?' || !char.IsPunctuation(c)).ToArray())
                        .Trim()
                })
                .Where(z => !ignoreUnanswered.Contains(z.txt));

            return msgsWithDebugData.Select(z => z.msg).ToList();
        }
    }
}