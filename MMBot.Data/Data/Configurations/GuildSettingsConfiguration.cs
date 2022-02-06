using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configuration;

public class GuildSettingsConfiguration : IEntityTypeConfiguration<GuildSettings>
{
    public void Configure(EntityTypeBuilder<GuildSettings> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
