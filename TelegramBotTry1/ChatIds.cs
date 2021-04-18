using Telegram.Bot.Types;

namespace TelegramBotTry1
{
    public static class ChatIds
    {
        private static readonly long Test125Id = -1001448532640; //чат 125
        private static readonly long BotvaId = -1001100176543;   //чат БотВажное
        private static readonly long UnasweredId = -1001469821060;

        public static readonly ChatId Test125 = new ChatId(Test125Id);
        public static readonly ChatId Botva = new ChatId(BotvaId);
        public static readonly ChatId Unanswered = new ChatId(UnasweredId);
    }
}