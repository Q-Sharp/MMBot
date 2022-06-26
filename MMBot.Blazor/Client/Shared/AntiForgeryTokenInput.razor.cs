namespace MMBot.Blazor.Client.Shared;

public partial class AntiForgeryTokenInput
{
    [Inject]
    public IJSRuntime JSRuntime { get; set; }

    private string token = "";

    protected override async Task OnInitializedAsync()
        => token = await JSRuntime.InvokeAsync<string>("getAntiForgeryToken");

    public string GetToken() => token;
}
