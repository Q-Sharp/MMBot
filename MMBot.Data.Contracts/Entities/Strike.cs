namespace MMBot.Data.Contracts.Entities;

public class Strike : IHaveId
{
    public int Id { get; set; }

    public string Reason { get; set; }

    public DateTime? StrikeDate { get; set; }

    public int MemberId { get; set; }

    public virtual Member Member { get; set; }

    public byte[] Version { get; set; }

    public object Update(object strike)
    {
        if (strike is Strike m && Id == m.Id)
        {
            Reason = m.Reason;
            StrikeDate = m.StrikeDate;
            MemberId = m.MemberId;
            Member = m.Member;
        }

        return this;
    }
}
