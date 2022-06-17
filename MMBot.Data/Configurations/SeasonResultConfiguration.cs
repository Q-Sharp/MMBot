namespace MMBot.Data.Configurations;

public class SeasonResultConfiguration : IEntityTypeConfiguration<SeasonResult>
{
    public void Configure(EntityTypeBuilder<SeasonResult> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
