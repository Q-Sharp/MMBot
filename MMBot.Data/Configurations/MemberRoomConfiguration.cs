namespace MMBot.Data.Configurations;

public class MemberRoomConfiguration : IEntityTypeConfiguration<MemberRoom>
{
    public void Configure(EntityTypeBuilder<MemberRoom> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();
               
    }
}
