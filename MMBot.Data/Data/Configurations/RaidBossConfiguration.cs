using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configuration;

public class RaidBossConfiguration : IEntityTypeConfiguration<RaidBoss>
{
    public void Configure(EntityTypeBuilder<RaidBoss> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);

        builder.HasMany(m => m.RaidParticipation)
            .WithOne(m => m.RaidBoss)
            .HasForeignKey(x => x.RaidParticipationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
