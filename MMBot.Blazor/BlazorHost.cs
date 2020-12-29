using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MMBot.Blazor
{
    public static class BlazorHost
    {
        public static IHostBuilder CreateHostBuilder(string[] args, string dbFilePath, string wwwRootPath) =>
            (Host.CreateDefaultBuilder(args))
                .ConfigureLogging(x => x.ClearProviders().AddSerilog(Log.Logger))
                .ConfigureAppConfiguration(c => 
                {
                    var kv = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("db", dbFilePath) };
                    c.AddInMemoryCollection(kv);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                              .UseContentRoot(wwwRootPath);
                });
    }
}
