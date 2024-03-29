﻿using Discord;
using Discord.Commands;
using MMBot.Data.Contracts.Entities;
using MMBot.Data.Contracts.Helpers;
using MMBot.Data.Helpers;
using MMBot.Discord.Filters;
using MMBot.Discord.Helpers;
using MMBot.Discord.Modules.Interfaces;

namespace MMBot.Discord.Modules.Member;

/// <summary>
/// Methods for Managers
/// </summary>
public partial class MemberModule : MMBotModule, IMemberModule
{
    [Command("Delete")]
    [Alias("D")]
    [Summary("Deletes member with given name.")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Delete(string name)
    {
        var m = await (await _databaseService.LoadMembersAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, name, Context.Channel, _commandHandler);
        _databaseService.DeleteMember(m);
        await _databaseService.SaveDataAsync();
        return FromSuccess($"The member {m} was deleted");
    }

    [Command("Set")]
    [Summary("Set [Username] [Property name] [Value]")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Set(string name, string propertyName, [Remainder] string value)
    {
        var m = await (await _databaseService.LoadMembersAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, name, Context.Channel, _commandHandler);
        var r = m.ChangeProperty(propertyName, value);
        await _databaseService.SaveDataAsync();
        return FromSuccess(r);
    }

    [Command("Get")]
    [Summary("Get [Username] [Property name]")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Get(string name, string propertyName)
    {
        var m = await (await _databaseService.LoadMembersAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, name, Context.Channel, _commandHandler);
        var r = m.GetProperty(propertyName);
        return FromSuccess(r);
    }

    [Command("Create")]
    [Summary("Creates a new member.")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Create([Remainder] string name)
    {
        var m = _databaseService.CreateMember(Context.Guild.Id);
        m.Name = name;
        m.IsActive = true;
        await _databaseService.SaveDataAsync();
        return FromSuccess($"The member {m} was added to database.");
    }

    [Command("ShowAll")]
    [Summary("Show all member where propertyName has value")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> ShowAll(string propertyName, [Remainder] string value)
    {
        var m = await _databaseService.LoadMembersAsync(Context.Guild.Id);

        var fm = m.FilterCollectionByPropertyWithValue(propertyName, value).Select(x => x.Name).ToList();
        return FromSuccess($"These members fulfill the given condition ({propertyName} == {value}): {string.Join(", ", fm)}");
    }

    [Command("AddStrike")]
    [Summary("Add a strike to a member")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> AddStrike(string name, [Remainder] string strikeReason)
    {
        var m = await (await _databaseService.LoadMembersAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, name, Context.Channel, _commandHandler);

        m.Strike.Add(new Strike { Reason = strikeReason, MemberId = m.Id, Member = m, StrikeDate = DateTime.UtcNow });
        await _databaseService?.SaveDataAsync();

        return FromSuccess($"{m} now has {m?.Strike?.Count ?? 0} strike(s)!");
    }

    [Command("RemoveStrike")]
    [Summary("Removes a strike from a member.")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> RemoveStrike([Remainder] string name)
    {
        var m = await (await _databaseService.LoadMembersAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, name, Context.Channel, _commandHandler);

         m.Strike.Remove(m.Strike.OrderBy(y => y.StrikeDate).FirstOrDefault());
        await _databaseService?.SaveDataAsync();

        return FromSuccess($"{m} now has {m?.Strike?.Count ?? 0} strike(s)!");
    }

    [Command("ShowAllStrikes")]
    [Summary("Show all member with strikes")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> ShowAllStrikes()
    {
        var me = (await _databaseService.LoadMembersAsync(Context.Guild.Id)).Where(x => x.Strike?.Count > 0)?.ToList();

        return (me?.Count ?? 0) <= 0
            ? FromErrorUnsuccessful("No member with strikes found!")
            : (RuntimeResult)FromSuccess(me.GetTablePropertiesWithValues());
    }

    [Command("GroupMember")]
    [Summary("Groups all following members")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> GroupMember(params string[] memberNames)
    {
        var m = await _databaseService.LoadMembersAsync(Context.Guild.Id);
        var grouped = await Task.WhenAll(memberNames.Select(x => m.FindAndAskForEntity(Context.Guild.Id, x, Context.Channel, _commandHandler)));

        var group = new MemberGroup();

        grouped.ForEach(x => x.MemberGroup = group);
        await _databaseService?.SaveDataAsync();

        return FromSuccess($"{string.Join(", ", memberNames)} are now in a MemberGroup!");
    }

    [Command("UnGroupMember")]
    [Summary("Ungroups all following members")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> UnGroupMember(params string[] memberNames)
    {
        var m = await _databaseService.LoadMembersAsync(Context.Guild.Id);
        var grouped = await Task.WhenAll(memberNames.Select(x => m.FindAndAskForEntity(Context.Guild.Id, x, Context.Channel, _commandHandler)));
        grouped.ForEach(x => x.MemberGroupId = null);
        await _databaseService?.SaveDataAsync();

        return FromSuccess($"{string.Join(", ", memberNames)} aren't in a MemberGroup any longer!");
    }

    [Command("Profile")]
    [Alias("p")]
    [Summary("Shows all information of a member.")]
    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    public async Task<RuntimeResult> Profile([Remainder] string name)
    {
        var m = await (await _databaseService.LoadMembersAsync(Context.Guild.Id)).FindAndAskForEntity(Context.Guild.Id, name, Context.Channel, _commandHandler);

        if (m is null)
            return FromErrorObjectNotFound("Member", name);

        var imageUrl = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => x.GetUserAndDiscriminator() == m.Discord)?.GetAvatarUrl());
        var e = m.GetEmbedPropertiesWithValues(imageUrl);
         await ReplyAsync("", false, e as Embed);
        return FromSuccess();
    }
}
