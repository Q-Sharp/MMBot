using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using MMBot.Data.Contracts;
using MMBot.Data.Contracts.Helpers;
using MMBot.Data.Helpers;
using MMBot.Discord.Filters;
using MMBot.Discord.Helpers;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Modules.PersonalRoom;

[Name("PersonalRoom")]
[Group("PersonalRoom")]
[Alias("pr")]
public class PersonalRoomModule : MMBotModule
{
    public PersonalRoomModule(IDatabaseService databaseService, IGuildSettingsService guildSettings, ICommandHandler commandHandler)
        : base(databaseService, guildSettings, commandHandler)
    {

    }

    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    [Command("SetCategoryForMemberRooms")]
    [Summary("Sets category for rooms")]
    public async Task<RuntimeResult> SetCategoryForMemberRooms(ICategoryChannel category)
    {
        var gs = await GetGuildSettings();
        gs.CategoryId = category.Id;
        await _databaseService.SaveDataAsync();

        return FromSuccess("Category set!");
    }

    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    [Command("SetMemberRoleForRooms")]
    [Alias("SetRoleForMemberRooms")]
    [Summary("Sets member role for creating rooms")]
    public async Task<RuntimeResult> SetMemberRoleForRooms(IRole role)
    {
        var gs = await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id);
        gs.MemberRoleId = role.Id;
        await _databaseService.SaveDataAsync();

        return FromSuccess("Member role set");
    }

    [RequireUserPermissionOrBotOwner(ChannelPermission.ManageRoles)]
    [Command("CleanUpMemberRooms")]
    [Alias("cumr")]
    [Summary("Sets member role for creating rooms")]
    public async Task<RuntimeResult> CleanUpMemberRooms()
    {
        var gs = await _guildSettingsService.GetGuildSettingsAsync(Context.Guild.Id);
        var dbRooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);

        var rooms = Context.Guild.GetCategoryChannel(gs.CategoryId)?.Channels;

        var cleanedUp = string.Empty;
        dbRooms.Where(x => !rooms.Select(x => x.Id).Contains(x.RoomId)).ForEach(x =>
        {
            _databaseService.DeletePersonalRoom(x);
            cleanedUp += $"{x.Name}, ";
        });

        cleanedUp = cleanedUp.TrimEnd(new char[] { ',', ' ' });

        return FromSuccess($"These rooms were removed from db: {cleanedUp}");
    }

    [Command("CreateRoom")]
    [Alias("cr")]
    [Summary("Creates personal member room!")]
    public async Task<RuntimeResult> CreateRoom([Remainder] string roomName)
    {
        var gs = await _guildSettingsService.GetGuildSettingsAsync(Context.Guild.Id);

        if (gs.CategoryId == 0 || gs.MemberRoleId == 0)
            return FromError(CommandError.Unsuccessful, "Member rooms feature has to be configured. Set Category and MemberRole!");

        var rooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);
        var roles = Context.Guild.Roles.Where(x => x.Members.Select(x => x.Id).Contains(Context.User.Id));
        var isMember = roles.Any(x => x.Id == gs.MemberRoleId) || Context.User.IsOwner();

        if (!rooms.Any(x => x.UserId == Context.User.Id) && isMember)
        {
            RestTextChannel c = null;
            try
            {
                c = await Context.Guild.CreateTextChannelAsync(roomName, f => f.CategoryId = gs.CategoryId);

                if (c != null && Context.User.IsOwner())
                {
                    await c.AddPermissionOverwriteAsync(Context.User,
                        new OverwritePermissions(addReactions: PermValue.Allow, sendMessages: PermValue.Allow, readMessageHistory: PermValue.Allow, viewChannel: PermValue.Allow));
                }
            }
            catch (Exception ex)
            {
                return FromError(CommandError.Unsuccessful, ex.Message);
            }

            if (c is not null)
            {
                var room = _databaseService.CreatePersonalRoom(Context.Guild.Id);
                room.Name = c.Name;
                room.RoomId = c.Id;
                room.UserId = Context.User.Id;
                await _databaseService.SaveDataAsync();

                return FromSuccess(Context.User.IsOwner() ? $"My father's room: {c.Name} was created!" : $"Room: {c.Name} was created!");
            }
        }

        return FromError(CommandError.Unsuccessful, "Every !member! can ONLY create ONE room....");
    }

    [Command("DeleteRoom")]
    [Alias("dr")]
    [Summary("Deletes personal member room!")]
    public async Task<RuntimeResult> DeleteRoom(ITextChannel room)
    {
        var dbRooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);
        var dbFound = dbRooms.FirstOrDefault(r => r.RoomId == room.Id);

        if (dbFound is not null && (Context.User.Id == dbFound.UserId || Context.IsAdmin()))
        {
            _databaseService.DeletePersonalRoom(dbFound);
            await _databaseService.SaveDataAsync();

            var found = Context.Guild.TextChannels.FirstOrDefault(x => x.Id == room.Id);
            try
            {
                await found?.DeleteAsync();
            }
            catch (Exception ex)
            {
                return FromError(CommandError.Unsuccessful, ex.Message);
            }

            return FromSuccess("Room deleted!");
        }

        return FromError(CommandError.Unsuccessful, "Room couldn't be deleted!");
    }

    [Command("RenameRoom")]
    [Alias("rr")]
    [Summary("Rename personal member room!")]
    public async Task<RuntimeResult> RenameRoom(ITextChannel room, [Remainder] string newName)
    {
        var dbRooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);

        var dbFound = dbRooms.FirstOrDefault(r => r.RoomId == room.Id);

        if (dbFound is not null && (Context.User.Id == dbFound.UserId || Context.IsAdmin()))
        {
            _databaseService.RenamePersonalRoom(dbFound, newName);
            await _databaseService.SaveDataAsync();

            var found = Context.Guild.TextChannels.FirstOrDefault(x => x.Id == room.Id);
            try
            {
                await found?.ModifyAsync(f => f.Name = newName);
            }
            catch (Exception ex)
            {
                return FromError(CommandError.Unsuccessful, ex.Message);
            }

            return FromSuccess("Room renamed!");
        }

        return FromError(CommandError.Unsuccessful, "Room couldn't be renamed!");
    }

    [Command("PinLastMessage")]
    [Alias("plm", "pin")]
    [Summary("Pin last message in personal member room!")]
    public async Task<RuntimeResult> PinLastMessage()
    {
        var dbRooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);

        var foundRoom = dbRooms.FirstOrDefault(r => r.RoomId == Context.Channel.Id);

        if (foundRoom is not null && (Context.User.Id == foundRoom.UserId || Context.IsAdmin()))
        {
            try
            {
                var lm = Context.Channel.CachedMessages.FirstOrDefault(m => m.Author.Id == foundRoom.UserId);
                await (lm as SocketUserMessage)?.PinAsync();
            }
            catch (Exception ex)
            {
                return FromError(CommandError.Unsuccessful, ex.Message);
            }

            return FromSuccess("Last message pinned!");
        }

        return FromError(CommandError.Unsuccessful, "Couldn't pin your last message");
    }

    [Command("UnPinLastPin")]
    [Alias("uplm", "unpin")]
    [Summary("Unpin last pinned message in personal member room!")]
    public async Task<RuntimeResult> UnpinLastPin()
    {
        var dbRooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);

        var dbFound = dbRooms.FirstOrDefault(r => r.RoomId == Context.Channel.Id);

        if (dbFound is not null && (Context.User.Id == dbFound.UserId || Context.IsAdmin()))
        {
            try
            {
                var lm = Context.Channel.CachedMessages.FirstOrDefault(m => m.Author.Id == dbFound.UserId);
                await (lm as SocketUserMessage)?.PinAsync();
            }
            catch (Exception ex)
            {
                return FromError(CommandError.Unsuccessful, ex.Message);
            }

            return FromSuccess("Last message pinned!");
        }

        return FromError(CommandError.Unsuccessful, "Couldn't pin your last message");
    }
}
