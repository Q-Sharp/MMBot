namespace MMBot.Blazor.Client.Shared;

public partial class MainLayout
{
    [Inject]
    public ISelectedGuildService SelectedGuildService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [Inject]
    public MMBotAuthenticationStateProvider AuthProvider { get; set; }

    private bool _drawerOpen = true;
    private readonly MudTheme currentTheme = new()
    {
        Palette = new PaletteDark()
        {
            Black = "#27272f",
            Background = "#32333d",
            BackgroundGrey = "#27272f",
            Surface = "#373740",
            DrawerBackground = "#27272f",
            DrawerText = "rgba(255,255,255, 0.50)",
            DrawerIcon = "rgba(255,255,255, 0.50)",
            AppbarBackground = "#27272f",
            AppbarText = "rgba(255,255,255, 0.70)",
            TextPrimary = "rgba(255,255,255, 0.70)",
            TextSecondary = "rgba(255,255,255, 0.50)",
            ActionDefault = "#adadb1",
            ActionDisabled = "rgba(255,255,255, 0.26)",
            ActionDisabledBackground = "rgba(255,255,255, 0.12)"
        }
    };

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    public DCChannel SelectedGuild { get; set; } = new DCChannel();
    public DCUser LoggedUser { get; private set; } = DCUser.Anonymous;
    public IEnumerable<DCChannel> Guilds { get; set; } = Enumerable.Empty<DCChannel>();

    protected async override Task OnInitializedAsync()
    {
        LoggedUser = await AuthProvider.GetCurrentUser();

        if(LoggedUser.IsAuthenticated)
        {
            Guilds = await SelectedGuildService.GetGuilds();
            SelectedGuild = Guilds.FirstOrDefault(x => x.IsSelected) ?? Guilds.FirstOrDefault();
            await GuildChanged();
        }
    }

    private async Task GuildChanged() 
        => await SelectedGuildService.SetSelectedGuild(SelectedGuild.Id);
}
