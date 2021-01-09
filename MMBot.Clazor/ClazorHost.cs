using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MMBot.Data;
using MMBot.Data.Services;
using MMBot.Data.Services.Interfaces;
using Serilog;

namespace MMBot.Clazor
{
    public static class ClazorHost
    {
        public static WebAssemblyHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddDbContext<Context>(o => o.UseSqlite($"Data Source={builder.Configuration.GetValue<string>("db")}"));
            builder.Services.AddScoped<IDatabaseService, DatabaseService>();

            return builder;
        }
    }
}
