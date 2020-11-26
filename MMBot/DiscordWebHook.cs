using System;
using System.Threading.Tasks;
using Discord.Webhook;
using Microsoft.Extensions.Hosting;

namespace MMBot
{
    public class DiscordWebHook
    {
        public static IHostBuilder CreateDiscordWebhookHost(string[] args, ulong id, string token)
            => Host.CreateDefaultBuilder(args)
                    .ConfigureServices(async (c, x) =>
                    {
                        var client = new DiscordWebhookClient(id, token);
                        while (1 > 0)
                        {
                            await client.SendMessageAsync("Test Description https://forms.gle/CFZ92wrzqntYyemu9");
                            await Task.Delay(TimeSpan.FromSeconds(30));
                        }
                    });
    }
}
