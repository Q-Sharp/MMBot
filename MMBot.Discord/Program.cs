using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace MMBot.Discord
{
    public class Program
    {
        private const string logTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
        public static async Task Main(string[] args)
        {
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
}
