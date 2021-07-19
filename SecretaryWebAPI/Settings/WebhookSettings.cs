using TelegramBotTry1.Settings;

namespace SecretaryWebAPI.Settings
{
    public static partial class WebhookSettings
    {
        public static string Url { get; }
        public static string BotToken => Secrets.MainBotToken;
    }
}
