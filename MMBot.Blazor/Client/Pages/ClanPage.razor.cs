namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class ClanPage
{
    [Inject]
    public IAuthorizedAntiForgeryClientFactory AuthorizedAntiForgeryClientFactory { get; set; }

    [Inject]
    ICRUDViewModel<ClanModel, Clan> ClanVM { get; set; }

    private string searchString = "";

    protected override async Task OnInitializedAsync() => await ClanVM.Init();

    private bool FilterFunc(ClanModel clan)
    {
        if (string.IsNullOrWhiteSpace(searchString))
            return true;
        if (clan.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        if (clan.Tag.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }
}
