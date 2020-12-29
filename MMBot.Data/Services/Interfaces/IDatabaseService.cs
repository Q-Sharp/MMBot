using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMBot.Data.Entities;

namespace MMBot.Data.Services.Interfaces
{
    public interface IDatabaseService
    {
        Task<Clan> CreateClanAsync(ulong guildId);
        Task<IList<Clan>> LoadClansAsync(ulong guildId);
        void DeleteClan(Clan c);
        void DeleteClan(int id);

        Task<Member> CreateMemberAsync(ulong guildId);
        Task<IList<Member>> LoadMembersAsync(ulong guildId);
        void DeleteMember(Member m);
        void DeleteMember(int id);

        Task<GuildSettings> LoadGuildSettingsAsync(ulong guildId);

        Task<Restart> AddRestart();
        Task<Tuple<ulong, ulong>> ConsumeRestart();

        Task<Channel> CreateChannelAsync(ulong guildId);
        Task<IList<Channel>> LoadChannelsAsync(ulong? guildId = null);
        void DeleteChannel(Channel c);
        void DeleteChannel(int id);

        Task<MMTimer> CreateTimerAsync(ulong guildId);
        Task<IList<MMTimer>> LoadTimerAsync(ulong? guildId = null);
        Task<MMTimer> GetTimerAsync(string name, ulong guildId);
        void DeleteTimer(MMTimer c);
        void DeleteTimer(int id);

        Task SaveDataAsync();
        Task MigrateAsync();
        Task<Context> DeleteDB();
        Task CleanDB(IEnumerable<ulong> guildIds = null);
    }
}
