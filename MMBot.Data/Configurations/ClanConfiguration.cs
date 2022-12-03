namespace MMBot.Data.Configurations;

public class ClanConfiguration : IEntityTypeConfiguration<Clan>
{
    public void Configure(EntityTypeBuilder<Clan> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();

        builder.HasIndex(c => new { c.Tag, c.Name, c.GuildId, c.SortOrder })
               .IsUnique();

        builder.HasMany(c => c.Member)
               .WithOne(m => m.Clan)
               .HasForeignKey(m => m.ClanId)
               .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasMany(c => c.RaidBoss)
               .WithOne(c => c.Clan)
               .HasForeignKey(c => c.ClanId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
