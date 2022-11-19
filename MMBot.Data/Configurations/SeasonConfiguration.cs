namespace MMBot.Data.Configurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();
               

        builder
           .HasMany(s => s.SeasonResult)
           .WithOne(s => s.Season)
           .HasForeignKey(s => s.SeasonId);
    }
}
