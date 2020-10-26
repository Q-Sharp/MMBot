using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using TTMMBot.Data.Enums;
using TTMMBot.Helpers;
using TTMMBot.Modules.Interfaces;
using TTMMBot.Services.IE;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules.Admin
{
    [Name("Admin")]
    [Group("Admin")]
    [Alias("admin", "a", "A")]
    public partial class AdminModule : MMBotModule, IAdminModule
    {
        private static long _commandIsRunning = 0;
        
        private readonly ICsvService _csvService;
        private readonly IAdminService _adminService;
        private readonly IJsonService _jsonService;
        private readonly IHttpClientFactory _clientFactory;

        public AdminModule(IDatabaseService databaseService, ICsvService csvService, IAdminService adminService, IGuildSettingsService guildSettings, ICommandHandler commandHandler, IJsonService jsonService, IHttpClientFactory clientFactory)
            : base(databaseService, guildSettings, commandHandler)
        {
            _csvService = csvService;
            _adminService = adminService;
            _jsonService = jsonService;
            _clientFactory = clientFactory;
        }

        [Command("ImportCSV")]
        [Alias("import")]
        [Summary("Imports a notion csv export to update db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ImportCsv()
        {
            try
            {
                var csvFile = Context.Message.Attachments.FirstOrDefault();
                var myWebClient = _clientFactory.CreateClient();
                if (csvFile != null)
                {
                    var csv = await myWebClient.GetAsync(csvFile.Url);
                    if (_csvService != null)
                    {
                        var csvByte = await csv.Content.ReadAsByteArrayAsync();
                        var result = await _csvService.ImportCsv(csvByte);

                        if(result == null)
                            File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "lastImport.csv"), csvByte);

                        await ReplyAsync(result == null
                            ? "CSV file import was successful"
                            : $"ERROR: {result.Message}");
                    }
                }

                await _databaseService.CleanDB();
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
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
                var result = await _csvService?.ExportCsv();
                await File.WriteAllBytesAsync(_guildSettings.FileName, result);

                await Context.Channel.SendFileAsync(_guildSettings.FileName, "Csv db export");
                File.Delete(_guildSettings.FileName);
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

        [Command("FixRoles")]
        [Alias("FR")]
        [Summary("Checks and fixes discord roles of all clan members.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task FixRoles() =>
            await Task.Run(async () =>
            {
                try
                {
                    if (Interlocked.Read(ref _commandIsRunning) > 0)
                    {
                        await ReplyAsync($"I can only run a single long running command at a time!");
                        return;
                    }

                    Interlocked.Increment(ref _commandIsRunning);
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
                            var user = await Task.Run(async () =>
                               (await Context.Guild.GetUsersAsync()).FirstOrDefault(x =>
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
                    Interlocked.Decrement(ref _commandIsRunning);
                }
            });

        [Command("Show")]
        [Summary("Show guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Show()
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync());
            var e = gs.GetEmbedPropertiesWithValues();
            await ReplyAsync("", false, e as Embed);
        }

        [Command("Set")]
        [Summary("Set guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Set(string propertyName, [Remainder] string value)
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync());
            var r = gs.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            await ReplyAsync(r);
        }

        [Command("Get")]
        [Summary("Get guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Get(string propertyName)
        {
            var gs = await _databaseService.LoadGuildSettingsAsync();
            var r = gs.GetProperty(propertyName);
            await ReplyAsync(r);
        }
    }
}
