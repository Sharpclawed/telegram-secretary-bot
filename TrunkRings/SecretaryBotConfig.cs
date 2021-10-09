namespace TrunkRings
{
    public class SecretaryBotConfig : ISecretaryBotConfig
    {
        public string ConnectionString { get; set; }
    }

    public interface ISecretaryBotConfig
    {
        string ConnectionString { get; }
    }
}
