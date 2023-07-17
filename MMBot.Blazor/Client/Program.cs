var builder = WebAssemblyHostBuilder.CreateDefault(args);
var services = builder.Services;

Log.Logger = new LoggerConfiguration()
                .WriteTo.BrowserConsole()
                .CreateLogger();

builder.Logging.ClearProviders();

if (builder.HostEnvironment.IsDevelopment())
    builder.Logging.AddSerilog(Log.Logger);

services.AddOptions();
services.AddAuthorizationCore();

services.TryAddSingleton<AuthenticationStateProvider, MMBotAuthenticationStateProvider>();
services.TryAddSingleton(sp => (MMBotAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
services.AddTransient<AuthorizedHandler>()
        .AddScoped<ISelectedGuildService, SelectedGuildService>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

services.AddHttpClient("default", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

services.AddHttpClient("authorizedClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<AuthorizedHandler>();

services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));

services.AddTransient<IAuthorizedAntiForgeryClientFactory, AuthorizedAntiForgeryClientFactory>();

services.AddBlazoredSessionStorage();
services.AddMudServices();

services.AddScoped<IRepository<Clan>, DataRepository<Clan>>()
        .AddScoped<IRepository<Member>, DataRepository<Member>>()
        .AddScoped<ICRUDViewModel<MemberModel, Member>, ViewModel<MemberModel, Member>>()
        .AddScoped<ICRUDViewModel<ClanModel, Clan>, ViewModel<ClanModel, Clan>>();

await builder.Build().RunAsync();
