using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    [Name("Admin")]
    [Group("Admin")]
    [Alias("admin", "a", "A")]
    [RequireUserPermission(ChannelPermission.ManageRoles)]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        public IDatabaseService DatabaseService { get; set; }

        public NotionCSVImportService CSVImportService { get; set; }

        [Command("ImportCSV")]
        [Alias("import")]
        [Summary("Imports a notion csv export to update db")]
        public async Task ImportCSV()
        {
            try
            {
                var csvFile = Context.Message.Attachments.FirstOrDefault();
                var myWebClient = new WebClient();
                var csv = myWebClient.DownloadData(csvFile.Url);
                var result = await CSVImportService?.ImportCSV(csv);

                await ReplyAsync(result == null ? "CSV file import was successfull" : $"ERROR: {result.Message}");
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
        public async Task Restart()
        {
            Process.Start(AppDomain.CurrentDomain.FriendlyName);
            await ReplyAsync($"Bot service restarted!");
            
            Environment.Exit(0);
        }

        [RequireOwner]
        [Command("DeleteDB")]
        [Alias("deletedb")]
        [Summary("Deletes sqlite db file")]
        public async Task DeleteDB() => await Task.Run(async () =>
        {
            var db = $"{Path.Combine(Directory.GetCurrentDirectory(), "TTMMBot.db")}";
            File.Delete(db);
            await ReplyAsync($"{db} has been deleted.");
            await Restart();
        });
    }
}
