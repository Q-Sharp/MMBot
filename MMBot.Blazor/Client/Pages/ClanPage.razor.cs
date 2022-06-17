namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class ClanPage
{
    [Inject]
    public IAuthorizedAntiForgeryClientFactory AuthorizedAntiForgeryClientFactory { get; set; }

    [Inject]
    ICRUDViewModel<ClanModel, Clan> ClanVM { get; set; }

    [Inject] IRepository<Clan> Clan { get; set; }


    private string searchString = "";

    public async void CommitEdit() => await ClanVM.Update(ClanVM.SelectedEntity);

    public async void Delete() => await ClanVM.Delete();

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
