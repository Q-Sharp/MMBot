using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMBot.Data.Entities;
using MMBot.Data.Interfaces;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace MMBot.Data
{
    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if(!options.IsConfigured)
                options.UseNpgsql($@"Server=127.0.0.1;Port=5433;Database=MMBotDB;Username=postgres;Password=P0stGresSQL2021");
        }

        public Context()
        {

        }

        public Context(DbContextOptions<Context> options = null) : base(options)
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
            modelBuilder.UseIdentityAlwaysColumns();

            modelBuilder.Entity<Channel>()
                .UseXminAsConcurrencyToken()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Clan>()
                .UseXminAsConcurrencyToken()
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
                .UseXminAsConcurrencyToken()
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
                .UseXminAsConcurrencyToken()
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
                .UseXminAsConcurrencyToken()
                .HasKey(c => c.Id);

            modelBuilder.Entity<MMTimer>()
                .UseXminAsConcurrencyToken()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Restart>()
                .UseXminAsConcurrencyToken()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Strike>()
                .UseXminAsConcurrencyToken()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Vacation>()
                .UseXminAsConcurrencyToken()
                .HasKey(c => c.Id);
        }

        public async Task MigrateAsync() => await Database.MigrateAsync();
    }
}
