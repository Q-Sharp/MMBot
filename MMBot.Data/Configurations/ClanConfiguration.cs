namespace MMBot.Data.Configurations;

public class ClanConfiguration : IEntityTypeConfiguration<Clan>
{
    public void Configure(EntityTypeBuilder<Clan> builder)
    {
        _ = builder.Property(x => x.Version).IsRowVersion();

        _ = builder.HasIndex(c => new { c.Tag, c.Name, c.GuildId, c.SortOrder })
               .IsUnique();

        _ = builder.HasMany(c => c.Member)
               .WithOne(m => m.Clan)
               .HasForeignKey(m => m.ClanId)
               .OnDelete(DeleteBehavior.ClientSetNull);

        _ = builder.HasMany(c => c.RaidBoss)
               .WithOne(c => c.Clan)
               .HasForeignKey(c => c.ClanId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
