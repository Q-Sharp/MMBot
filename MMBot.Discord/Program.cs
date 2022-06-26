using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace MMBot.Discord;

public class Program
{
    private const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File(path: Path.Combine(Environment.CurrentDirectory, "mmbot.log"), outputTemplate: logTemplate, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
            .CreateLogger();

        try
        {
            using var hb = DiscordSocketHost.CreateDiscordSocketHost(args)?.Build();
            await hb.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, logTemplate);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
