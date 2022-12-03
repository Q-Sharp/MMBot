namespace MMBot.Data.Configurations;

public class MMTimerConfiguration : IEntityTypeConfiguration<MMTimer>
{
    public void Configure(EntityTypeBuilder<MMTimer> builder) => builder.Property(x => x.Version).IsRowVersion();
}
