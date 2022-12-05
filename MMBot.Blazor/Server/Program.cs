using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.Extensions.Options;

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
        .AddScoped<IRepository<Clan>, DataRepository<Clan, Context>>()
        .AddScoped<IRepository<Member>, DataRepository<Member, Context>>();

services.AddHttpClient();
services.AddOptions();
services.AddLocalization();

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

    options.SaveTokens = true;
});

services.AddAuthorization();
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

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseRequestLocalization("en-US");

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
