using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMBot.Data.Interfaces;

namespace MMBot.Data.Helpers
{
    public static class EntityHelper
    {
        public async static Task ImportOrUpgradeWithIdentifier<T>(this DbSet<T> currentData, T updateWithData, ulong guildId)
            where T : class, IHaveId, IHaveIdentifier 
                => await ImportOrUpgradeWithIdentifier(currentData, new List<T> { updateWithData }, guildId);

        public async static Task ImportOrUpgradeWithIdentifier<T>(this DbSet<T> currentData, IList<T> updateWithData, ulong guildId) 
            where T : class, IHaveId, IHaveIdentifier
        {
            foreach(var uwd in updateWithData)
            {
                var found = currentData.AsQueryable().FirstOrDefault(x => x.Name == uwd.Name && x.Identitfier == uwd.Identitfier && uwd.GuildId == guildId);

                if(found is not null)
                    found.Update(uwd);
                else
                    await currentData.AddAsync(uwd);
            }
        }

        public async static Task ImportOrUpgrade<T>(this DbSet<T> currentData, IList<T> updateWithData) 
            where T : class, IHaveId
        {
            foreach(var uwd in updateWithData)
            {
                var found = await currentData.FindAsync(uwd.Id);

                if(found is not null)
                    found.Update(uwd);
                else
                    await currentData.AddAsync(uwd);
            }
        }
    }
}
