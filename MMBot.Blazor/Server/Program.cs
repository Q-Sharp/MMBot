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

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var connectionString = configuration.GetConnectionString("Context");

services.AddDbContextFactory<Context>(o => o.UseLazyLoadingProxies()
                                            .UseNpgsql(connectionString))
        .AddScoped<IDatabaseService, DatabaseService>()
        .AddScoped<IBlazorDatabaseService, BlazorDatabaseService>()
        .AddScoped<IRepository<Clan>, DataRepository<Clan, Context>>()
        .AddScoped<IRepository<Member>, DataRepository<Member, Context>>();

//services.AddSingleton<ITicketStore, MMBotTicketStore>();
//services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
//        .Configure<ITicketStore>((options, store) => options.SessionStore = store);

//services.AddAntiforgery(options =>
//{
//    options.HeaderName = AntiforgeryDefaults.Headername;
//    options.Cookie.Name = AntiforgeryDefaults.Cookiename;
//    options.Cookie.SameSite = SameSiteMode.Strict;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//});

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
.AddDiscord(DiscordAuthenticationDefaults.AuthenticationScheme, c =>
{
    c.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    c.ClientId = configuration["Discord:AppId"];
    c.ClientSecret = configuration["Discord:AppSecret"];

    c.Events = new OAuthEvents
    {
        OnCreatingTicket = async context =>
        {
            var guildClaim = await DiscordHelpers.GetGuildClaims(context);
            context.Identity.AddClaim(guildClaim);
        },
        OnAccessDenied = context =>
        {
            context.AccessDeniedPath = PathString.FromUriComponent("/");
            context.ReturnUrlParameter = string.Empty;
            return Task.CompletedTask;
        }
    };

    c.SaveTokens = true;
});

services.AddAuthorization();
//services.AddControllersWithViews(/*o => o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute())*/);
services.AddControllers();
services.AddRazorPages();

services.AddMudServices();

services.AddSession()
        .AddHttpContextAccessor();

services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
    options.EnableForHttps = true;
});

services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Any);
    options.ForwardedHeaders = ForwardedHeaders.All;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    _ = app.UseDeveloperExceptionPage();
}
else
{
    _ = app.UseForwardedHeaders()
       .UseCertificateForwarding()
       .UseExceptionHandler("/Error")
       .UseHsts()
       .UseCookiePolicy();
}

//app.UseSecurityHeaders(SecurityHeadersDefinitions.GetHeaderPolicyCollection(app.Environment.IsDevelopment(),
//                    configuration["Discord:Authority"]));

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = context =>
    {
        if (context.File.Name == "service-worker-assets.js")
        {
            context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
            context.Context.Response.Headers.Add("Expires", "-1");
        }
    }
});

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
