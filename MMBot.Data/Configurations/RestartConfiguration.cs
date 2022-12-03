namespace MMBot.Data.Configurations;

public class RestartConfiguration : IEntityTypeConfiguration<Restart>
{
    public void Configure(EntityTypeBuilder<Restart> builder) => builder.Property(x => x.Version).IsRowVersion();
}
