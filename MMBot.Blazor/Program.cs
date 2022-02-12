using Serilog;

namespace MMBot.Blazor;

public class Program
{
    private const string _logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    public static async Task Main(string[] args)
    {
        try
        {
            using var bh = BlazorHost.CreateHostBuilder(args)?.Build();
            await bh.RunAsync();
        }
        catch (Exception e)
        {
            Log.Fatal(e, _logTemplate);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
