using System.Collections.Generic;
using Telegram.Bot.Types;

namespace TrunkRings
{
    public class SecretaryBotConfig : ISecretaryBotConfig
    {
        public string ConnectionString { get; set; }
        public ChatId DebugChatId { get; set; }
        public ChatId UnansweredChatId { get; set; }
        public ChatId LogDistributingChatId { get; set; }
    }

    public interface ISecretaryBotConfig
    {
        string ConnectionString { get; }
        ChatId DebugChatId { get; set; }
        ChatId UnansweredChatId { get; set; }
        ChatId LogDistributingChatId { get; set; }
    }
}
