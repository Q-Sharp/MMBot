namespace MMBot.Blazor.Client.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public IDialogService DialogService { get; set; }
    public ISessionStorageService SessionStorage { get; set; }

    public ViewModelBase(ISessionStorageService sessionStorage, IDialogService dialogService)
    {
        DialogService = dialogService;
        SessionStorage = sessionStorage;
    }

    public bool Initialized { get; set; }

    public virtual Task InitializeAsync()
    {
        Initialized = true;
        return Task.CompletedTask;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
