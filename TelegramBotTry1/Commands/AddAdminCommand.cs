﻿using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Telegram.Bot.Types;

namespace TelegramBotTry1.Commands
{
    public class AddAdminCommand : IBotCommand
    {
        private readonly IAdminService adminService;
        private readonly ITgBotClientEx tgClient;
        private readonly ChatId chatId;
        public string AdminName { get; }
        public long AddedUserId { get; }
        public string AddedUserName { get; }

        public AddAdminCommand(IAdminService adminService, ITgBotClientEx tgClient, ChatId chatId, string adminName, long addedUserId, string addedUsername)
        {
            this.adminService = adminService;
            this.tgClient = tgClient;
            this.chatId = chatId;
            AdminName = adminName;
            AddedUserId = addedUserId;
            AddedUserName = addedUsername;
        }

        public async Task ProcessAsync()
        {
            var addedBy = new Admin{UserId = AddedUserId, UserName = AddedUserName };
            adminService.Make(AdminName, addedBy);

            var result = "Команда обработана";
            await tgClient.SendTextMessageAsync(chatId, result);
        }
    }
}