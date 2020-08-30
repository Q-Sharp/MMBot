using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TTMMBot.Data.Entities;

namespace TTMMBot.Data
{
    public class Context : DbContextWithTriggers, IContext
    {
        private readonly string _dbname = $"{Path.Combine(Directory.GetCurrentDirectory(), "TTMMBot.db")}";
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options
            .UseSqlite($"Data Source={_dbname}")
            .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll)
            .UseLazyLoadingProxies();

        public bool UseTriggers { get; set; } = true;
        public DbSet<Member> Member { get; set; }
        public DbSet<Clan> Clan { get; set; }
        public DbSet<GlobalSettings> GlobalSettings { get; set; }
        public DbSet<Restart> Restart { get; set; }
        public DbSet<Vacation> Vacation { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Clan>()
                .HasIndex(c => c.Tag)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Name)
                .IsUnique();

            //modelBuilder.Entity<Member>()
            //    .HasIndex(m => new { m.JoinOrder, m.ClanID })
            //    .IsUnique();

            modelBuilder.Entity<Member>()
                .HasOne(m => m.Clan)
                .WithMany(c => c.Member)
                .HasForeignKey(m => m.ClanId);

            modelBuilder.Entity<Vacation>()
                .HasOne(v => v.Member)
                .WithMany(m => m.Vacation)
                .HasForeignKey(v => v.MemberId);
        }

        public async Task MigrateAsync() => await Database.MigrateAsync();
    }
}
