using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Helpers;
using TTMMBot.Modules.Interfaces;

namespace TTMMBot.Modules.Admin
{
    public partial class AdminModule : ModuleBase<SocketCommandContext>, IAdminModule
    {
        [Command("Restart")]
        [Alias("restart")]
        [Summary("Restarts the bot")]
        [RequireOwner]
        public async Task Restart(bool saveRestart = true)
        {
            if(saveRestart)
            {
                var r = await _databaseService?.AddRestart();
                r.Guild = Context.Guild.Id;
                r.Channel = Context.Channel.Id;
                await _databaseService?.SaveDataAsync();
            }

            Process.Start(AppDomain.CurrentDomain.FriendlyName);
            await ReplyAsync($"Bot service is restarting...");
            
            Environment.Exit(0);
        }

        [Command("DeleteDB")]
        [Summary("Deletes sqlite db file")]
        [RequireOwner]
        public async Task DeleteDb() => await Task.Run(async () =>
        {
            var db = $"{Path.Combine(Directory.GetCurrentDirectory(), "TTMMBot.db")}";
            File.Delete(db);
            await ReplyAsync($"{db} has been deleted.");
            await Restart(false);
        });

        [Command("Show")]
        [Summary("Show Global settings")]
        [RequireOwner]
        public async Task Show()
        {
            var gs = (await _databaseService.LoadGlobalSettingsAsync());
            var e = gs.GetEmbedPropertiesWithValues();
            await ReplyAsync("", false, e as Embed);
        }

        [RequireOwner]
        [Command("Set")]
        [Summary("Set Global settings")]
        public async Task Set(string propertyName, [Remainder] string value)
        {
            var gs = (await _databaseService.LoadGlobalSettingsAsync());
            var r = gs.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            await ReplyAsync(r);
        }

        [Command("AddChannelToUrlScan")]
        [Summary("Adds channel to url scan list")]
        [RequireOwner]
        public async Task AddChannelToUrlScan(IGuildChannel channel)
        {
            await ReplyAsync("Not implemented!");
            await _commandHandler.AddChannelToGoogleFormsWatchList(channel);
        }

        [Command("RemoveChannelFromUrlScan")]
        [Summary("Removes channel from url scan list")]
        [RequireOwner]
        public async Task RemoveChannelFromUrlScan(IGuildChannel channel)
        {
            await ReplyAsync("Not implemented!");
            await _commandHandler.RemoveChannelFromGoogleFormsWatchList(channel);
        }
    }
}
