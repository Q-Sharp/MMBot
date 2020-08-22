using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.Commands;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    [Name("Admin")]
    [Group("Admin")]
    [Alias("admin", "a", "A")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        public IDatabaseService DatabaseService { get; set; }

        public NotionCSVImportService CsvImportService { get; set; }

        public AdminService AdminService { get; set; }

        [RequireOwner]
        [Command("ImportCSV")]
        [Alias("import")]
        [Summary("Imports a notion csv export to update db")]
        public async Task ImportCsv()
        {
            try
            {
                var csvFile = Context.Message.Attachments.FirstOrDefault();
                var myWebClient = new WebClient();
                var csv = myWebClient.DownloadData(csvFile.Url);
                var result = await CsvImportService?.ImportCSV(csv);

                await ReplyAsync(result == null ? "CSV file import was successful" : $"ERROR: {result.Message}");
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
