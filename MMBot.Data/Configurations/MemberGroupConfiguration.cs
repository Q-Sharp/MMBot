namespace MMBot.Data.Configurations;

public class MemberGroupConfiguration : IEntityTypeConfiguration<MemberGroup>
{
    public void Configure(EntityTypeBuilder<MemberGroup> builder)
    {
        _ = builder.Property(x => x.Version).IsRowVersion();

        _ = builder.HasMany(m => m.Members)
            .WithOne(m => m.MemberGroup)
            .HasForeignKey(x => x.MemberGroupId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
