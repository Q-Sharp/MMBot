using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MMBot.Data.Enums;
using MMBot.Helpers;
using MMBot.Modules.Interfaces;
using MMBot.Services.Interfaces;

namespace MMBot.Modules.Admin
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
            var csvFile = Context.Message.Attachments.FirstOrDefault();
            var myWebClient = _clientFactory.CreateClient();
            if (csvFile != null)
            {
                var csv = await myWebClient.GetAsync(csvFile.Url);
                if (_csvService != null)
                {
                    var csvByte = await csv.Content.ReadAsByteArrayAsync();
                    var result = await _csvService.ImportCsv(csvByte, Context.Guild.Id);

                    if(result == null)
                        File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "lastImport.csv"), csvByte);

                    await ReplyAsync(result == null
                        ? "CSV file import was successful"
                        : $"ERROR: {result.Message}");
                }
            }

            await _databaseService.CleanDB();
        }

        [Command("ReImportCSV")]
        [Alias("Reimport")]
        [Summary("Reimport last csv file")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ReImportCSV()
        {
            var csv = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, "lastImport.csv"));
            if (_csvService != null)
            {
                var result = await _csvService.ImportCsv(csv, Context.Guild.Id);

                await ReplyAsync(result == null
                    ? "CSV file import was successful"
                    : $"ERROR: {result.Message}");
            }

            await _databaseService.CleanDB();
        }

        [Command("ExportCSV")]
        [Alias("export")]
        [Summary("Exports a csv file from db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ExportCsv()
        {
            var settings = await _guildSettings.GetGuildSettingsAsync(Context.Guild.Id);

            var result = await _csvService?.ExportCsv(Context.Guild.Id);
            await File.WriteAllBytesAsync(settings.FileName, result);

            await Context.Channel.SendFileAsync(settings.FileName, "Csv db export");
            File.Delete(settings.FileName);
        }

        [Command("Reorder")]
        [Alias("reorder")]
        [Summary("Reorders member in db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ReorderJoin()
        {
            await Task.Run(() => _adminService.Reorder(Context.Guild.Id));
            await ReplyAsync("Members join order updated!");
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
                catch
                {
                    throw;
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
            var gs = (await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id));
            var e = gs.GetEmbedPropertiesWithValues();
            await ReplyAsync("", false, e as Embed);
        }

        [Command("Set")]
        [Summary("Set guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Set(string propertyName, [Remainder] string value)
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id));
            var r = gs.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            await ReplyAsync(r);
        }

        [Command("Get")]
        [Summary("Get guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Get(string propertyName)
        {
            var gs = await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id);
            var r = gs.GetProperty(propertyName);
            await ReplyAsync(r);
        }
    }
}
