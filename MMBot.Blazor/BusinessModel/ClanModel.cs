using MMBot.Data.Entities;

namespace MMBot.Blazor.BusinessModel;

public class ClanModel : Clan
{
    public event Action StateChanged;
    private void NotifyStateChanged() => StateChanged?.Invoke();

    public override int SortOrder
    {
        get => base.SortOrder;
        set
        {
            base.SortOrder = value;
            NotifyStateChanged();
        }
    }

    public override string Tag
    {
        get => base.Tag;
        set
        {
            base.Tag = value;
            NotifyStateChanged();
        }
    }

    public override string Name
    {
        get => base.Name;
        set
        {
            base.Name = value;
            NotifyStateChanged();
        }
    }

    public static ClanModel Create(Clan c)
    {
        return new ClanModel
        {
            Id = c.Id,
            SortOrder = c.SortOrder,
            Tag = c.Tag,
            Name = c.Name,
            DiscordRole = c.DiscordRole,
            GuildId = c.GuildId,
        };
    }
}
