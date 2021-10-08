using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TrunkRings.Domain.Services;
using Telegram.Bot.Types;
using TrunkRings.Commands;

namespace TrunkRings
{
    class CommandDetector
    {
        private readonly ITgBotClientEx tgClient;
        private readonly IAdminService adminService;
        private readonly IBkService bkService;
        private readonly IOneTimeChatService oneTimeChatService;
        private readonly IMessageService messageService;

        public CommandDetector(ITgBotClientEx tgClient, IAdminService adminService, IBkService bkService, IOneTimeChatService oneTimeChatService, IMessageService messageService)
        {
            this.tgClient = tgClient;
            this.adminService = adminService;
            this.bkService = bkService;
            this.oneTimeChatService = oneTimeChatService;
            this.messageService = messageService;
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
                @"^[/](ping)$",
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
                    return new ViewAdminsCommand(adminService, tgClient, chatId);
                case "addadmin":
                    var addAdminName = match.Groups[patternPosition + 1].Value;
                    return new AddAdminCommand(adminService, tgClient, chatId, addAdminName, message.From.Id, message.From.Username);
                case "removeadmin":
                    var removeAdminName = match.Groups[patternPosition + 1].Value;
                    return new RemoveAdminCommand(adminService, tgClient, chatId, removeAdminName, message.From.Id, message.From.Username);
                case "viewbk":
                    return new ViewBkCommand(bkService, tgClient, chatId);
                case "addbk":
                    var addBkName = match.Groups[patternPosition + 1].Value;
                    return new AddBkCommand(bkService, tgClient, chatId, addBkName);
                case "removebk":
                    var removeBkName = match.Groups[patternPosition + 1].Value;
                    return new RemoveBkCommand(bkService, tgClient, chatId, removeBkName);
                case "viewwaiters":
                    return new ViewWaitersCommand(messageService, tgClient, chatId);
                case "viewinactivechats":
                    return new ViewInactiveChatsCommand(messageService, tgClient, chatId);
                case "viewonetimechats":
                    return new ViewOneTimeChatsCommand(oneTimeChatService, tgClient, chatId);
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
                    return new ViewHistoryCommand(messageService, tgClient, chatId, historyBegin, historyEnd, historyChatName);
                case "historyof":
                    var historyUserId = match.Groups[patternPosition + 1].Value;
                    var historyofBegin = DateTime.ParseExact(match.Groups[patternPosition + 2].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyofEnd = historyofBegin.AddDays(double.Parse(match.Groups[patternPosition + 3].Value));
                    return new ViewHistoryOfCommand(messageService, tgClient, chatId, historyofBegin, historyofEnd, long.Parse(historyUserId));
                case "historyall":
                    var historyallBegin = DateTime.ParseExact(match.Groups[patternPosition + 1].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                    var historyallEnd = historyallBegin.AddDays(double.Parse(match.Groups[patternPosition + 2].Value));
                    return new ViewHistoryAllCommand(messageService, tgClient, chatId, historyallBegin, historyallEnd);
                case "help":
                    return new SendHelpTipCommand(tgClient, message.Chat.Id);
                case "ping":
                    return new SendBotStatusCommand(messageService, tgClient, message.Chat.Id);
            }

            return new SendMessageCommand(tgClient, chatId, "Неизвестная команда");
        }
    }
}