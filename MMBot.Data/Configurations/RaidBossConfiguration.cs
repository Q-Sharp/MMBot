namespace MMBot.Data.Configurations;

public class RaidBossConfiguration : IEntityTypeConfiguration<RaidBoss>
{
    public void Configure(EntityTypeBuilder<RaidBoss> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();

        builder.HasMany(m => m.RaidParticipation)
            .WithOne(m => m.RaidBoss)
            .HasForeignKey(x => x.RaidParticipationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
