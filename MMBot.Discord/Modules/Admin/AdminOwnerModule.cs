using System.IO.Compression;
using Discord.Commands;
using MMBot.Discord.Filters;
using MMBot.Discord.Modules.Interfaces;

namespace MMBot.Discord.Modules.Admin;

public partial class AdminModule : MMBotModule, IAdminModule
{
    private readonly string _backupDir = Path.Combine(".", "backup");
    private readonly string _export = "dataexport.zip";

    [Command("ExportDb")]
    [Summary("Exports db data as json files in zip archive")]
    [RequireOwnerv2]
    public async Task<RuntimeResult> ExportDb()
    {
        var ex = string.Empty;

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

            var m = await Context.Channel.SendFileAsync(ex, "dbExport");

            return FromSuccess(answer: m);
        }
        catch (Exception e)
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
    [RequireOwnerv2]
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

    [Command("ShowServers")]
    [Summary("Shows all servrs the bot is member of")]
    [RequireOwner]
    public async Task<RuntimeResult> ShowServers()
    {
        await ReplyAsync(string.Join(Environment.NewLine, Context.Client.Guilds.Select(x => x.Name)));
        return FromSuccess();
    }
}
