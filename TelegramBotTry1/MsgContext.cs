using System.Data.Entity;

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
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // PostgreSQL uses the public schema by default - not dbo.
            modelBuilder.HasDefaultSchema("public");
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<MsgContext, Configuration>());
        }
    }
    //public sealed class Configuration : DbMigrationsConfiguration<MsgContext>
    //{
    //    public Configuration()
    //    {
    //        AutomaticMigrationsEnabled = true;
    //    }

    //    protected override void Seed(MsgContext context)
    //    {

    //    }
    //}
}
