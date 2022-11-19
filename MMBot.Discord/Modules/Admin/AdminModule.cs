using Discord;
using Discord.Commands;
using MMBot.Data.Contracts;
using MMBot.Discord.Filters;
using MMBot.Discord.Helpers;
using MMBot.Discord.Modules.Interfaces;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Modules.Admin;

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
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> ImportCsv()
    {
        var csvFile = Context.Message.Attachments.FirstOrDefault();
        var myWebClient = _clientFactory.CreateClient();
        string fileName = null;
        IMessage answer = null;

        if (csvFile is not null && _csvService is not null)
        {
            try
            {
                var csv = await myWebClient.GetAsync(csvFile.Url);
                var csvByte = await csv.Content.ReadAsByteArrayAsync();
                var result = await _csvService.ImportCsv(csvByte, Context.Guild.Id);
                fileName = $"{Context.Guild.Id}lastImport.csv";

                if (result is null)
                    File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, fileName), csvByte);

                answer = await ReplyAsync(result is null
                    ? "CSV file import was successful"
                    : $"ERROR: {result.Message}");
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        await _databaseService.CleanDB();
        return FromSuccess(answer: answer);
    }

    [Command("ExportCSV")]
    [Alias("export")]
    [Summary("Exports a csv file from db")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> ExportCsv()
    {
        var settings = await _guildSettingsService.GetGuildSettingsAsync(Context.Guild.Id);

        var result = await _csvService?.ExportCsv(Context.Guild.Id);
        await File.WriteAllBytesAsync(settings.FileName, result);

        var answer = await Context.Channel.SendFileAsync(settings.FileName, "Csv db export");
        File.Delete(settings.FileName);
        return FromSuccess(answer: answer);
    }

    [Command("Reorder")]
    [Alias("reorder")]
    [Summary("Reorders member in db")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> ReorderJoin()
    {
        await Task.Run(() => _adminService.Reorder(Context.Guild.Id));
        var answer = await ReplyAsync("Members join order updated!");
        return FromSuccess(answer: answer);
    }

    [Command("FixRoles")]
    [Alias("FR")]
    [Summary("Checks and fixes discord roles of all clan members.")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> FixRoles()
    {
        IMessage answer = null;

        try
        {
            if (Interlocked.Read(ref _commandIsRunning) > 0)
                return FromError(CommandError.Unsuccessful, $"I can only run a single long running command at a time!");

            _ = Interlocked.Increment(ref _commandIsRunning);
            _ = await ReplyAsync($"Fixing roles of discord members accordingly to their clan membership....");

            var c = await _databaseService.LoadClansAsync(Context.Guild.Id);
            var ar = Context.Guild.Roles.Where(x => c.Select(clan => clan.DiscordRole).Contains(x.Name)).ToArray();

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
                        Context.Guild.Users.FirstOrDefault(x => $"{x.Username}#{x.Discriminator}" == member.Discord));

                    if (user is null || member.ClanId is null || clan.DiscordRole is null)
                        continue;

                    try
                    {
                        await user.RemoveRolesAsync(ar);
                        await user.AddRoleAsync(clanRole);
                    }
                    catch (Exception e)
                    {
                        _ = await ReplyAsync($"{member.Name}'s role couldn't be fixed: {e.Message}");
                    }
                }
            }

            //var members = await _databaseService.LoadMembersAsync();

            //foreach(var m in members)
            //{

            //}

            _ = await ReplyAsync($"All roles have been fixed!");
        }
        catch
        {
            throw;
        }
        finally
        {
            _ = Interlocked.Decrement(ref _commandIsRunning);
        }

        return FromSuccess(answer: answer);
    }

    [Command("Show")]
    [Summary("Show guild settings")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Show()
    {
        var gs = await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id);
        var e = gs.GetEmbedPropertiesWithValues();
        var answer = await ReplyAsync("", false, e as Embed);
        return FromSuccess(answer: answer);
    }

    [Command("Set")]
    [Summary("Set guild settings")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Set(string propertyName, [Remainder] string value)
    {
        var gs = await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id);
        var r = gs.ChangeProperty(propertyName, value);
        await _databaseService.SaveDataAsync();
        return FromSuccess(r);
    }

    [Command("Get")]
    [Summary("Get guild settings")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Get(string propertyName)
    {
        var gs = await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id);
        var r = gs.GetProperty(propertyName);
        var answer = await ReplyAsync(r);
        return FromSuccess(answer: answer);
    }
}
