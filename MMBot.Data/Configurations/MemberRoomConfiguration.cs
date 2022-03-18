using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MMBot.Data.Contracts.Entities;

namespace MMBot.Data.Configurations;

public class MemberRoomConfiguration : IEntityTypeConfiguration<MemberRoom>
{
    public void Configure(EntityTypeBuilder<MemberRoom> builder)
    {
        builder.UseXminAsConcurrencyToken()
               .HasKey(c => c.Id);
    }
}
