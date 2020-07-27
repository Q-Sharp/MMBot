using TTMMBot.Data.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TTMMBot.Services.Interfaces
{
    public interface IDatabaseClanService
    {
        Task<Clan> FindClanAsync(Clan clan);
        Task DeleteClanAsync(Clan clan);
        Task<IList<Clan>> LoadClansAsync();
        Task CreateClanAsync(Clan clan);
    }
}