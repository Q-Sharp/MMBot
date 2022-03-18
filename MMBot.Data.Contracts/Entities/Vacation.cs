using System.Text.Json.Serialization;

namespace MMBot.Data.Contracts.Entities;

public class Vacation : IHaveId
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int MemberId { get; set; }

    [JsonIgnore]
    public virtual Member Member { get; set; }

    public void Update(object vacation)
    {
        if (vacation is Vacation v && Id == v.Id)
        {
            StartDate = v.StartDate;
            EndDate = v.EndDate;
            MemberId = v.MemberId;
        }
    }
}
