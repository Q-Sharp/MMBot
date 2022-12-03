namespace MMBot.Blazor.Client.Shared;

public partial class ClanLookUp
{
    [Inject]
    public ICRUDViewModel<ClanModel, Clan> ClanVM { get; set; }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        ClanVM.SelectedEntity = ClanVM?.Entities.FirstOrDefault(x => x.Id == SelectedId);
        StateHasChanged();
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task UpdateId(object changed) => await ValueChanged.InvokeAsync();

    [Parameter]
    public int? SelectedId { get; set; }

    [Parameter]
    public string SelectedClan { get; set; }

    [Parameter]
    public EventCallback ValueChanged { get; set; }
}
