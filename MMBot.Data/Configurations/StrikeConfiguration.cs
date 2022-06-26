namespace MMBot.Data.Configurations;

public class StrikeConfiguration : IEntityTypeConfiguration<Strike>
{
    public void Configure(EntityTypeBuilder<Strike> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
