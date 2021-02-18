using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using MMBot.Data;
using MMBot.Discord.Services;
using MMBot.Discord.Services.CommandHandler;
using MMBot.Discord.Services.GoogleForms;
using MMBot.Discord.Services.IE;
using MMBot.Discord.Services.Interfaces;
using MMBot.Discord.Services.MemberSort;
using MMBot.Discord.Services.Timer;
using Microsoft.EntityFrameworkCore;
using MMBot.Data.Services;
using MMBot.Data.Services.Interfaces;

namespace MMBot.Discord
{
    public static class DiscordSocketHost
    {
         public static IHostBuilder CreateDiscordSocketHost(string[] args) =>
           Host.CreateDefaultBuilder(args)
                .ConfigureLogging(x => x.ClearProviders().AddSerilog(Log.Logger))
                .UseSystemd()
                .ConfigureAppConfiguration((hostContext, configBuilder) =>
                {
                    configBuilder.AddEnvironmentVariables("MMBot_")
                                 .AddJsonFile("appsettings.json", false, true)
                                 .AddUserSecrets<DiscordWorker>()
                                 .AddCommandLine(args);        
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var dsc = new DiscordSocketClient(new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Warning,
                        MessageCacheSize = 1000
                    });

                    var cs = new CommandService(new CommandServiceConfig
                    {
                        LogLevel = LogSeverity.Warning,
                        CaseSensitiveCommands = false,
                        DefaultRunMode = RunMode.Async,
                        SeparatorChar = ' '
                    });

                    var hc = services.AddHttpClient("forms", c =>
                    {
                        c.BaseAddress = new Uri("https://docs.google.com");
                        c.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        c.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36");
                    });

                    var connectionString = hostContext.Configuration.GetConnectionString("Context");

                    services
                        .AddHostedService<DiscordWorker>()
                        .AddSingleton<GuildSettingsService>()
                        .AddDbContext<Context>(o => o.UseNpgsql(connectionString))
                        //.AddDbContext<Context>(o => o.UseSqlite($"Data Source={dbFilePath}"))
                        .AddSingleton<IGuildSettingsService, GuildSettingsService>()
                        .AddSingleton(dsc)
                        .AddSingleton(cs)
                        .AddScoped<IGoogleFormsService, GoogleFormsService>()
                        .AddSingleton<ICommandHandler, CommandHandler>()
                        .AddScoped<IGuildSettingsService, GuildSettingsService>()
                        .AddScoped<IDatabaseService, DatabaseService>()
                        .AddScoped<ICsvService, CsvService>()
                        .AddScoped<IJsonService, JsonService>()
                        .AddScoped<IAdminService, AdminService>()
                        .AddScoped<IMemberSortService, MemberSortService>()
                        .AddSingleton<InteractiveService, InteractiveService>()
                        .AddSingleton<ITimerService, TimerService>()
                        //.AddScoped<IGoogleSheetsService, GoogleSheetsService>()
                        //.AddScoped<IRaidService, RaidService>()
                        .BuildServiceProvider();
                });
    }
}
