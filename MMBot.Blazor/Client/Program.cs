var builder = WebAssemblyHostBuilder.CreateDefault(args);

Log.Logger = new LoggerConfiguration()
                .WriteTo.BrowserConsole()
                .CreateLogger();

builder.Logging.ClearProviders();

if (builder.HostEnvironment.IsDevelopment())
    builder.Logging.AddSerilog(Log.Logger);

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();

builder.Services.TryAddSingleton<AuthenticationStateProvider, MMBotAuthenticationStateProvider>();
builder.Services.TryAddSingleton(sp => (MMBotAuthenticationStateProvider)sp.GetRequiredService<AuthenticationStateProvider>());
builder.Services.AddTransient<AuthorizedHandler>();

builder.RootComponents.Add<App>("#app");

builder.Services.AddHttpClient("default", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient("authorizedClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
}).AddHttpMessageHandler<AuthorizedHandler>();

builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("default"));

builder.Services.AddTransient<IAuthorizedAntiForgeryClientFactory, AuthorizedAntiForgeryClientFactory>();

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddMudServices();

builder.Services.AddCRUDViewModels();

await builder.Build().RunAsync();
