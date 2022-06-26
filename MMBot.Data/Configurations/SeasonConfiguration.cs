namespace MMBot.Data.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);

        builder
           .HasMany(s => s.SeasonResult)
           .WithOne(s => s.Season)
           .HasForeignKey(s => s.SeasonId);
    }
}
