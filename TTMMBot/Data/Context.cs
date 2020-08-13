using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using TTMMBot.Data.Entities;

namespace TTMMBot.Data
{
    public class Context : DbContext, ITTMMBotContext
    {
        private readonly string dbname = $"{Path.Combine(Directory.GetCurrentDirectory(), "TTMMBot.db")}";
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options
            .UseSqlite($"Data Source={dbname}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
            .UseLazyLoadingProxies();

        public DbSet<Member> Member { get; set; }
        public DbSet<Clan> Clan { get; set; }
        public DbSet<Vacation> Vacation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clan>()
                .HasIndex(c => c.Tag)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Name)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasOne(m => m.Clan)
                .WithMany(c => c.Member)
                .HasForeignKey(m => m.ClanID);

            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.Member)
                .WithMany(m => m.Vacation)
                .HasForeignKey(v => v.MemberID);
        }

        public async Task MigrateAsync() => await Database.MigrateAsync();
    }
}
