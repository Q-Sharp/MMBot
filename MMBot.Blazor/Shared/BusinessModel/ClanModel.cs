namespace MMBot.Blazor.Shared.BusinessModel;

public class ClanModel : Clan, ICreate
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

    public static ClanModel Create(Clan c) => new()
    {
        Id = c.Id,
        SortOrder = c.SortOrder,
        Tag = c.Tag,
        Name = c.Name,
        DiscordRole = c.DiscordRole,
        GuildId = c.GuildId,
    };

    public ICreate Create(object from) => from is Clan c
            ? new ClanModel()
            {
                Id = c.Id,
                SortOrder = c.SortOrder,
                Tag = c.Tag,
                Name = c.Name,
                DiscordRole = c.DiscordRole,
                GuildId = c.GuildId,
            }
            : (ICreate)null;
}
