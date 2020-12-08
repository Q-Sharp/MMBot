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
        protected readonly ITimerService _timerService;

        public TimerModule(IDatabaseService databaseService, ICommandHandler commandHandler, ITimerService timerService, IGuildSettingsService guildSettings)
            : base(databaseService, guildSettings, commandHandler)
        {
            _timerService = timerService;
        }

        [Command("Create")]
        [Alias("C")]
        [Summary("Creates a new timer with a [name] and optinal with recurring = true or false")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> CreateTimer(string name, bool recurring = true)
        {
            if((await _databaseService.LoadTimerAsync(Context.Guild.Id)).FirstOrDefault(t => t.Name.ToLower() == name.ToLower()) is not null)
            {
                var es = $"A timer with that name already exists!";
                await ReplyAsync(es);
                return FromError(CommandError.Unsuccessful, es);
            }

            var t = await _databaseService.CreateTimerAsync(Context.Guild.Id);
            if(t is not null)
            {
                t.Name = name;
                t.IsRecurring = recurring;
                t.GuildId = Context.Guild.Id;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The timer {t} was added to database.");
            }
            
            return FromSuccess();
        }

        [Command("Delete")]
        [Alias("d")]
        [Summary("Delete a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> DeleteTimer(string name)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);

            if(t !=  null)
            {
               if(t.IsActive)
                   await StopTimer(name);

               _databaseService.DeleteTimer(t);
               await _databaseService.SaveDataAsync();
               await ReplyAsync($"The timer {name} was deleted");
            }
            
            return FromErrorObjectNotFound("Timer", name);
        }

        [Command("List")]
        [Alias("l")]
        [Summary("Adds a Notification channel and a message to a existing timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> ListTimers()
        {
            var timer = (await _databaseService.LoadTimerAsync(Context.Guild.Id)).ToList();
            await ReplyAsync(timer.Count > 0 ? timer.GetTablePropertiesWithValues() : "No timers");
            return FromSuccess();
        }

        [Command("AddNotification")]
        [Alias("AN")]
        [Summary("Adds a Notification channel and a message to a existing timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> AddNotification(string name, ISocketMessageChannel channel, [Remainder] string message)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);
            if(t is not null)
            {
                if(t.IsActive)
                    await StopTimer(name);

                t.ChannelId = channel.Id;
                t.GuildId = Context.Guild.Id;
                t.Message = message;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"A notification will be send to {channel.Name} for timer {t}.");
            }

            return FromErrorObjectNotFound("Timer", name);
        }

        [Command("RemoveNotification")]
        [Alias("RN")]
        [Summary("Removes a notification channel the message from a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> RemoveNotification(ulong guildId, string name)
        {     
            var t = await _databaseService.GetTimerAsync(name, guildId);
            if(t is not null)
            {
                if(t.IsActive)
                    await StopTimer(name);

                t.ChannelId = null;
                t.Message = null;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The notification for timer {t} is deleted.");
                return FromSuccess();
            }

            return FromErrorObjectNotFound("Timer", name);
        }

        [Command("StartTimer")]
        [Alias("Start")]
        [Summary("Starts a timer, which rings once after timerspan and every timerspan")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> StartTimer(string name, string timeToFirstRing, string timeInterval = null)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);
            if(t is not null)
            {
                if(t.Message is null || t.ChannelId is null)
                    return FromError(CommandError.Unsuccessful, $"You can't run a timer without a message and a textchannel.");

                if(t.IsActive)
                    return FromError(CommandError.Unsuccessful, $"This timer is already running...");

                t.IsActive = true;
                t.StartTime = DateTime.UtcNow;
                t.RingSpan = TimeSpan.Parse(timeInterval ?? timeToFirstRing);
                var toFirstRingSpan = TimeSpan.Parse(timeToFirstRing);

                t.EndTime = t.StartTime + toFirstRingSpan;
                await _databaseService.SaveDataAsync();

                await _timerService?.Start(t, false, toFirstRingSpan);
                await ReplyAsync($"Timer {t} is running.");
                return FromSuccess();
            }

            return FromErrorObjectNotFound("Timer", name);
        }

        [Command("StopTimer")]
        [Alias("Stop")]
        [Summary("Stops a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> StopTimer(string name)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);
            if(t is not null)
            {
                await _timerService?.Stop(t);
                t.IsActive = false;
                t.StartTime = null;
                t.RingSpan = null;
                t.EndTime = null;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"Timer {t} stopped.");

                return FromSuccess();
            }

            return FromErrorObjectNotFound("Timer", name);
        }

        [Command("ShowTimeLeft")]
        [Alias("sc")]
        [Summary("Starts a countdown")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task<RuntimeResult> ShowTimeLeft(string name)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);

            if(t is not null)
            {
                var timeLeft = await _timerService?.GetCountDown(t);
                await ReplyAsync($"Countdown for {t}: {timeLeft}");
                return FromSuccess();
            }

            return FromErrorObjectNotFound("Timer", name);
        }
    }
}
