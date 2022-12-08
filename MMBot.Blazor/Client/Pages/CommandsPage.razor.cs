namespace MMBot.Blazor.Client.Pages;

[Authorize]
public partial class CommandsPage
{
    public IEnumerable<Tuple<string, string>> Commands { get; }

    protected override Task OnInitializedAsync()
    {
        return Task.FromResult(0);
    }
}
