using System;

namespace MMBot.Blazor.Data
{
    public class StateContainer
    {
        private string _selectedGuildId;
        public string SelectedGuildId 
        {
            get => _selectedGuildId;
            set
            {
                _selectedGuildId = value;
                NotifyStateChanged();
            }
        }

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
