namespace MMBot.Data.Configurations;

public class SeasonResultConfiguration : IEntityTypeConfiguration<SeasonResult>
{
    public void Configure(EntityTypeBuilder<SeasonResult> builder) => builder.Property(x => x.Version).IsRowVersion();
}
