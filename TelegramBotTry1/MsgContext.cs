using System.Data.Entity;
using System.Data.Entity.Migrations;
using TelegramBotTry1.Domain;
using TelegramBotTry1.Migrations;

namespace TelegramBotTry1
{
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class MsgContext : DbContext
    {
        private static string ConnectionString => "Database=TelegramHistory2;Password=postgres;User ID=postgres;Server=localhost";

        public MsgContext() : base(ConnectionString)
        {
        }

        public DbSet<MessageDataSet> MessageDataSets { get; set; }
        public DbSet<AdminDataSet> AdminDataSets { get; set; }
        public DbSet<BookkeeperDataSet> BookkeeperDataSets { get; set; }
        public DbSet<OnetimeChatDataSet> OnetimeChatDataSets { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema("public");
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MsgContext, Configuration>());
        }
    }
}
