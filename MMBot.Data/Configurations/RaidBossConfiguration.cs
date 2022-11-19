namespace MMBot.Data.Configurations;

public class RaidBossConfiguration : IEntityTypeConfiguration<RaidBoss>
{
    public void Configure(EntityTypeBuilder<RaidBoss> builder)
    {
        _ = builder.Property(x => x.Version).IsRowVersion();

        _ = builder.HasMany(m => m.RaidParticipation)
            .WithOne(m => m.RaidBoss)
            .HasForeignKey(x => x.RaidParticipationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
