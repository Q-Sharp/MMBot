using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using TTMMBot.Data.Entities;

namespace TTMMBot.Data
{
    public class Context : DbContext, ITTMMBotContext
    {
        private const string dbname = "TTMMBot.db";

        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={dbname}");

        public DbSet<Member> Member { get; set; }
        public DbSet<Clan> Clan { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Member>()
                .HasOne(p => p.Clan)
                .WithMany(rp => rp.Members)
                .HasForeignKey(p => p.ClanID);

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Name)
                .IsUnique();

            modelBuilder.Entity<Clan>()
                .HasIndex("Tag", "Name")
                .IsUnique(true);

            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.Member)
                .WithOne(m => m.Vacation);
        }
    }
}
