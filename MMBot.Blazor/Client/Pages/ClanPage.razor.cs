namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class ClanPage
{
    [Inject]
    public IAuthorizedAntiForgeryClientFactory AuthorizedAntiForgeryClientFactory { get; set; }

    [Inject]
    private ICRUDViewModel<ClanModel, Clan> ClanVM { get; set; }

    [CascadingParameter]
    public DCChannel SelectedGuild { get; set; } = new DCChannel();

    private readonly string searchString = "";

    protected override async Task OnParametersSetAsync() => await ClanVM.Load();

    private bool FilterFunc(ClanModel clan) 
        => string.IsNullOrWhiteSpace(searchString)
            || clan.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            || clan.Tag.Contains(searchString, StringComparison.OrdinalIgnoreCase);
}
