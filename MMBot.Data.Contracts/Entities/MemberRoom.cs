namespace MMBot.Data.Contracts.Entities;

public class MemberRoom : IHaveId, IHaveGuildId
{
    public int Id { get; set; }

    public ulong GuildId { get; set; }

    [Required]
    [Display]
    public string Name { get; set; }

    [Required]
    [Display]
    public ulong RoomId { get; set; }

    public ulong UserId { get; set; }

    public byte[] Version { get; set; }
    public object Update(object mr)
    {
        if (mr is MemberRoom room && Id == room.Id)
        {
            GuildId = room.GuildId;
            Name = room.Name;
            RoomId = room.RoomId;
            UserId = room.UserId;
        }
        return this;
    }
}
