using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using Serilog.Events;
using System.Threading.Tasks;
using MMBot.Discord;
using MMBot.Blazor;

namespace MMBot
{
    public class Program
    {
        private const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

        private static readonly string _logFilePath = Path.Combine(Environment.CurrentDirectory, "mmbot.log");
        private static readonly string _dbFilePath = Path.Combine(Environment.CurrentDirectory, "mmbot.db");
        private static readonly string _wwwRootPath = Path.Combine(Environment.CurrentDirectory, @".\wwwroot\");

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                 .Enrich.FromLogContext()
                 .WriteTo.Console(theme: AnsiConsoleTheme.Literate, outputTemplate: logTemplate, restrictedToMinimumLevel: LogEventLevel.Verbose)
                 .WriteTo.File(path: _logFilePath, outputTemplate: logTemplate, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
                 .CreateLogger();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

            try
            {
                using var hb = DiscordSocketHost.CreateDiscordSocketHost(args, _dbFilePath)?.Build();
                using var bh = BlazorHost.CreateHostBuilder(args, _dbFilePath, _wwwRootPath)?.Build();
                var t = new Task[] { bh.RunAsync(), hb.RunAsync() };
                Task.WaitAll(t);
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

        private static void CurrentDomain_ProcessExit(object sender, EventArgs e) 
            => Log.Logger.Information("Exiting!");

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                Log.Logger.Error($"Unhandled exception: {ex.Message}");

            if (e.IsTerminating)
                Log.Logger.Error("Terminating!");
        }
    }
}
