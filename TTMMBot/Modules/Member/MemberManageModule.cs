using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Helpers;
using TTMMBot.Modules.Interfaces;

namespace TTMMBot.Modules.Member
{
    /// <summary>
    /// Methods for Managers
    /// </summary>
    public partial class MemberModule : ModuleBase<SocketCommandContext>, IMemberModule
    {
        [Command("Delete")]
        [Alias("D")]
        [Summary("Deletes member with given name.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Delete(string name)
        {
            try
            {
                var m = (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
                _databaseService.DeleteMember(m);
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The member {m} was deleted");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }
        
        [Command("Set")]
        [Summary("Set [Username] [Property name] [Value]")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Set(string name, string propertyName, [Remainder] string value)
        {
            var m = (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            var r = m.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            await ReplyAsync(r);
        }

        [Command("Get")]
        [Summary("Get [Username] [Property name]")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Get(string name, string propertyName)
        {
            var m = (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
            var r = m.GetProperty(propertyName);
            await ReplyAsync(r);
        }

        [Command("Create")]
        [Summary("Creates a new member.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task Create(string name)
        {
            try
            {
                var m = await _databaseService.CreateMemberAsync();
                m.Name = name;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The member {m} was added to database.");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("ShowAll")]
        [Summary("Show all member where propertyName has value")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ShowAll(string propertyName, [Remainder] string value)
        {
            try
            {
                var m = await _databaseService.LoadMembersAsync();

                var fm = m.FilterCollectionByPropertyWithValue(propertyName, value).Select(x => x.Name).ToList();
                await ReplyAsync($"These members fulfill the given condition ({propertyName} == {value}): {string.Join(", ", fm)}"); 
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }
    }
}
