using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TTMMBot.Data.Entities;
using TTMMBot.Data.Enums;
using TTMMBot.Helpers;
using TTMMBot.Services;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules
{
    [Name("Admin")]
    [Group("Admin")]
    [Alias("admin", "a", "A")]
    public class AdminModule : ModuleBase<SocketCommandContext>, IAdminModule
    {
        private static volatile bool _commandIsRunning;

        public IDatabaseService DatabaseService { get; set; }
        public INotionCsvService CsvService { get; set; }
        public IAdminService AdminService { get; set; }
        public GlobalSettingsService GlobalSettings { get; set; }
        public ICommandHandler CommandHandler { get; set; }

        [RequireOwner]
        [Command("ImportCSV", RunMode = RunMode.Async)]
        [Alias("import")]
        [Summary("Imports a notion csv export to update db")]
        public async Task ImportCsv()
        {
            try
            {
                if (_commandIsRunning)
                {
                    await ReplyAsync($"I can only run a single long running command at a time!");
                    return;
                }

                var csvFile = Context.Message.Attachments.FirstOrDefault();
                var myWebClient = new WebClient();
                if (csvFile != null)
                {
                    var csv = myWebClient.DownloadData(csvFile.Url);
                    if (CsvService != null)
                    {
                        var result = await CsvService.ImportCsv(csv);

                        if(result == null)
                            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "lastImport.csv"), csv);

                        await ReplyAsync(result == null
                            ? "CSV file import was successful"
                            : $"ERROR: {result.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
            finally
            {
                _commandIsRunning = false;
            }
        }

        [RequireOwner]
        [Command("ReImportCSV")]
        [Alias("Reimport")]
        [Summary("Reimport last csv file")]
        public async Task ReImportCSV()
        {
            try
            {
                if (_commandIsRunning)
                {
                    await ReplyAsync($"I can only run a single long running command at a time!");
                    return;
                }

                var csv = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, "lastImport.csv"));
                if (CsvService != null)
                {
                    var result = await CsvService.ImportCsv(csv);

                    await ReplyAsync(result == null
                        ? "CSV file import was successful"
                        : $"ERROR: {result.Message}");
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [RequireOwner]
        [Command("ExportCSV")]
        [Alias("export")]
        [Summary("Exports a csv file from db")]
        public async Task ExportCsv()
        {
            try
            {
                if (CsvService != null)
                {
                    var result = await CsvService?.ExportCsv();
                    await File.WriteAllBytesAsync(GlobalSettings.FileName, result);
                }

                await Context.Channel.SendFileAsync(GlobalSettings.FileName, "Csv db export");
                File.Delete(GlobalSettings.FileName);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [RequireOwner]
        [Command("Reorder")]
        [Alias("reorder")]
        [Summary("Reorders member in db")]
        public async Task ReorderJoin()
        {
            try
            {
                await Task.Run(() => AdminService.Reorder());
                await ReplyAsync("Members join order updated!");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [RequireOwner]
        [Command("Restart")]
        [Alias("restart")]
        [Summary("Restarts the bot")]
        public async Task Restart(bool saveRestart = true)
        {
            if(saveRestart)
            {
                var r = await DatabaseService?.AddRestart();
                r.Guild = Context.Guild.Id;
                r.Channel = Context.Channel.Id;
                await DatabaseService?.SaveDataAsync();
            }

            Process.Start(AppDomain.CurrentDomain.FriendlyName);
            await ReplyAsync($"Bot service is restarting...");
            
            Environment.Exit(0);
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("FixRoles", RunMode = RunMode.Async)]
        [Alias("FR")]
        [Summary("Checks and fixes discord roles of all clan members.")]
        public async Task FixRoles() =>
            await Task.Run(async () =>
            {
                try
                {
                    if (_commandIsRunning)
                    {
                        await ReplyAsync($"I can only run a single long running command at a time!");
                        return;
                    }

                    _commandIsRunning = true;
                    await ReplyAsync($"Fixing roles of discord members accordingly to their clan membership....");

                    var c = await DatabaseService.LoadClansAsync();
                    var ar = Context.Guild.Roles.Where(x => c.Select(clan => clan.DiscordRole).Contains(x.Name))
                        .ToArray();

                    var clanMessage = await ReplyAsync("...");
                    foreach (var clan in c)
                    {
                        await clanMessage.ModifyAsync(m => m.Content = $"Fixing roles of members of {clan}....");
                        var clanRole = Context.Guild.Roles.FirstOrDefault(x => x.Name == clan.DiscordRole) as IRole;

                        var memberMessage = await ReplyAsync("...");
                        foreach (var member in clan.Member)
                        {
                            await memberMessage.ModifyAsync(m => m.Content = $"Fixing roles of {member}...");
                            var user = await Task.Run(() =>
                                Context.Guild.Users.FirstOrDefault(x =>
                                    $"{x.Username}#{x.Discriminator}" == member.Discord));

                            if (user is null || member.ClanId is null || clan.DiscordRole is null)
                                continue;

                            try
                            {
                                if (member.Role == Role.CoLeader || member.Role == Role.Leader)
                                {
                                    await user.RemoveRolesAsync(ar);
                                    await user.AddRolesAsync(ar);
                                }
                                else
                                {
                                    await user.RemoveRolesAsync(ar);
                                    await user.AddRoleAsync(clanRole);
                                }
                            }
                            catch (Exception e)
                            {
                                await ReplyAsync($"{member.Name}'s role couldn't be fixed: {e.Message}");
                            }
                        }
                    }

                    await ReplyAsync($"All roles have been fixed!");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}");
                }
                finally
                {
                    _commandIsRunning = false;
                }
            });

        [RequireOwner]
        [Command("DeleteDB")]
        [Alias("deletedb")]
        [Summary("Deletes sqlite db file")]
        public async Task DeleteDb() => await Task.Run(async () =>
        {
            var db = $"{Path.Combine(Directory.GetCurrentDirectory(), "TTMMBot.db")}";
            File.Delete(db);
            await ReplyAsync($"{db} has been deleted.");
            await Restart(false);
        });

        [RequireOwner]
        [Command("Show")]
        [Summary("Show Global settings")]
        public async Task Show()
        {
            var gs = (await DatabaseService.LoadGlobalSettingsAsync());
            var e = gs.GetEmbedPropertiesWithValues();
            await ReplyAsync("", false, e as Embed);
        }

        [RequireOwner]
        [Command("Set")]
        [Summary("Set Global settings")]
        public async Task Set(string propertyName, [Remainder] string value)
        {
            var gs = (await DatabaseService.LoadGlobalSettingsAsync());
            var r = gs.ChangeProperty(propertyName, value);
            await DatabaseService.SaveDataAsync();
            await ReplyAsync(r);
        }

        [RequireOwner]
        [Command("AddChannelToUrlScan")]
        [Summary("Adds channel to url scan list")]
        public async Task AddChannelToUrlScan(IGuildChannel channel)
        {
            await ReplyAsync("Not implemented!");
            await CommandHandler.AddChannelToGoogleFormsWatchList(channel);
        }

        [RequireOwner]
        [Command("RemoveChannelFromUrlScan")]
        [Summary("Removes channel from url scan list")]
        public async Task RemoveChannelFromUrlScan(IGuildChannel channel)
        {
            await ReplyAsync("Not implemented!");
            await CommandHandler.RemoveChannelFromGoogleFormsWatchList(channel);
        }
    }
}
