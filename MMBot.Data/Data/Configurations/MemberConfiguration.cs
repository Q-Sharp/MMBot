using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configuration
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.UseXminAsConcurrencyToken()
                   .HasKey(c => c.Id);

            builder.HasMany(v => v.Vacation)
                .WithOne(m => m.Member)
                .HasForeignKey(v => v.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(v => v.Strike)
                .WithOne(m => m.Member)
                .HasForeignKey(v => v.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasMany(p => p.SeasonResult)
              .WithOne(p => p.Member)
              .HasForeignKey(v => v.MemberId)
              .OnDelete(DeleteBehavior.ClientCascade);

             builder.HasMany(m => m.RaidParticipation)
                 .WithOne(m => m.Member)
                 .HasForeignKey(x => x.RaidParticipationId)
                 .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
