namespace MMBot.Data.Contracts.Entities;

public class Restart : IHaveId
{
    public int Id { get; set; }

    public ulong Guild { get; set; }
    public ulong Channel { get; set; }

    public bool DBImport { get; set; }

    public byte[] Version { get; set; }

    public void Update(object restart)
    {
        if (restart is Restart r && Id == r.Id)
        {
            Guild = r.Guild;
            Channel = r.Channel;
        }
    }
}
