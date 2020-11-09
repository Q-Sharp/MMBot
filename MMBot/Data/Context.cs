using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMBot.Data.Entities;

namespace MMBot.Data
{
    public class Context : DbContext
    {
        private readonly string _dbname = $"{Path.Combine(Directory.GetCurrentDirectory(), "MMBot.db")}";
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite($"Data Source={_dbname}");

        public DbSet<Member> Member { get; set; }
        public DbSet<Clan> Clan { get; set; }
        public DbSet<GuildSettings> GuildSettings { get; set; }
        public DbSet<Restart> Restart { get; set; }
        public DbSet<Vacation> Vacation { get; set; }
        public DbSet<Channel> Channel { get; set; }
        public DbSet<MemberGroup> MemberGroup { get; set; }
        public DbSet<MMTimer> Timer { get; set; }
        public DbSet<Strike> Strike { get; set; }
        
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

            modelBuilder.Entity<Strike>()
                .HasOne(v => v.Member)
                .WithMany(m => m.Strikes)
                .HasForeignKey(v => v.MemberId);

            modelBuilder.Entity<Clan>()
                .HasIndex(c => c.SortOrder)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasOne(m => m.MemberGroup)
                .WithMany(mg => mg.Members)
                .HasForeignKey(m => m.MemberGroupId);

            modelBuilder.Entity<GuildSettings>()
                .HasIndex(m => m.GuildId)
                .IsUnique();
        }

        public async Task MigrateAsync() => await Database.MigrateAsync();
    }
}
