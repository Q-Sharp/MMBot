using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace MMBot.Blazor.Services
{
    public class DCUser : IDCUser
    {
        public DCUser()
        {
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null) 
        {
            if(!EqualityComparer<T>.Default.Equals(field, value)) 
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }

        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event PropertyChangedEventHandler PropertyChanged;
        
        public string Name { get; set; }

        private string _currentGuildId;
        public string CurrentGuildId
        {
            get => _currentGuildId;
            set => SetProperty(ref _currentGuildId, value);
        }

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
