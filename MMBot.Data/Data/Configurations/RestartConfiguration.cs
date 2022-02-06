using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configuration;

public class RestartConfiguration : IEntityTypeConfiguration<Restart>
{
    public void Configure(EntityTypeBuilder<Restart> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
