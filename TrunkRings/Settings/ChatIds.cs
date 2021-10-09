using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TrunkRings.Settings
{
    public static class ChatIds
    {
        public static ChatId Debug { get; set; }
        public static ChatId Unanswered { get; set; }
        public static ChatId LogDistributing { get; set; }
        public static List<long> AllowedForDistribution { get; set; }
    }
}