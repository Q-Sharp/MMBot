using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Data.Enums;
using TTMMBot.Helpers;
using TTMMBot.Modules.Interfaces;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules.Admin
{
    [Name("Admin")]
    [Group("Admin")]
    [Alias("admin", "a", "A")]
    public partial class AdminModule : ModuleBase<SocketCommandContext>, IAdminModule
    {
        private static volatile bool _commandIsRunning;
        
        private readonly IDatabaseService _databaseService;
        private readonly INotionCsvService _csvService;
        private readonly IAdminService _adminService;
        private readonly IGlobalSettingsService _globalSettings;
        private readonly ICommandHandler _commandHandler;

        public AdminModule(IDatabaseService databaseService, INotionCsvService csvService, IAdminService adminService, IGlobalSettingsService globalSettings, ICommandHandler commandHandler)
        {
            _databaseService = databaseService;
            _csvService = csvService;
            _adminService = adminService;
            _globalSettings = globalSettings;
            _commandHandler = commandHandler;
        }

        [Command("ImportCSV", RunMode = RunMode.Async)]
        [Alias("import")]
        [Summary("Imports a notion csv export to update db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
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
                    if (_csvService != null)
                    {
                        var result = await _csvService.ImportCsv(csv);

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

        [Command("ReImportCSV")]
        [Alias("Reimport")]
        [Summary("Reimport last csv file")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
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
                if (_csvService != null)
                {
                    var result = await _csvService.ImportCsv(csv);

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

        [Command("ExportCSV")]
        [Alias("export")]
        [Summary("Exports a csv file from db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ExportCsv()
        {
            try
            {
                if (_csvService != null)
                {
                    var result = await _csvService?.ExportCsv();
                    await File.WriteAllBytesAsync(_globalSettings.FileName, result);
                }

                await Context.Channel.SendFileAsync(_globalSettings.FileName, "Csv db export");
                File.Delete(_globalSettings.FileName);
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Reorder")]
        [Alias("reorder")]
        [Summary("Reorders member in db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ReorderJoin()
        {
            try
            {
                await Task.Run(() => _adminService.Reorder());
                await ReplyAsync("Members join order updated!");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("FixRoles", RunMode = RunMode.Async)]
        [Alias("FR")]
        [Summary("Checks and fixes discord roles of all clan members.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
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

                    var c = await _databaseService.LoadClansAsync();
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
    }
}
