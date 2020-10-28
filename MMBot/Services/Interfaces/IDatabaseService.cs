using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMBot.Data.Entities;

namespace MMBot.Services.Interfaces
{
    public interface IDatabaseService : IMMBotInterface, IGuildSetter
    {
        Task<Clan> CreateClanAsync();
        Task<IList<Clan>> LoadClansAsync();
        void DeleteClan(Clan c);

        Task<Member> CreateMemberAsync();
        Task<IList<Member>> LoadMembersAsync();
        void DeleteMember(Member m);

        Task<GuildSettings> LoadGuildSettingsAsync();

        Task<Restart> AddRestart();
        Task<Tuple<ulong, ulong>> ConsumeRestart();

        Task<Channel> CreateChannelAsync();
        Task<IList<Channel>> LoadChannelsAsync();
        void DeleteChannel(Channel c);

        Task SaveDataAsync();
        Task MigrateAsync();
        Task CleanDB();
    }
}
