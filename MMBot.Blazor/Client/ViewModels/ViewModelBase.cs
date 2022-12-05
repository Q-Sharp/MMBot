namespace MMBot.Blazor.Client.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
    public IDialogService DialogService { get; set; }
    public ISelectedGuildService SelectedGuildService { get; set; }

    public ViewModelBase(ISelectedGuildService selectedGuildService, IDialogService dialogService)
    {
        DialogService = dialogService;
        SelectedGuildService = selectedGuildService;
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
