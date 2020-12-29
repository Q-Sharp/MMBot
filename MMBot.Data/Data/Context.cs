using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMBot.Data.Entities;
using MMBot.Data.Interfaces;

namespace MMBot.Data
{
    public class Context : DbContext
    {
        private readonly string _dbname = $"{Path.Combine(Directory.GetCurrentDirectory(), "MMBot.db")}";
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if(!options.IsConfigured)
                options.UseSqlite($"Data Source={_dbname}");
        }

        public Context() : base()
        {
        }

        public Context(DbContextOptions opt) : base(opt)
        {
        }

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
            modelBuilder.Entity<Channel>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Clan>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Clan>()
                .HasIndex(c => new { c.Tag, c.Name, c.GuildId, c.SortOrder })
                .IsUnique();

            modelBuilder.Entity<Clan>()
                .HasMany(c => c.Member)
                .WithOne(m => m.Clan)
                .HasForeignKey(m => m.ClanId)
                .OnDelete(DeleteBehavior.ClientSetNull);

             modelBuilder.Entity<Member>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Member>()
                .HasMany(v => v.Vacation)
                .WithOne(m => m.Member)
                .HasForeignKey(v => v.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<Member>()
                .HasMany(v => v.Strikes)
                .WithOne(m => m.Member)
                .HasForeignKey(v => v.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<MemberGroup>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<MemberGroup>()
                .HasMany(m => m.Members)
                .WithOne(m => m.MemberGroup)
                .HasForeignKey(x => x.MemberGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<GuildSettings>()
                .HasIndex(m => m.GuildId)
                .IsUnique();

            modelBuilder.Entity<GuildSettings>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<MMTimer>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Restart>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Strike>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Vacation>()
                .HasKey(c => c.Id);
        }

        public async Task MigrateAsync() => await Database.MigrateAsync();
    }
}
