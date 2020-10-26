using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TTMMBot.Modules.Interfaces;

namespace TTMMBot.Modules.Admin
{
    public partial class AdminModule : MMBotModule, IAdminModule
    {
        private string _backupDir = Path.Combine(".", "backup");
        private string _export = "dataexport.zip";
        private string _import = "dataexport.zip";

        [Command("ExportDb")]
        [Summary("Exports db data as json files in zip archive")]
        [RequireOwner()]
        public async Task ExportDb()
        {
            try
            {
                var json = await _jsonService.ExportDBToJson();

                var ex = await Task.Run(async () =>
                {
                    Directory.CreateDirectory(_backupDir);

                    foreach (var entry in json)
                        await File.WriteAllTextAsync(Path.Combine(_backupDir, $"{entry.Key}.json"), entry.Value);

                    ZipFile.CreateFromDirectory(_backupDir, _export);

                    Directory.Delete(_backupDir, true);
                    return _export;
                });

                await Context.Channel.SendFileAsync(ex, "dbExport");
                File.Delete(ex);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("ImportDb")]
        [Summary("Imports the db with help of json files in zip archive")]
        [RequireOwner()]
        public async Task ImportDb()
        {
            try
            {
                await ReplyAsync("Starting db import this can take a while...");

                var csvFile = Context.Message.Attachments.FirstOrDefault();
                var myWebClient = _clientFactory.CreateClient();

                if (csvFile != null)
                {
                    var zip = await myWebClient.GetAsync(csvFile.Url);
                    var zipByte = await zip.Content.ReadAsByteArrayAsync();

                    await File.WriteAllBytesAsync(_import, zipByte);

                    var dict = await Task.Run(async () =>
                    {
                        Directory.CreateDirectory(_backupDir);

                        ZipFile.ExtractToDirectory(_import, _backupDir);

                        var dict = new Dictionary<string, string>();

                        foreach (var entry in Directory.GetFiles(_backupDir))
                            dict.Add(Path.GetFileNameWithoutExtension(entry), await File.ReadAllTextAsync(entry));

                        Directory.Delete(_backupDir, true);
                        return dict;
                    });

                    var result = await _jsonService.ImportJsonToDB(dict);
                    File.Delete(_import);
                    await ReplyAsync(result ? "db import completed!" : "error in db import!");
                }
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
            if (saveRestart)
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

        [Command("AddChannelToUrlScan")]
        [Summary("Adds channel to url scan list")]
        [RequireOwner]
        public async Task AddChannelToUrlScan(IGuildChannel channel, IGuildChannel qChannel)
        {
            _commandHandler.AddChannelToGoogleFormsWatchList(channel, qChannel);

            var ch = await _databaseService.LoadChannelsAsync();

            if(!ch.Any(c => c.GuildId == channel.GuildId && c.TextChannelId == channel.Id))
            {
                var dbChannel = await _databaseService.CreateChannelAsync();
                dbChannel.GuildId = channel.GuildId;
                dbChannel.TextChannelId = channel.Id;
                dbChannel.AnswerTextChannelId = qChannel.Id;
                await _databaseService.SaveDataAsync();
            }
            await ReplyAsync($"Successfully added {channel.Name} to UrlScanList and {qChannel.Name} for any questions!");
        }

        [Command("RemoveChannelFromUrlScan")]
        [Summary("Removes channel from url scan list")]
        [RequireOwner]
        public async Task RemoveChannelFromUrlScan(IGuildChannel channel)
        {
            _commandHandler.RemoveChannelFromGoogleFormsWatchList(channel);

            var c = (await _databaseService.LoadChannelsAsync()).FirstOrDefault(x => x.GuildId == channel.GuildId && x.TextChannelId == channel.Id);

            if(c != null)
                _databaseService.DeleteChannel(c);

            await ReplyAsync($"Successfully removed {channel.Name} to UrlScanList.");
        }
    }
}
