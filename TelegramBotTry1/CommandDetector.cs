using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1
{
    public static class CommandDetector
    {
        public static IBotCommand Parse(string messageText, long? userId = null, string userName = null)
        {
            messageText = messageText.ToLower();
            var patterns = new List<string>
            {
                @"^[/](addadmin)[:]\s*(.*)$",
                @"^[/](removeadmin)[:]\s*(.*)$",
                @"^[/](viewadmins)$",
                @"^[/](addbk)[:]\s*(.*)$",
                @"^[/](removebk)[:]\s*(.*)$",
                @"^[/](viewbk)$",
                @"^[/](viewwaiters)$",
                @"^[/](viewinactivechats)$",
                @"^[/](addonetimechat)[:]\s*(.*)$",
                @"^[/](removeonetimechat)[:]\s*(.*)$",
                @"^[/](viewonetimechats)$",
                @"^[/](history)[:]\s*(.*)\s\b(\d{2}[.]\d{2}[.]\d{4})[ ](\d+)$",
                @"^[/](historyof)[:]\s*(.*)\s\b(\d{2}[.]\d{2}[.]\d{4})[ ](\d+)$",
                @"^[/](historyall)[:]\s*\b(\d{2}[.]\d{2}[.]\d{4})[ ](\d+)$",
            };

            var resultPattern = new Regex(string.Join("|", patterns));
            var match = resultPattern.Match(messageText);
            if (!match.Success)
                throw new InvalidCastException();

            var exactMatch = match.Groups.Cast<Group>().Skip(1).First(g => g.Success);
            var patternPosition = int.Parse(exactMatch.Name);
            var commandText = exactMatch.Value;

            switch (commandText)
            {
                case "viewadmins":
                    return new ViewAdminsCommand();
                case "addadmin":
                    var addAdminName = match.Groups[patternPosition + 1].Value;
                    return new AddAdminCommand(addAdminName, userId.Value, userName);
                case "removeadmin":
                    var removeAdminName = match.Groups[patternPosition + 1].Value;
                    return new RemoveAdminCommand(removeAdminName, userId.Value, userName);
                case "viewbk":
                    return new ViewBkCommand();
                case "addbk":
                    var addBkName = match.Groups[patternPosition + 1].Value;
                    return new AddBkCommand(addBkName);
                case "removebk":
                    var removeBkName = match.Groups[patternPosition + 1].Value;
                    return new RemoveBkCommand(removeBkName);
                case "viewwaiters":
                    return new ViewWaitersCommand();
                case "viewinactivechats":
                    return new ViewInactiveChatsCommand();
                case "viewonetimechats":
                    return new ViewOneTimeChatsCommand();
                case "addonetimechat":
                    var addOnetimechatName = match.Groups[patternPosition + 1].Value;
                    return new AddOnetimeChatCommand(addOnetimechatName);
                case "removeonetimechat":
                    var removeOnetimechatName = match.Groups[patternPosition + 1].Value;
                    return new RemoveOnetimeChatCommand(removeOnetimechatName);
                case "history":
                    var historyChatName = match.Groups[patternPosition + 1].Value;
                    var historyBegin = DateTime.ParseExact(match.Groups[patternPosition + 2].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyEnd = historyBegin.AddDays(double.Parse(match.Groups[patternPosition + 3].Value));
                    return new ViewHistoryCommand(historyBegin, historyEnd, historyChatName);
                case "historyof":
                    var historyUserName = match.Groups[patternPosition + 1].Value;
                    var historyofBegin = DateTime.ParseExact(match.Groups[patternPosition + 2].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyofEnd = historyofBegin.AddDays(double.Parse(match.Groups[patternPosition + 3].Value));
                    return new ViewHistoryOfCommand(historyofBegin, historyofEnd, historyUserName);
                case "historyall":
                    var historyallBegin = DateTime.ParseExact(match.Groups[patternPosition + 1].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyallEnd = historyallBegin.AddDays(double.Parse(match.Groups[patternPosition + 2].Value));
                    return new ViewHistoryAllCommand(historyallBegin, historyallEnd);
            }

            throw new InvalidCastException();
        }
    }
}