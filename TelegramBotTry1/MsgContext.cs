using System.Data.Entity;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Migrations;
using TelegramBotTry1.SecretsStore;

namespace TelegramBotTry1
{
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class MsgContext : DbContext
    {
        public MsgContext() : base(Secrets.DbConnectionString)
        {
        }

        public DbSet<MessageDataSet> MessageDataSets { get; set; }
        public DbSet<AdminDataSet> AdminDataSets { get; set; }
        public DbSet<BookkeeperDataSet> BookkeeperDataSets { get; set; }
        public DbSet<OnetimeChatDataSet> OnetimeChatDataSets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("public");
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MsgContext, Configuration>());
        }
    }
}
