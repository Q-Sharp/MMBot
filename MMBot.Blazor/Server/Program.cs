using System.Net;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

using MMBot.Blazor.Server.Auth;
using MMBot.Blazor.Shared.Defaults;
using MMBot.Blazor.Shared.Helpers;
using MMBot.Data;
using MMBot.Data.Contracts;
using MMBot.Data.Services.Database;
using MudBlazor.Services;
using Serilog;

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
                          .AddSerilog(Log.Logger)
                          .AddSystemdConsole());

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

var connectionString = configuration.GetConnectionString("Context");

services.AddDbContext<Context>(o => o.UseNpgsql(connectionString))
        .AddScoped<IDatabaseService, DatabaseService>();

services.AddSingleton<ITicketStore, MMBotTicketStore>();
services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
        .Configure<ITicketStore>((options, store) => options.SessionStore = store);

services.AddAntiforgery(options =>
{
    options.HeaderName = AntiforgeryDefaults.Headername;
    options.Cookie.Name = AntiforgeryDefaults.Cookiename;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

services.AddHttpClient();
services.AddOptions();

services.AddResponseCaching();

services.AddAuthentication(opt =>
{
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, c =>
{
    c.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    c.ExpireTimeSpan = TimeSpan.FromDays(30);
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
    c.Validate();
});

services.AddAuthorization();
services.AddControllersWithViews(o => o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
services.AddRazorPages();

services.AddMudServices();

services.AddSession()
        .AddHttpContextAccessor()
        .AddScoped<IBlazorDatabaseService, BlazorDatabaseService>();

services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Any);
    options.ForwardedHeaders = ForwardedHeaders.All;
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
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

app.UseSecurityHeaders(SecurityHeadersDefinitions.GetHeaderPolicyCollection(app.Environment.IsDevelopment(),
                    configuration["Discord:Authority"]));

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();

app.UseStaticFiles(new StaticFileOptions()
{
    HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress,
    OnPrepareResponse = (context) =>
    {
        var headers = context.Context.Response.GetTypedHeaders();
        headers.CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(30)
        };
    }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseResponseCaching();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();
