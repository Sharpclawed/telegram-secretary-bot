﻿using Microsoft.EntityFrameworkCore;
using TrunkRings.DAL.Models;
using TrunkRings.DAL.Settings;

namespace TrunkRings.DAL
{
    public sealed class SecretaryContext : DbContext
    {
        public DbSet<MessageDataSet> MessageDataSets { get; set; }
        public DbSet<AdminDataSet> AdminDataSets { get; set; }
        public DbSet<BookkeeperDataSet> BookkeeperDataSets { get; set; }
        public DbSet<OnetimeChatDataSet> OnetimeChatDataSets { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("public");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(DbSettings.DbConnectionString);

        public void SetConnectionString(string connectionString)
        {
            DbSettings.DbConnectionString = connectionString;
        }
    }
}
