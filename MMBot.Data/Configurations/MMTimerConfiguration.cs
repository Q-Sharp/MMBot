namespace MMBot.Data.Configurations;

public class MMTimerConfiguration : IEntityTypeConfiguration<MMTimer>
{
    public void Configure(EntityTypeBuilder<MMTimer> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
