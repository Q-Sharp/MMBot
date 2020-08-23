using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Data;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    [Name("Admin")]
    [Group("Admin")]
    [Alias("admin", "a", "A")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        public IDatabaseService DatabaseService { get; set; }
        public NotionCSVService CsvService { get; set; }
        public AdminService AdminService { get; set; }
        public GlobalSettings GlobalSettings { get; set; }

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
                var result = await CsvService?.ImportCSV(csv);

                await ReplyAsync(result == null ? "CSV file import was successful" : $"ERROR: {result.Message}");
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
                var result = await CsvService?.ExportCSV();
                await File.WriteAllBytesAsync(GlobalSettings.FileName, result);

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
