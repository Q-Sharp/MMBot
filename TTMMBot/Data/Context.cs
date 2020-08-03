using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using TTMMBot.Data.Entities;

namespace TTMMBot.Data
{
    public class Context : DbContext, ITTMMBotContext
    {
        private string dbname = $"{Path.Combine(Directory.GetCurrentDirectory(), "TTMMBot.db")}";

        public Context(DbContextOptions<Context> options = null) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={dbname}");

        public DbSet<Member> Member { get; set; }
        public DbSet<Clan> Clan { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clan>()
                .HasKey(m => m.Tag);

            modelBuilder.Entity<Clan>()
                .HasMany(rp => rp.Members)
                .WithOne(c => c.Clan)
                .HasForeignKey(rp => rp.ClanTag);

            modelBuilder.Entity<Member>()
                .HasKey(m => m.MemberID);

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Name)
                .IsUnique();

            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.Member)
                .WithOne(m => m.Vacation);
        }
    }
}
