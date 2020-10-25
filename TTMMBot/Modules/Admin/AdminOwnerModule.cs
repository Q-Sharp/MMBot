using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.IO.Compression;
using TTMMBot.Helpers;
using TTMMBot.Modules.Interfaces;

namespace TTMMBot.Modules.Admin
{
    public partial class AdminModule : MMBotModule, IAdminModule
    {
        [Command("ExportDb")]
        [Summary("Exports db data as json")]
        [RequireOwner()]
        public async Task ExportDb()
        {
            try
            {
                var json = await _jsonService.ExportDBToJson();

                var ex = await Task.Run(() => 
                {
                    var backupDir = Path.Combine(".", "backup");
                    var export = "dataexport.zip";

                    Directory.CreateDirectory(backupDir);
                   
                    foreach(var entry in json)
                        File.WriteAllText(Path.Combine(backupDir, $"{entry.Key}.json"), entry.Value);

                    ZipFile.CreateFromDirectory(backupDir, export);

                    Directory.Delete(backupDir, true);
                    return export;
                });

                await Context.Channel.SendFileAsync(ex, "dbExport");
                File.Delete(ex);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }


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
            var gs = (await _databaseService.LoadGuildSettingsAsync());
            var e = gs.GetEmbedPropertiesWithValues();
            await ReplyAsync("", false, e as Embed);
        }

        [Command("Set")]
        [Summary("Set Global settings")]
        [RequireOwner]
        public async Task Set(string propertyName, [Remainder] string value)
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync());
            var r = gs.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            await ReplyAsync(r);
        }

        [Command("Get")]
        [Summary("Get Global setting")]
        [RequireOwner]
        public async Task Get(string propertyName)
        {
            var gs = await _databaseService.LoadGuildSettingsAsync();
            var r = gs.GetProperty(propertyName);
            await ReplyAsync(r);
        }

        [Command("AddChannelToUrlScan")]
        [Summary("Adds channel to url scan list")]
        [RequireOwner]
        public async Task AddChannelToUrlScan(IGuildChannel channel)
        {
            await _commandHandler.AddChannelToGoogleFormsWatchList(channel);

            var dbChannel = await _databaseService.CreateChannelAsync();
            dbChannel.GuildId = channel.GuildId;
            dbChannel.TextChannelId = channel.Id;
            await _databaseService.SaveDataAsync();

            await ReplyAsync($"Successfully added {channel.Name} to UrlScanList.");
        }

        [Command("RemoveChannelFromUrlScan")]
        [Summary("Removes channel from url scan list")]
        [RequireOwner]
        public async Task RemoveChannelFromUrlScan(IGuildChannel channel)
        {
            await _commandHandler.RemoveChannelFromGoogleFormsWatchList(channel);

            var c = (await _databaseService.LoadChannelsAsync()).FirstOrDefault(x => x.GuildId == channel.GuildId && x.TextChannelId == channel.Id);
            _databaseService.DeleteChannel(c);

            await ReplyAsync($"Successfully removed {channel.Name} to UrlScanList.");
        }
    }
}
