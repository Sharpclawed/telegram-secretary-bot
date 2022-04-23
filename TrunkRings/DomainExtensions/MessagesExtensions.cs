using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TrunkRings.Domain.Models;

namespace TrunkRings.DomainExtensions
{
    public static class MessagesExtensions
    {
        public static IEnumerable<DomainMessage> FilterObviouslySuperfluous(this IEnumerable<DomainMessage> dataSets)
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

            var msgsWithDebugData = dataSets.Select(m => new
                {
                    msg = m,
                    txt = new string(
                            Regex.Replace(m.Message, @"\p{Cs}", " ")
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
                                .Replace("спсаибо", "")
                                .Replace("спсибо", "")
                                .Replace("спс", "")
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
                .Where(x => !ignoreUnanswered.Contains(x.txt));

            return msgsWithDebugData.Select(x => x.msg).ToList();
        }
    }
}