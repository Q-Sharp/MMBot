namespace MMBot.Data.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        _ = builder.Property(x => x.Version).IsRowVersion();

        _ = builder.HasMany(v => v.Vacation)
            .WithOne(m => m.Member)
            .HasForeignKey(v => v.MemberId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        _ = builder.HasMany(v => v.Strike)
            .WithOne(m => m.Member)
            .HasForeignKey(v => v.MemberId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        _ = builder.HasMany(p => p.SeasonResult)
          .WithOne(p => p.Member)
          .HasForeignKey(v => v.MemberId)
          .OnDelete(DeleteBehavior.ClientCascade);

        _ = builder.HasMany(m => m.RaidParticipation)
            .WithOne(m => m.Member)
            .HasForeignKey(x => x.RaidParticipationId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
