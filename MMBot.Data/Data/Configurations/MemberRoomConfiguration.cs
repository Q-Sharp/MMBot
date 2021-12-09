using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Entities;

namespace MMBot.Data.Configuration;

public class MemberRoomConfiguration : IEntityTypeConfiguration<MemberRoom>
{
    public void Configure(EntityTypeBuilder<MemberRoom> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
