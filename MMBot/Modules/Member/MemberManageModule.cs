using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using MMBot.Data.Entities;
using MMBot.Helpers;
using MMBot.Modules.Interfaces;

namespace MMBot.Modules.Member
{
    /// <summary>
    /// Methods for Managers
    /// </summary>
    public partial class MemberModule : MMBotModule, IMemberModule
    {
        [Command("Delete")]
        [Alias("D")]
        [Summary("Deletes member with given name.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Delete(string name)
        {
            var m = await (await _databaseService.LoadMembersAsync()).FindAndAskForMember(Context.Guild.Id, name, Context.Channel, _commandHandler);
            _databaseService.DeleteMember(m, Context.Guild.Id);
            await _databaseService.SaveDataAsync();
            return FromSuccess($"The member {m} was deleted");
        }
        
        [Command("Set")]
        [Summary("Set [Username] [Property name] [Value]")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Set(string name, string propertyName, [Remainder] string value)
        {
            var m = await (await _databaseService.LoadMembersAsync()).FindAndAskForMember(Context.Guild.Id, name, Context.Channel, _commandHandler);
            var r = m.ChangeProperty(propertyName, value);
            await _databaseService.SaveDataAsync();
            return FromSuccess(r);
        }

        [Command("Get")]
        [Summary("Get [Username] [Property name]")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Get(string name, string propertyName)
        {
            var m = await (await _databaseService.LoadMembersAsync()).FindAndAskForMember(Context.Guild.Id, name, Context.Channel, _commandHandler);
            var r = m.GetProperty(propertyName);
            return FromSuccess(r);
        }

        [Command("Create")]
        [Summary("Creates a new member.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> Create(string name)
        {
            var m = await _databaseService.CreateMemberAsync(Context.Guild.Id);
            m.Name = name;
            await _databaseService.SaveDataAsync();
            return FromSuccess($"The member {m} was added to database.");
        }

        [Command("ShowAll")]
        [Summary("Show all member where propertyName has value")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> ShowAll(string propertyName, [Remainder] string value)
        {
            var m = await _databaseService.LoadMembersAsync();

            var fm = m.FilterCollectionByPropertyWithValue(propertyName, value).Select(x => x.Name).ToList();
            return FromSuccess($"These members fulfill the given condition ({propertyName} == {value}): {string.Join(", ", fm)}");
        }

        [Command("AddStrike")]
        [Summary("Add a strike to a member")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> AddStrike(string name, [Remainder] string strikeReason)
        {
            var m = await (await _databaseService.LoadMembersAsync()).FindAndAskForMember(Context.Guild.Id, name, Context.Channel, _commandHandler);
            
            m.Strikes.Add(new Strike { Reason = strikeReason, MemberId = m.Id, Member = m, StrikeDate = DateTime.UtcNow });
            await _databaseService?.SaveDataAsync();

            return FromSuccess($"{m} now has {m?.Strikes?.Count() ?? 0} strike(s)!");
        }

        [Command("RemoveStrike")]
        [Summary("Removes a strike from a member.")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> RemoveStrike(string name)
        {
            var m = await (await _databaseService.LoadMembersAsync()).FindAndAskForMember(Context.Guild.Id, name, Context.Channel, _commandHandler);
            
            m.Strikes.Remove(m.Strikes.OrderBy(y => y.StrikeDate).FirstOrDefault());
            await _databaseService?.SaveDataAsync();

            return FromSuccess($"{m} now has {m?.Strikes?.Count() ?? 0} strike(s)!");
        }

        [Command("ShowAllStrikes")]
        [Summary("Show all member with strikes")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> ShowAllStrikes()
        {
            var me = (await _databaseService.LoadMembersAsync(Context.Guild.Id)).Where(x => x.Strikes?.Count > 0)?.ToList();

            if((me?.Count ?? 0) <= 0)
                return FromErroUnsuccessful("No member with strikes found!");

            return FromSuccess(me.GetTablePropertiesWithValues());
        }
    }
}
