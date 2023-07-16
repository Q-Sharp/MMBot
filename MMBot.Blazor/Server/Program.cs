using MMBot.Blazor.Server.Auth;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
var env = builder.Environment;

configuration.AddEnvironmentVariables("MMBot_")
             .AddUserSecrets(typeof(Program).Assembly)
             .AddCommandLine(args);

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

services.AddLogging(l => l.ClearProviders()
                          .AddSerilog(Log.Logger));

var connectionString = configuration.GetConnectionString("Context");

services.AddDbContextFactory<Context>(o => o.UseLazyLoadingProxies().UseNpgsql(connectionString))
        .AddScoped<IDatabaseService, DatabaseService>()
        .AddScoped<IBlazorDatabaseService, BlazorDatabaseService>()
        .AddScoped(typeof(IRepository<>), typeof(DataRepository<>))
        .AddTransient<IMiddleware, ExceptionHandlingMiddleware>();

services.AddHttpClient();
services.AddOptions();

services.AddAuthentication(opt =>
{
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.Cookie.MaxAge = TimeSpan.FromDays(30);
    options.Cookie.Name = ApiAuthDefaults.CookieName;
})
.AddDiscord(DiscordAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ClientId = configuration["Discord:AppId"];
    options.ClientSecret = configuration["Discord:AppSecret"];

    options.Scope.Add("guilds");

    options.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var guildClaim = await Task.Run(async () =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://discordapp.com/api/users/@me/guilds");
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, CancellationToken.None);
                if (!response.IsSuccessStatusCode)
                    throw new Exception("failed to get guilds");

                var payload = await response.Content.ReadAsStringAsync();
                var claim = new Claim("guilds", payload, ClaimValueTypes.String);
                return claim;
            });

            context.Identity.AddClaim(guildClaim);
        },
        OnAccessDenied = context =>
        {
            context.AccessDeniedPath = PathString.FromUriComponent("/");
            context.ReturnUrlParameter = string.Empty;
            return Task.CompletedTask;
        }
    };

    options.SaveTokens = true;
});

services.AddAuthorization();
services.AddControllers(o => o.Conventions.Add(new GenericRouteConvention()))
        .ConfigureApplicationPartManager(m => m.FeatureProviders.Add(new GenericFeatureProvider()));

services.AddRazorPages().AddMvcOptions(options =>
{
    //var policy = new AuthorizationPolicyBuilder()
    //    .RequireAuthenticatedUser()
    //    .Build();
    //options.Filters.Add(new AuthorizeFilter(policy));
});

services.AddSession()
        .AddHttpContextAccessor();

services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Any);
    options.ForwardedHeaders = ForwardedHeaders.All;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseForwardedHeaders()
       .UseCertificateForwarding()
       .UseExceptionHandler("/Error")
       .UseHsts()
       .UseCookiePolicy();
}

app.UseSecurityHeaders(
    SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment(),
        configuration["Discord:Authority"]!));


app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRequestLocalization("en-US");

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToPage("/_Host");

using var scope = app.Services.CreateScope();
var ctx = scope.ServiceProvider.GetRequiredService<Context>();
await ctx.MigrateAsync();

app.Run();
