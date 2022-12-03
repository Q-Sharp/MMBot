namespace MMBot.Data.Configurations;

public class VacationConfiguration : IEntityTypeConfiguration<Vacation>
{
    public void Configure(EntityTypeBuilder<Vacation> builder) => builder.Property(x => x.Version).IsRowVersion();
}
