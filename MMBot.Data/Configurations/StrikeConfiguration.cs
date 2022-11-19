namespace MMBot.Data.Configurations;

public class StrikeConfiguration : IEntityTypeConfiguration<Strike>
{
    public void Configure(EntityTypeBuilder<Strike> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();
               
    }
}
