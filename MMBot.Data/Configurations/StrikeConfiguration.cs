using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Contracts.Entities;

namespace MMBot.Data.Configurations;

public class StrikeConfiguration : IEntityTypeConfiguration<Strike>
{
    public void Configure(EntityTypeBuilder<Strike> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
