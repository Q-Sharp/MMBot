using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configuration;

public class SeasonResultConfiguration : IEntityTypeConfiguration<SeasonResult>
{
    public void Configure(EntityTypeBuilder<SeasonResult> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
