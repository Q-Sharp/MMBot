using System.Text.Json.Serialization;
using MMBot.Data.Contracts.Enums;

namespace MMBot.Data.Contracts.Entities;

public class Season : IHaveId
{
    public int Id { get; set; }
    public int No { get; set; }
    public EraType Era { get; set; }
    public DateTime Start { get; set; }
    public string Identitfier => $"{Start} {Era} {No}";
    public string Name => Identitfier;

    [JsonIgnore]
    public virtual ICollection<SeasonResult> SeasonResult { get; set; }

    public void Update(object season)
    {

    }
}
