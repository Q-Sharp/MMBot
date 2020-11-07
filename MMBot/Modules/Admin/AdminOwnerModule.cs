using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MMBot.Modules.Interfaces;
using MMBot.Helpers;
using System;

namespace MMBot.Modules.Admin
{
    public partial class AdminModule : MMBotModule, IAdminModule
    {
        private readonly string _backupDir = Path.Combine(".", "backup");
        private readonly string _export = "dataexport.zip";
        private readonly string _import = "dataexport.zip";

        [Command("ExportDb")]
        [Summary("Exports db data as json files in zip archive")]
        [RequireOwner()]
        public async Task<RuntimeResult> ExportDb()
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

            return FromSuccess();
        }

        [Command("ImportDb")]
        [Summary("Imports the db with help of json files in zip archive")]
        [RequireOwner()]
        public async Task<RuntimeResult> ImportDb()
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

                if(result)
                    await Restart();
            }

            return FromSuccess();
        }


        [Command("Restart")]
        [Alias("restart")]
        [Summary("Restarts the bot")]
        [RequireOwner]
        public async Task<RuntimeResult> Restart(bool saveRestart = true)
        {
           if (saveRestart)
            {
                var r = await _databaseService?.AddRestart();
                r.Guild = Context.Guild.Id;
                r.Channel = Context.Channel.Id;
                await _databaseService?.SaveDataAsync();
            }

            var t = _adminService.Restart();
            await ReplyAsync($"Bot service is restarting...");
            await t;
            return FromSuccess();
        }

        [Command("DeleteDB")]
        [Summary("Deletes sqlite db file")]
        [RequireOwner]
        public async Task<RuntimeResult> DeleteDb()
        {
            await _adminService.DeleteDb();
            await ReplyAsync($"db file has been deleted.");
            await Restart(false);
            return FromSuccess();
        }

        [Command("AddChannelToUrlScan")]
        [Summary("Adds channel to url scan list")]
        [RequireOwner]
        public async Task<RuntimeResult> AddChannelToUrlScan(IGuildChannel channel, IGuildChannel qChannel)
        {
            await _commandHandler.AddChannelToGoogleFormsWatchList(channel, qChannel);

            var ch = await _databaseService.LoadChannelsAsync();

            if(!ch.Any(c => c.GuildId == channel.GuildId && c.TextChannelId == channel.Id))
            {
                var dbChannel = await _databaseService.CreateChannelAsync(Context.Guild.Id);
                dbChannel.GuildId = channel.GuildId;
                dbChannel.TextChannelId = channel.Id;
                dbChannel.AnswerTextChannelId = qChannel.Id;
                await _databaseService.SaveDataAsync();
            }

            return FromSuccess($"Successfully added {channel.Name} to UrlScanList and {qChannel.Name} for any questions!");
        }

        [Command("RemoveChannelFromUrlScan")]
        [Summary("Removes channel from url scan list")]
        [RequireOwner]
        public async Task<RuntimeResult> RemoveChannelFromUrlScan(IGuildChannel channel)
        {
            await _commandHandler.RemoveChannelFromGoogleFormsWatchList(channel);

            var c = (await _databaseService.LoadChannelsAsync()).FirstOrDefault(x => x.GuildId == channel.GuildId && x.TextChannelId == channel.Id);

            if(c != null)
                _databaseService.DeleteChannel(c, Context.Guild.Id);

            return FromSuccess($"Successfully removed {channel.Name} to UrlScanList.");
        }

        [Command("ListUrlScanChannel")]
        [Summary("Lists all url scan channel")]
        [RequireOwner]
        public async Task<RuntimeResult> ListChannelFromUrlScan()
        {
            var cs = await _databaseService.LoadChannelsAsync(Context.Guild.Id);

            if(cs != null)
            {
                string result = "Channel   |   QuestionsChannel";
                foreach(var c in cs)
                {
                    var tc = Context.Client?.GetGuild(c.GuildId)?.GetTextChannel(c.TextChannelId);
                    var tqc = Context.Client?.GetGuild(c.GuildId)?.GetTextChannel(c.AnswerTextChannelId);

                    if(tc == null || tqc == null)
                    {
                        _databaseService.DeleteChannel(c, Context.Guild.Id);
                        await _databaseService.SaveDataAsync();
                        continue;
                    }

                    result += $"#{tc.Name} | #{tqc.Name} {Environment.NewLine}";
                }
                return FromSuccess(result.TrimEnd(Environment.NewLine.ToArray()));
            }
            return FromErrorObjectNotFound("UrlScanChannels", "Any");
        }
    }
}
