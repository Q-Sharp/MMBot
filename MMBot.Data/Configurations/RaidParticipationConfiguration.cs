namespace MMBot.Data.Configurations;

public class RaidParticipationConfiguration : IEntityTypeConfiguration<RaidParticipation>
{
    public void Configure(EntityTypeBuilder<RaidParticipation> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
