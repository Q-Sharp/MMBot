namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class ClanPage
{
    [Inject]
    public IAuthorizedAntiForgeryClientFactory AuthorizedAntiForgeryClientFactory { get; set; }

    [Inject]
    private ICRUDViewModel<ClanModel, Clan> ClanVM { get; set; }

    private readonly string searchString = "";

    protected override async Task OnInitializedAsync() => await ClanVM.Init();

    private bool FilterFunc(ClanModel clan) 
        => string.IsNullOrWhiteSpace(searchString)
            || clan.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            || clan.Tag.Contains(searchString, StringComparison.OrdinalIgnoreCase);
}
