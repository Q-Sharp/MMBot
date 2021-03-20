using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MMBot.Blazor.Services
{
    public class DCUser : IDCUser
    {
        public string CurrentGuildId { get; set; }
        public string Name { get; set; }
        public ulong Id { get; set; }
        public IList<DCChannel> Guilds { get; set; } = new List<DCChannel>();
        public string AvatarUrl { get; set; }
    }
}
