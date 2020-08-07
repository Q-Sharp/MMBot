using System.IO;
using Microsoft.EntityFrameworkCore;
using TTMMBot.Data.Entities;

namespace TTMMBot.Data
{
    public class Context : DbContext, ITTMMBotContext
    {
        private readonly string dbname = $"{Path.Combine(Directory.GetCurrentDirectory(), "TTMMBot.db")}";
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={dbname}");

        public DbSet<Member> Member { get; set; }
        public DbSet<Clan> Clan { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clan>()
                .HasKey(m => m.ClanID);

            modelBuilder.Entity<Clan>()
                .HasMany(rp => rp.Members)
                .WithOne(c => c.Clan)
                .HasForeignKey(rp => rp.ClanID);

            modelBuilder.Entity<Clan>()
                .HasIndex(c => c.Tag)
                .IsUnique();

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
