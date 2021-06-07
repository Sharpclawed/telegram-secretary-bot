using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TelegramBotTry1.Dto;

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
                @"^[/](viewadmins)\s*(.*)$",
                @"^[/](addbk)[:]\s*(.*)$",
                @"^[/](removebk)[:]\s*(.*)$",
                @"^[/](viewbk)\s*(.*)$",
                @"^[/](viewwaiters)\s*(.*)$",
                @"^[/](viewinactivechats)\s*(.*)$",
                @"^[/](addonetimechat)[:]\s*(.*)$",
                @"^[/](removeonetimechat)[:]\s*(.*)$",
                @"^[/](viewonetimechats)\s*(.*)$"
            };

            var resultPattern = new Regex(string.Join("|", patterns));
            var match = resultPattern.Match(messageText);
            if (!match.Success)
                throw new InvalidCastException();

            var patternPosition = int.Parse(match.Groups.Cast<Group>().Skip(1).First(g => g.Success).Name);
            var commandText = match.Groups[patternPosition].Value;
            var entityName = match.Groups[patternPosition + 1].Value;

            switch (commandText)
            {
                case "viewadmins":
                    return new ViewAdminsCommand();
                case "addadmin":
                    return new AddAdminCommand(entityName, userId.Value, userName);
                case "removeadmin":
                    return new RemoveAdminCommand(entityName, userId.Value, userName);
                case "viewbk":
                    return new ViewBkCommand();
                case "addbk":
                    return new AddBkCommand(entityName);
                case "removebk":
                    return new RemoveBkCommand(entityName);
                case "viewwaiters":
                    return new ViewWaitersCommand();
                case "viewinactivechats":
                    return new ViewInactiveChatsCommand();
                case "viewonetimechats":
                    return new ViewOneTimeChatsCommand();
                case "addonetimechat":
                    return new AddOnetimeChatCommand(entityName);
                case "removeonetimechat":
                    return new RemoveOnetimeChatCommand(entityName);
            }

            throw new InvalidCastException();
        }
    }
}