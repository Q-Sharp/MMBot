using System;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MMBot.Blazor;

public class Program
{
    private const string _logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    public static void Main(string[] args)
    {
        try
        {
            using var bh = BlazorHost.CreateHostBuilder(args)?.Build();
            var t = bh.RunAsync();
            t.Wait();
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
