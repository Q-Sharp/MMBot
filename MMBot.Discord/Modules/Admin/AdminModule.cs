using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MMBot.Helpers;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;
using MMBot.Data.Services.Interfaces;

namespace MMBot.Discord.Modules.Admin
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
        public async Task<RuntimeResult> ImportCsv()
        {
            var csvFile = Context.Message.Attachments.FirstOrDefault();
            var myWebClient = _clientFactory.CreateClient();
            string fileName = null;

            if (csvFile is not null && _csvService is not null)
            {
                try
                {
                    var csv = await myWebClient.GetAsync(csvFile.Url);
                    var csvByte = await csv.Content.ReadAsByteArrayAsync();
                    var result = await _csvService.ImportCsv(csvByte, Context.Guild.Id);
                    fileName = $"{Context.Guild.Id}lastImport.csv";

                    if(result is null)
                        File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, fileName), csvByte);

                    await ReplyAsync(result is null
                        ? "CSV file import was successful"
                        : $"ERROR: {result.Message}");
                }
                finally
                {
                    File.Delete(fileName);
                }
            }

            await _databaseService.CleanDB();
            return FromSuccess();
        }

        [Command("ExportCSV")]
        [Alias("export")]
        [Summary("Exports a csv file from db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> ExportCsv()
        {
            var settings = await _guildSettings.GetGuildSettingsAsync(Context.Guild.Id);

            var result = await _csvService?.ExportCsv(Context.Guild.Id);
            await File.WriteAllBytesAsync(settings.FileName, result);

            await Context.Channel.SendFileAsync(settings.FileName, "Csv db export");
            File.Delete(settings.FileName);
            return FromSuccess();
        }

        [Command("Reorder")]
        [Alias("reorder")]
        [Summary("Reorders member in db")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> ReorderJoin()
        {
            await Task.Run(() => _adminService.Reorder(Context.Guild.Id));
            await ReplyAsync("Members join order updated!");
            return FromSuccess();
        }

        [Command("FixRoles")]
        [Alias("FR")]
        [Summary("Checks and fixes discord roles of all clan members.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> FixRoles()
        {
            try
            {
                if (Interlocked.Read(ref _commandIsRunning) > 0)
                    return FromError(CommandError.Unsuccessful, $"I can only run a single long running command at a time!");

                Interlocked.Increment(ref _commandIsRunning);
                await ReplyAsync($"Fixing roles of discord members accordingly to their clan membership....");

                var c = await _databaseService.LoadClansAsync(Context.Guild.Id);
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
                            await user.RemoveRolesAsync(ar);
                            await user.AddRoleAsync(clanRole);
                        }
                        catch (Exception e)
                        {
                            await ReplyAsync($"{member.Name}'s role couldn't be fixed: {e.Message}");
                        }
                    }
                }

                //var members = await _databaseService.LoadMembersAsync();

                //foreach(var m in members)
                //{

                //}

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

            return FromSuccess();
        }

        [Command("Show")]
        [Summary("Show guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Show()
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id));
            var e = gs.GetEmbedPropertiesWithValues();
            await ReplyAsync("", false, e as Embed);
            return FromSuccess();
        }

        [Command("Set")]
        [Summary("Set guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Set(string propertyName, [Remainder] string value)
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id));
            var r = gs.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            return FromSuccess(r);
        }

        [Command("Get")]
        [Summary("Get guild settings")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Get(string propertyName)
        {
            var gs = await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id);
            var r = gs.GetProperty(propertyName);
            await ReplyAsync(r);
            return FromSuccess();
        }

        [Command("FillForm")]
        [Summary("Fills the form from the link with playertags")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> FillForm(string url)
        {
            try
            {
                var pt = (await _databaseService.LoadMembersAsync(Context.Guild.Id))
                                                .Where(x => x.AutoSignUpForFightNight && !string.IsNullOrWhiteSpace(x.PlayerTag))
                                                .Select(x => x.PlayerTag)
                                                .ToArray();

                await _commandHandler?.FillForm(url, pt, Context.Channel, Context.Guild.Id);
                return FromSuccess();
            }
            catch
            {
                // ignore
            }

            return FromError(CommandError.Unsuccessful, "Something went wrong");
        }
    }
}
