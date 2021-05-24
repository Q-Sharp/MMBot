using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configuration
{
    public class RaidParticipationConfiguration : IEntityTypeConfiguration<RaidParticipation>
    {
        public void Configure(EntityTypeBuilder<RaidParticipation> builder)
        {
            builder.UseXminAsConcurrencyToken()
                   .HasKey(c => c.Id);
        }
    }
}
