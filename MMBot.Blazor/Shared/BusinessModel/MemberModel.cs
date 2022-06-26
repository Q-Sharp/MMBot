namespace MMBot.Blazor.Shared.BusinessModel;

public class MemberModel : Member, ICreate
{
    public event Action StateChanged;
    private void NotifyStateChanged() => StateChanged?.Invoke();

    public override string Name
    {
        get => base.Name;
        set
        {
            base.Name = value;
            NotifyStateChanged();
        }
    }

    public override string Discord
    {
        get => base.Discord;
        set
        {
            base.Discord = value;
            NotifyStateChanged();
        }
    }

    public override int? AHigh
    {
        get => base.AHigh;
        set
        {
            base.AHigh = value;
            NotifyStateChanged();
        }
    }

    public override int? Current
    {
        get => base.Current;
        set
        {
            base.Current = value;
            NotifyStateChanged();
        }
    }

    public override Role Role
    {
        get => base.Role;
        set
        {
            base.Role = value;
            NotifyStateChanged();
        }
    }

    public override DiscordStatus DiscordStatus
    {
        get => base.DiscordStatus;
        set
        {
            base.DiscordStatus = value;
            NotifyStateChanged();
        }
    }

    public override bool IsActive
    {
        get => base.IsActive;
        set
        {
            base.IsActive = value;
            NotifyStateChanged();
        }
    }

    public override DateTime? LastUpdated
    {
        get => base.LastUpdated;
        set
        {
            base.LastUpdated = value;
            NotifyStateChanged();
        }
    }

    public override int? ClanId
    {
        get => base.ClanId;
        set
        {
            base.ClanId = value;
            NotifyStateChanged();
        }
    }

    public ICreate Create(object from)
    {
        if(from is Member m)
        {
            return new MemberModel
            {
                Id = m.Id,
                Name = m.Name,
                Discord = m.Discord,
                AHigh = m.AHigh,
                Role = m.Role,
                DiscordStatus = m.DiscordStatus,
                IsActive = m.IsActive,
                ClanId = m.ClanId,
                LastUpdated = m.LastUpdated,
                Join = m.Join,
                IgnoreOnMoveUp = m.IgnoreOnMoveUp,
                PlayerTag = m.PlayerTag,
                GuildId = m.GuildId,
                MemberGroupId = m.MemberGroupId,
                LocalTimeOffSet = m.LocalTimeOffSet,
            };
        }

        return null;
    }

    public static MemberModel Create(Member m)
    {
        return new MemberModel
        {
            Id = m.Id,
            Name = m.Name,
            Discord = m.Discord,
            AHigh = m.AHigh,
            Role = m.Role,
            DiscordStatus = m.DiscordStatus,
            IsActive = m.IsActive,
            ClanId = m.ClanId,
            LastUpdated = m.LastUpdated,
            Join = m.Join,
            IgnoreOnMoveUp = m.IgnoreOnMoveUp,
            PlayerTag = m.PlayerTag,
            GuildId = m.GuildId,
            MemberGroupId = m.MemberGroupId,
            LocalTimeOffSet = m.LocalTimeOffSet,
        };
    }
}
