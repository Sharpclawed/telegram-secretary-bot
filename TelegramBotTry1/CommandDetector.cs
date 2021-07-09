using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Services;
using Telegram.Bot.Types;
using TelegramBotTry1.Commands;

namespace TelegramBotTry1
{
    public class CommandDetector
    {
        private readonly ITgBotClientEx tgClient;
        private readonly AdminService adminService;
        private readonly BkService bkService;
        private readonly OneTimeChatService oneTimeChatService;

        public CommandDetector(ITgBotClientEx tgClient, AdminService adminService, BkService bkService, OneTimeChatService oneTimeChatService)
        {
            this.tgClient = tgClient;
            this.adminService = adminService;
            this.bkService = bkService;
            this.oneTimeChatService = oneTimeChatService;
        }

        public IBotCommand Parse(Message message)
        {
            var messageText = message.Text.ToLower();
            var chatId = message.Chat.Id;

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
                @"^[/](help)$",
            };

            var resultPattern = new Regex(string.Join("|", patterns));
            var match = resultPattern.Match(messageText);
            if (!match.Success)
                return new SendMessageCommand(tgClient, chatId, "Неизвестная команда");

            var exactMatch = match.Groups.Cast<Group>().Skip(1).First(g => g.Success);
            var patternPosition = int.Parse(exactMatch.Name);
            var commandText = exactMatch.Value;

            switch (commandText)
            {
                case "viewadmins":
                    return new ViewAdminsCommand(tgClient, chatId);
                case "addadmin":
                    var addAdminName = match.Groups[patternPosition + 1].Value;
                    return new AddAdminCommand(adminService, tgClient, chatId, addAdminName, message.From.Id, message.From.Username);
                case "removeadmin":
                    var removeAdminName = match.Groups[patternPosition + 1].Value;
                    return new RemoveAdminCommand(adminService, tgClient, chatId, removeAdminName, message.From.Id, message.From.Username);
                case "viewbk":
                    return new ViewBkCommand(tgClient, chatId);
                case "addbk":
                    var addBkName = match.Groups[patternPosition + 1].Value;
                    return new AddBkCommand(bkService, tgClient, chatId, addBkName);
                case "removebk":
                    var removeBkName = match.Groups[patternPosition + 1].Value;
                    return new RemoveBkCommand(bkService, tgClient, chatId, removeBkName);
                case "viewwaiters":
                    return new ViewWaitersCommand(tgClient, chatId);
                case "viewinactivechats":
                    return new ViewInactiveChatsCommand(tgClient, chatId);
                case "viewonetimechats":
                    return new ViewOneTimeChatsCommand(tgClient, chatId);
                case "addonetimechat":
                    var addOnetimechatName = match.Groups[patternPosition + 1].Value;
                    return new AddOnetimeChatCommand(oneTimeChatService, tgClient, chatId, addOnetimechatName);
                case "removeonetimechat":
                    var removeOnetimechatName = match.Groups[patternPosition + 1].Value;
                    return new RemoveOnetimeChatCommand(oneTimeChatService, tgClient, chatId, removeOnetimechatName);
                case "history":
                    var historyChatName = match.Groups[patternPosition + 1].Value;
                    var historyBegin = DateTime.ParseExact(match.Groups[patternPosition + 2].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyEnd = historyBegin.AddDays(double.Parse(match.Groups[patternPosition + 3].Value));
                    return new ViewHistoryCommand(tgClient, chatId, historyBegin, historyEnd, historyChatName);
                case "historyof":
                    var historyUserId = match.Groups[patternPosition + 1].Value;
                    var historyofBegin = DateTime.ParseExact(match.Groups[patternPosition + 2].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyofEnd = historyofBegin.AddDays(double.Parse(match.Groups[patternPosition + 3].Value));
                    return new ViewHistoryOfCommand(tgClient, chatId, historyofBegin, historyofEnd, long.Parse(historyUserId));
                case "historyall":
                    var historyallBegin = DateTime.ParseExact(match.Groups[patternPosition + 1].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyallEnd = historyallBegin.AddDays(double.Parse(match.Groups[patternPosition + 2].Value));
                    return new ViewHistoryAllCommand(tgClient, chatId, historyallBegin, historyallEnd);
                case "help":
                    return new SendHelpTipCommand(tgClient, message.Chat.Id);
            }

            return new SendMessageCommand(tgClient, chatId, "Неизвестная команда");
        }
    }
}