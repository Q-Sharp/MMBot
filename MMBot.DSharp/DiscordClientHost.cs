namespace MMBot.DSharp;

public static class DiscordClientHost
{
    public static IHostBuilder CreateDiscordClientHost(string[] args) =>
       Host.CreateDefaultBuilder(args)
           .UseSystemd()
           .ConfigureAppConfiguration((hostContext, configBuilder) =>
           {
               try
               {
                   configBuilder.AddEnvironmentVariables("MMBot_")
                                .AddUserSecrets<DiscordClientWorker>();
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

               services
                  .AddHostedService<DiscordClientWorker>()
                  .AddDbContext<Context>(o => o.UseNpgsql(config.GetConnectionString("Context")))
                  .AddSingleton(sp => new DiscordClient(new DiscordConfiguration
                  {
                      Token = config["Discord:DevToken"],
                      TokenType = TokenType.Bot,
                      Intents = DiscordIntents.All,
                      LoggerFactory = sp.GetRequiredService<ILoggerFactory>()
                  }))
                  .AddHttpClient()
                  .BuildServiceProvider();
           });
}
