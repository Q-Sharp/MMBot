using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MMBot.Data;
using MMBot.Data.Contracts;
using MMBot.Data.Services.Database;
using MMBot.Discord.Services.Admin;
using MMBot.Discord.Services.CommandHandler;
using MMBot.Discord.Services.GuildSettings;
using MMBot.Discord.Services.IE;
using MMBot.Discord.Services.Interfaces;
using MMBot.Discord.Services.MemberSort;
using MMBot.Discord.Services.Timer;
using MMBot.Discord.Services.Translation;
using Serilog;

namespace MMBot.Discord;

public static class DiscordSocketHost
{
    public static IHostBuilder CreateDiscordSocketHost(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .UseSystemd()
           .ConfigureAppConfiguration((hostContext, configBuilder) =>
           {
               try
               {
                    configBuilder.AddEnvironmentVariables()
                                 .AddUserSecrets<DiscordWorker>();
               }
               catch
               {
                   // ignore
               }
           })
           .UseSerilog((h, l) => l.ReadFrom.Configuration(h.Configuration))
           .ConfigureServices((hostContext, services) =>
           {
               var config = hostContext.Configuration;
               var discordConfig = config.GetSection("Discord").GetSection("Settings");

                services
                   .AddHostedService<DiscordWorker>()
                   .AddSingleton<GuildSettingsService>()
                   .AddDbContext<Context>(o => o.UseNpgsql(config.GetConnectionString("Context")))
                   .AddSingleton<IGuildSettingsService, GuildSettingsService>()
                   .AddSingleton(o => new DiscordSocketClient(new DiscordSocketConfig
                   {
                       LogLevel = Enum.Parse<LogSeverity>(discordConfig.GetValue<string>("LogLevel"), true),
                       MessageCacheSize = discordConfig.GetValue<int>("MessageCacheSize"),
                       GatewayIntents = GatewayIntents.All
                   }))
                   .AddSingleton(s => new CommandService(new CommandServiceConfig
                   {
                       LogLevel = Enum.Parse<LogSeverity>(discordConfig["LogLevel"], true),
                       CaseSensitiveCommands = discordConfig.GetValue<bool>("CaseSensitiveCommands"),
                       DefaultRunMode = Enum.Parse<RunMode>(discordConfig.GetValue<string>("DefaultRunMode"), true),
                       SeparatorChar = discordConfig.GetValue<string>("SeparatorChar").FirstOrDefault(),
                   }))
                   .AddSingleton<ICommandHandler, CommandHandler>()
                   .AddScoped<IGuildSettingsService, GuildSettingsService>()
                   .AddScoped<IDatabaseService, DatabaseService>()
                   .AddScoped<ICsvService, CsvService>()
                   .AddScoped<IJsonService, JsonService>()
                   .AddScoped<IAdminService, AdminService>()
                   .AddScoped<IMemberSortService, MemberSortService>()
                   .AddSingleton<ITimerService, TimerService>()
                   .AddTransient<ITranslationService, TranslationService>()
                   .AddHttpClient()
                   .BuildServiceProvider();
           });
}
