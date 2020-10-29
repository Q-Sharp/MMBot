using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Helpers;
using MMBot.Modules.Interfaces;
using MMBot.Services.Interfaces;
using Discord.WebSocket;

namespace MMBot.Modules.Timer
{
    [Name("Timer")]
    [Group("Timer")]
    [Alias("T")]
    public partial class TimerModule : MMBotModule, ITimerModule
    {
        //protected readonly ITimerService _timerService;

        public TimerModule(IDatabaseService databaseService, ICommandHandler commandHandler, /*ITimerService timerService,*/ IGuildSettingsService guildSettings)
            : base(databaseService, guildSettings, commandHandler)
        {
            //_timerService = timerService;
        }

        [Command("Create")]
        [Alias("C")]
        [Summary("Creates a new timer with a [name] and optinal with recurring = true or false")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task CreateTimer(string name, bool recurring = true)
        {
            try
            {
                var t = await _databaseService.CreateTimerAsync();
                t.Name = name;
                t.IsRecurring = recurring;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The member {t} was added to database.");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("Delete")]
        [Alias("d")]
        [Summary("Delete a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task DeleteTimer(string name)
        {
            try
            {
                var t = (await _databaseService.LoadTimerAsync()).FirstOrDefault(t => t.Name.ToLower().CompareTo(name) == 0 && _guildSettings.GuildId == t.GuildId);
                _databaseService.DeleteTimer(t);
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The timer {t} was deleted");
            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }

        [Command("List")]
        [Alias("l")]
        [Summary("Adds a Notification channel and a message to a existing timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ListTimers()
        {
            var timer = (await _databaseService.LoadTimerAsync()).Where(t => _guildSettings.GuildId == t.GuildId).ToList();
            await ReplyAsync(timer.GetTablePropertiesWithValues());
        }

        [Command("AddNotification")]
        [Alias("AN")]
        [Summary("Adds a Notification channel and a message to a existing timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task AddNotification(string name, ISocketMessageChannel channel, [Remainder] string message)
        {
            throw new NotImplementedException();
        }

        [Command("RemoveNotification")]
        [Alias("RN")]
        [Summary("Removes all Notification channel the message from a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task RemoveNotification(string name)
        {
            throw new NotImplementedException();
        }

        [Command("StartTimer")]
        [Alias("Start")]
        [Summary("Starts a timer, which ring once after timerspan or every timerspan")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task StartTimer(string name, [Remainder] string timeSpan)
        {
            throw new NotImplementedException();
        }

        [Command("StopTimer")]
        [Alias("AN")]
        [Summary("Stops a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task StopTimer(string name)
        {
            throw new NotImplementedException();
        }
    }
}
