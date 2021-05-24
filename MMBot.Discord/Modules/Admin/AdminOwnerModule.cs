using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Helpers;
using System;
using MMBot.Discord.Services.CommandHandler;

namespace MMBot.Discord.Modules.Admin
{
    public partial class AdminModule : MMBotModule, IAdminModule
    {
        private readonly string _backupDir = Path.Combine(".", "backup");
        private readonly string _export = "dataexport.zip";

        [Command("ExportDb")]
        [Summary("Exports db data as json files in zip archive")]
        [RequireOwner()]
        public async Task<RuntimeResult> ExportDb()
        {
            string ex = string.Empty;

            try
            {
                var json = await _jsonService.ExportDBToJson();

                if (File.Exists(_export))
                    File.Delete(_export);

                ex = await Task.Run(async () =>
                {
                    Directory.CreateDirectory(_backupDir);

                    foreach (var entry in json)
                        await File.WriteAllTextAsync(Path.Combine(_backupDir, $"{entry.Key}.json"), entry.Value);

                    ZipFile.CreateFromDirectory(_backupDir, _export);
                    return _export;
                });

                await Context.Channel.SendFileAsync(ex, "dbExport");

                return FromSuccess();
            }
            catch(Exception e)
            {
                 return FromError(CommandError.Unsuccessful, $"Export Exception. {e.Message}");
            }
            finally
            {
                 Directory.Delete(_backupDir, true);
                 File.Delete(ex);
            }
        }

        [Command("ImportDb")]
        [Summary("Imports the db with help of json files in zip archive")]
        [RequireOwner()]
        public async Task<RuntimeResult> ImportDb()
        {
            var csvFile = Context.Message.Attachments.OrderBy(c => c.Id).FirstOrDefault();
            var myWebClient = _clientFactory.CreateClient();

            if (csvFile is not null)
            {
                await ReplyAsync("Starting db import this can take a while...");

                var zip = await myWebClient.GetAsync(csvFile.Url);
                var zipByte = await zip.Content.ReadAsByteArrayAsync();

                _adminService.Truncate();
                await ReplyAsync($"db is empty now.");

                await ReplyAsync("Importing data now....");
                await _adminService.DataImport(zipByte);

                return FromSuccess("Database import was successful.");
            }

            return FromErrorUnsuccessful("No attachment file found!");
        }


        [Command("Restart")]
        [Alias("restart")]
        [Summary("Restarts the bot")]
        [RequireOwner]
        public async Task<RuntimeResult> Restart(bool saveRestart = true)
        {
            await ReplyAsync("Restarting.....");
            await _adminService.Restart(saveRestart, Context.Guild.Id, Context.Channel.Id);
            return FromSuccess();
        }

        [Command("TruncateDb")]
        [Summary("Clears db")]
        [RequireOwner]
        public async Task<RuntimeResult> TruncateDb()
        {
            _adminService.Truncate();
            await ReplyAsync($"db is empty now.");
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
                var dbChannel = _databaseService.CreateChannel(Context.Guild.Id);
                dbChannel.GuildId = channel.GuildId;
                dbChannel.TextChannelId = channel.Id;
                dbChannel.AnswerTextChannelId = qChannel.Id;
                await _databaseService.SaveDataAsync();

                return FromSuccess($"Successfully added {channel.Name} to UrlScanList and {qChannel.Name} for any questions!");
            }

            return FromErrorUnsuccessful($"{channel.Name} is already on the UrlScanList!");
        }

        [Command("RemoveChannelFromUrlScan")]
        [Summary("Removes channel from url scan list")]
        [RequireOwner]
        public async Task<RuntimeResult> RemoveChannelFromUrlScan(IGuildChannel channel)
        {
            await _commandHandler.RemoveChannelFromGoogleFormsWatchList(channel);

            var c = (await _databaseService.LoadChannelsAsync(Context.Guild.Id)).FirstOrDefault(x => x.TextChannelId == channel.Id);

            if(c is not null)
            {
                _databaseService.DeleteChannel(c);
                await _databaseService.SaveDataAsync();
                return FromSuccess($"Successfully removed {channel.Name} from UrlScanList.");
            }
            
            return FromErrorObjectNotFound(nameof(channel), channel.Name);
        }

        [Command("ListUrlScanChannel")]
        [Summary("Lists all url scan channel")]
        [RequireOwner]
        public async Task<RuntimeResult> ListChannelFromUrlScan()
        {
            var cs = await _databaseService.LoadChannelsAsync(Context.Guild.Id);

            if(cs is not null)
            {
                var result = "Channel   |   QuestionsChannel";
                foreach(var c in cs)
                {
                    var tc = Context.Client?.GetGuild(c.GuildId)?.GetTextChannel(c.TextChannelId);
                    var tqc = Context.Client?.GetGuild(c.GuildId)?.GetTextChannel(c.AnswerTextChannelId);

                    if(tc is null || tqc is null)
                    {
                        _databaseService.DeleteChannel(c);
                        await _databaseService.SaveDataAsync();
                        continue;
                    }

                    result += $"{Environment.NewLine}#{tc.Name} | #{tqc.Name}";
                }
                return FromSuccess(result);
            }
            return FromErrorObjectNotFound("UrlScanChannels", "Any");
        }

        [Command("SetDeletedMessageChannel")]
        [RequireOwner]
        public async Task<RuntimeResult> SetDeletedMessageChannel(IGuildChannel channel)
        {
            await Task.Run(() => (_commandHandler as CommandHandler).SetDeletedMessagesChannel(channel));
            return FromSuccess();
        }
    }
}
