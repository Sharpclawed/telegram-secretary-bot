﻿using Telegram.Bot.Types;

namespace TelegramBotTry1.Settings
{
    public static class ChatIds
    {
        public static readonly ChatId Debug = new ChatId(Secrets.DebugChatId);
        public static readonly ChatId Botva = new ChatId(Secrets.BotvaChatId);
        public static readonly ChatId Unanswered = new ChatId(Secrets.UnasweredChatId);
    }
}