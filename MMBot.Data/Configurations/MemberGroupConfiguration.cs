using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configurations;

public class MemberGroupConfiguration : IEntityTypeConfiguration<MemberGroup>
{
    public void Configure(EntityTypeBuilder<MemberGroup> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);

        builder.HasMany(m => m.Members)
            .WithOne(m => m.MemberGroup)
            .HasForeignKey(x => x.MemberGroupId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}
