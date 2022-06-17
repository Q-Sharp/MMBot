namespace MMBot.Data.Configurations;

public class RestartConfiguration : IEntityTypeConfiguration<Restart>
{
    public void Configure(EntityTypeBuilder<Restart> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
