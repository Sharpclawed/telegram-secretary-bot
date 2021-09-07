using Telegram.Bot.Types;

namespace TrunkRings.Settings
{
    public static class ChatIds
    {
        public static readonly ChatId Debug = new ChatId(Secrets.DebugChatId);
        public static readonly ChatId Botva = new ChatId(Secrets.BotvaChatId);
        public static readonly ChatId Unanswered = new ChatId(Secrets.UnasweredChatId);
        public static readonly ChatId LogDistributing = new ChatId(Secrets.LogChatId);
    }
}