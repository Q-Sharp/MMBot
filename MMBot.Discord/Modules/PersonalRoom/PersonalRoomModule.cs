using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using MMBot.Data.Services.Interfaces;
using MMBot.Discord.Modules;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Modules.PersonalRoom
{
    [Name("PersonalRoom")]
    [Group("PersonalRoom")]
    [Alias("pr")]
    public class PersonalRoomModule : MMBotModule
    {
        public PersonalRoomModule(IDatabaseService databaseService, IGuildSettingsService guildSettings, ICommandHandler commandHandler)
            : base(databaseService, guildSettings, commandHandler)
        {

        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("SetCategoryForMemberRooms")]
        [Summary("Sets category for rooms")]
        public async Task<RuntimeResult> SetCategoryForMemberRooms(ICategoryChannel category)
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id));
            gs.CategoryId = category.Id;
            await _databaseService.SaveDataAsync();

            return FromSuccess("Category set!");
        }

        [RequireUserPermission(ChannelPermission.ManageRoles)]
        [Command("SetMemberRoleForRooms")]
        [Alias("SetRoleForMemberRooms")]
        [Summary("Sets member role for creating rooms")]
        public async Task<RuntimeResult> SetMemberRoleForRooms(IRole role)
        {
            var gs = (await _databaseService.LoadGuildSettingsAsync(Context.Guild.Id));
            gs.MemberRoleId = role.Id;
            await _databaseService.SaveDataAsync();

            return FromSuccess("Member role set");
        }

        [Command("CreateRoom")]
        [Alias("cr")]
        [Summary("Creates personal member room!")]
        public async Task<RuntimeResult> CreateRoom([Remainder] string roomName)
        {
            var rooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);
            var gs = await _guildSettings.GetGuildSettingsAsync(Context.Guild.Id);

            var roles = Context.Guild.Roles.Where(x => x.Members.Select(x => x.Id).Contains(Context.User.Id));
            var isMember = roles.Any(x => x.Id == gs.MemberRoleId);

            if (!rooms.Any(x => x.UserId == Context.User.Id) && isMember)
            {
                RestTextChannel c = null;
                try
                {
                    c = await Context.Guild.CreateTextChannelAsync(roomName, f => f.CategoryId = gs.CategoryId);
                }
                catch (Exception ex)
                {
                    return FromError(CommandError.Unsuccessful, ex.Message);
                }

                if(c is not null)
                {
                    var room = _databaseService.CreatePersonalRoom(Context.Guild.Id);
                    room.Name = roomName;
                    room.RoomId = c.Id;
                    room.UserId = Context.User.Id;
                    await _databaseService.SaveDataAsync();

                    return FromSuccess($"Room: {roomName} was created!");
                }
            }

            return FromError(CommandError.Unsuccessful, "Every !member! can ONLY create ONE room....");
        }

        [Command("DeleteRoom")]
        [Alias("dr", "rr")]
        [Summary("Deletes personal member room!")]
        public async Task<RuntimeResult> DeleteRoom(ITextChannel room)
        {
            var dbRooms = await _databaseService.LoadPersonalRooms(Context.Guild.Id);

            var dbFound = dbRooms.FirstOrDefault(r => r.RoomId == room.Id);

            var roles = Context.Guild.Roles.Where(x => x.Members.Select(x => x.Id).Contains(Context.User.Id));
            var isAdmin = roles.Any(x => x.Permissions.Administrator) || Context.User.Id == Context.Guild.OwnerId;

            if (dbFound is not null && (Context.User.Id == dbFound.UserId || isAdmin))
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
    }
}
