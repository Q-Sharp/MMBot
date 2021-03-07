using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MMBot.Blazor.Services
{
    public class DCUser : IDCUser
    {
        private ProtectedSessionStorage _protectedSessionStorage;

        public DCUser(ProtectedSessionStorage protectedSessionStorage)
        {
            _protectedSessionStorage = protectedSessionStorage;
        }
        
        public string Name { get; set; }
        public string CurrentGuildId { get; set; }

        public async Task SetCurrentGuildId(string guildId)
        {
            await _protectedSessionStorage.SetAsync(nameof(CurrentGuildId), guildId);
            CurrentGuildId = guildId;
        }

        public async Task<string> GetCurrentGuildId() => (await _protectedSessionStorage.GetAsync<string>(nameof(CurrentGuildId))).Value;
        public ulong? CurrentGuildIdUlong
        {
            get
            {
                if (ulong.TryParse(CurrentGuildId, out var val))
                    return val;
                return null;
            }
        }

        public IList<DCChannel> Guilds { get; set; } = new List<DCChannel>();
    }
}
