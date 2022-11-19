namespace MMBot.Data.Configurations;

public class RaidParticipationConfiguration : IEntityTypeConfiguration<RaidParticipation>
{
    public void Configure(EntityTypeBuilder<RaidParticipation> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();
               
    }
}
