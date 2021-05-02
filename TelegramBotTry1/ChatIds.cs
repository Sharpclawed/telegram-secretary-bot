using Telegram.Bot.Types;
using TelegramBotTry1.SecretsStore;

namespace TelegramBotTry1
{
    public static class ChatIds
    {
        public static readonly ChatId Test125 = new ChatId(Secrets.Test125Id);
        public static readonly ChatId Botva = new ChatId(Secrets.BotvaId);
        public static readonly ChatId Unanswered = new ChatId(Secrets.UnasweredId);
    }
}