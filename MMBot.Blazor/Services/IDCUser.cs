using System.Collections.Generic;
using System.ComponentModel;

namespace MMBot.Blazor.Services
{
    public interface IDCUser : INotifyPropertyChanged 
    {
        string CurrentGuildId { get; set; }
        ulong? CurrentGuildIdUlong { get; }
        IList<DCChannel> Guilds { get; set; }
        string Name { get; set; }
    }
}