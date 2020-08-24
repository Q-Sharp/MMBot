using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Data;
using TTMMBot.Data.Enums;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    [Name("Admin")]
    [Group("Admin")]
    [Alias("admin", "a", "A")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private static volatile bool _reorderIsRunning;

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

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("FixRoles", RunMode = RunMode.Async)]
        [Alias("FR")]
        [Summary("Checks and fixes discord roles of all clan members.")]
        public async Task FixRoles() =>
            await Task.Run(async () =>
            {
                try
                {
                    if (_reorderIsRunning)
                    {
                        await ReplyAsync($"I can do this only once at a time!");
                        return;
                    }

                    _reorderIsRunning = true;
                    await ReplyAsync($"Fixing roles of discord members accordingly to their clan membership....");

                    var c = await DatabaseService.LoadClansAsync();
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
                            var user = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => $"{x.Username}#{x.Discriminator}" == member.Discord));

                            if (user is null || member.ClanID is null || clan.DiscordRole is null)
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
                    _reorderIsRunning = false;
                }
                catch (Exception e)
                {
                    await ReplyAsync($"{e.Message}");
                }
            });

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
