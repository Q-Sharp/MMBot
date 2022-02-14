using System.Net;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MMBot.Blazor.Data;
using MMBot.Blazor.Helpers;
using MMBot.Blazor.Services;
using MMBot.Data;
using MMBot.Data.Services.Database;
using MMBot.Data.Services.Interfaces;
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

services.AddLogging(c => c.ClearProviders()
                          .AddSerilog(Log.Logger)
                          .AddSystemdConsole());

services.AddRazorPages();
services.AddServerSideBlazor();
services.AddRouting();

var connectionString = configuration.GetConnectionString("Context");

services.AddDbContext<Context>(o => o.UseNpgsql(connectionString))
        .AddScoped<IDatabaseService, DatabaseService>();

services.AddDataProtection()
.UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
{
    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
})
.SetApplicationName("MMBot");

services.AddAuthentication(opt =>
{
    opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, c =>
{
    c.Cookie.SameSite = SameSiteMode.Strict;
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

services.AddAuthorization(c => c.InvokeHandlersAfterFailure = false);

services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(IPAddress.Any);
    options.ForwardedHeaders = ForwardedHeaders.All;
});

services.AddSession()
        .AddHttpContextAccessor()
        .AddScoped<IAccountService, AccountService>()
        .AddScoped<IDCUser, DCUser>()
        .AddSingleton<StateContainer>()
        .AddScoped<BlazorDatabaseService>()
        .AddCRUDViewModels()
        .AddMudServices();

var app = builder.Build();

if (env.IsDevelopment())
    app.UseDeveloperExceptionPage();    
else
{
    app.UseForwardedHeaders()
       .UseCertificateForwarding()
       .UseExceptionHandler("/Error")
       .UseHsts()
       .UseCookiePolicy();
}

app.UseHttpsRedirection()
   .UseStaticFiles()
   .UseRouting()
   .UseAuthentication()
   .UseAuthorization()
   .UseSession();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

await app.RunAsync();
