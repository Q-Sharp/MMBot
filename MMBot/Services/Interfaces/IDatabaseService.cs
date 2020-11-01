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
        Task<Clan> GetClanAsync(string tag);
        void DeleteClan(Clan c);

        Task<Member> CreateMemberAsync();
        Task<IList<Member>> LoadMembersAsync();
        Task<Member> GetMemberAsync(string name);
        void DeleteMember(Member m);

        Task<GuildSettings> LoadGuildSettingsAsync();

        Task<Restart> AddRestart();
        Task<Tuple<ulong, ulong>> ConsumeRestart();

        Task<Channel> CreateChannelAsync();
        Task<IList<Channel>> LoadChannelsAsync();
        void DeleteChannel(Channel c);

        Task<MMTimer> CreateTimerAsync();
        Task<IList<MMTimer>> LoadTimerAsync();
        Task<MMTimer> GetTimerAsync(string name);
        void DeleteTimer(MMTimer c);

        Task SaveDataAsync();
        Task MigrateAsync();
        Task CleanDB();
    }
}
