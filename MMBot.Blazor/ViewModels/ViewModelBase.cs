using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MMBot.Blazor.Data;
using MudBlazor;

namespace MMBot.Blazor.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public IDialogService DialogService { get; set; }
    public StateContainer StateContainer { get; set; }

    public ViewModelBase(StateContainer stateContainer, IDialogService dialogService)
    {
        DialogService = dialogService;
        StateContainer = stateContainer;
    }

    public bool Initialized { get; set; }

    public virtual Task InitializeAsync()
    {
        Initialized = true;
        return Task.CompletedTask;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
