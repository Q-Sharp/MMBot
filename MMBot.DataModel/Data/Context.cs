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
                .HasIndex(c => new { c.Tag, c.GuildId })
                .IsUnique();

            modelBuilder.Entity<Clan>()
                .HasIndex(c => new { c.SortOrder, c.GuildId })
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasIndex(m =>new { m.Name, m.GuildId })
                .IsUnique();

             modelBuilder.Entity<Clan>()
                .HasMany(c => c.Member)
                .WithOne(m => m.Clan)
                .HasForeignKey(m => m.ClanId);

            modelBuilder.Entity<MemberGroup>()
                .HasMany(m => m.Members)
                .WithOne(m => m.MemberGroup)
                .HasForeignKey(x => x.MemberGroupId);

            modelBuilder.Entity<Member>()
                .HasMany(v => v.Vacation)
                .WithOne(m => m.Member)
                .HasForeignKey(v => v.MemberId);

            modelBuilder.Entity<Member>()
                .HasMany(v => v.Strikes)
                .WithOne(m => m.Member)
                .HasForeignKey(v => v.MemberId);

            modelBuilder.Entity<GuildSettings>()
                .HasIndex(m => m.GuildId)
                .IsUnique();
        }

        public async Task MigrateAsync() => await Database.MigrateAsync();
    }
}
