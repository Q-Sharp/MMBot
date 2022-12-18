var logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

Log.Logger = new LoggerConfiguration()
           .Enrich.FromLogContext()
           .WriteTo.File(path: Path.Combine(Environment.CurrentDirectory, "mmbot.log"), outputTemplate: logTemplate, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
           .CreateLogger();

try
{
    using var hb = DiscordClientHost.CreateDiscordClientHost(args).Build();
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