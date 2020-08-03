using TTMMBot.Data.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TTMMBot.Services
{
    public interface IDatabaseService
    {
        Task<Clan> CreateClanAsync();
        Task<IList<Clan>> LoadClansAsync();
        void DeleteClan(Clan c);
        Task<Member> CreateMemberAsync();
        Task<IList<Member>> LoadMembersAsync();
        void DeleteMember(Member m);
        Task SaveDataAsync();
        Task MigrateAsync();
    }
}