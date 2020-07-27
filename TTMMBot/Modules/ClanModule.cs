using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.Services;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules
{
    [Name("Clan")]
    [Group("Clan")]
    [Alias("clan", "c", "C")]
    [RequireUserPermission(ChannelPermission.ManageRoles)]
    public class ClanModule : ModuleBase<SocketCommandContext>
    {
        public IDatabaseService DatabaseService { get; set; }

        [Command]
        public async Task Clan()
        {
            try
            {
                var clans = await DatabaseService.ClanService.LoadClansAsync();

                var builder = new EmbedBuilder
                {
                    Color = Color.DarkTeal,
                    Description = "Clans",
                    Title = "Clans"
                };

                foreach (var clan in clans)
                {
                        builder.AddField(x =>
                        {
                            x.Name = $"{clan.ClanID} - {clan}";
                            x.IsInline = true;
                        });
                }

                await ReplyAsync("", false, builder.Build());
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Create")]
        [Alias("create", "c", "C")]
        public async Task Create(Clan clan)
        {
            try
            {
                await DatabaseService.ClanService.CreateClanAsync(clan);
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The clan [{clan.Tag}] {clan.Name} was added to database.");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Delete")]
        [Alias("delete", "d", "D")]
        public async Task Delete(Clan clan)
        {
            try
            {
                await DatabaseService.ClanService.DeleteClanAsync(clan);
                await DatabaseService.SaveDataAsync();
                await ReplyAsync($"The clan [{clan.Tag}] {clan.Name} was added to database.");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Add")]
        [Alias("add", "a", "A")]
        public async Task ÁddMember(Member member, Clan clan)
        {
            try
            {
                //await DatabaseService.ClanService..(clan, member);
                //await DatabaseService.SaveDataAsync();
                //await ReplyAsync($"The member {member} was moved to {clan}.");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Group("Set")]
        
        [Alias("set", "s", "S")]
        public class Set : ModuleBase<SocketCommandContext>
        {
           



            [Command("Tag")]
            [Alias("tag", "t", "T")]
            public async Task Tag(Clan clan)
            {
                
            }

            [Command("Name")]
            [Alias("name", "n", "N")]
            public async Task Name(Clan clan)
            {

            }
        }
    }
}
