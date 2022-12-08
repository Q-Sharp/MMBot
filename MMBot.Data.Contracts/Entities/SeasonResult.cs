namespace MMBot.Data.Contracts.Entities;

public class SeasonResult : IHaveId, IHaveGuildId
{
    public int Id { get; set; }
    public int No { get; set; }
    public ulong GuildId { get; set; }
    public int MemberId { get; set; }
    public int SeasonId { get; set; }

    [Display]
    public int? SHigh { get; set; }

    [Display]
    public int? Donations { get; set; }

    public virtual Member Member { get; set; }
    public virtual Season Season { get; set; }

    public byte[] Version { get; set; }

    public object Update(object season)
    {
        if (season is SeasonResult m && Id == m.Id)
        {

        }

        return this;
    }
}
