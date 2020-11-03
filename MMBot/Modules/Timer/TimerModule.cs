using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Helpers;
using MMBot.Modules.Interfaces;
using MMBot.Services.Interfaces;
using Discord.WebSocket;
using MMBot.Data.Entities;
using System.Collections.Generic;
using Nito.AsyncEx;
using Microsoft.Extensions.Logging;

namespace MMBot.Modules.Timer
{
    [Name("Timer")]
    [Group("Timer")]
    [Alias("T")]
    public partial class TimerModule : MMBotModule, ITimerModule
    {
        protected readonly ITimerService _timerService;
        private IList<MMTimer> CountDownList = new List<MMTimer>();
        private readonly AsyncLock _mutex = new AsyncLock();

        public TimerModule(IDatabaseService databaseService, ICommandHandler commandHandler, ITimerService timerService, IGuildSettingsService guildSettings)
            : base(databaseService, guildSettings, commandHandler)
        {
            _timerService = timerService;
        }

        [Command("Create")]
        [Alias("C")]
        [Summary("Creates a new timer with a [name] and optinal with recurring = true or false")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task CreateTimer(string name, bool recurring = true)
        {
            if((await _databaseService.LoadTimerAsync()).FirstOrDefault(t => t.Name.ToLower() == name.ToLower() && Context.Guild.Id == t.GuildId) != null)
            {
                await ReplyAsync($"A timer with that name already exists!");
                return;
            }

            var t = await _databaseService.CreateTimerAsync(Context.Guild.Id);
            t.Name = name;
            t.IsRecurring = recurring;
            t.GuildId = Context.Guild.Id;
            await _databaseService.SaveDataAsync();
            await ReplyAsync($"The timer {t} was added to database.");
        }

        [Command("Delete")]
        [Alias("d")]
        [Summary("Delete a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task DeleteTimer(string name)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);

            if(t !=  null)
            {
               if(t.IsActive)
                   await StopTimer(name);

               _databaseService.DeleteTimer(t, Context.Guild.Id);
               await _databaseService.SaveDataAsync();
               await ReplyAsync($"The timer {name} was deleted");
            }           
        }

        [Command("List")]
        [Alias("l")]
        [Summary("Adds a Notification channel and a message to a existing timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ListTimers()
        {
            var timer = (await _databaseService.LoadTimerAsync(Context.Guild.Id)).ToList();
            await ReplyAsync(timer.Count > 0 ? timer.GetTablePropertiesWithValues() : "No timers");
        }

        [Command("AddNotification")]
        [Alias("AN")]
        [Summary("Adds a Notification channel and a message to a existing timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task AddNotification(string name, ISocketMessageChannel channel, [Remainder] string message)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);
            if(t != null)
            {
                if(t.IsActive)
                    await StopTimer(name);

                t.ChannelId = channel.Id;
                t.GuildId = Context.Guild.Id;
                t.Message = message;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"A notification will be send to {channel.Name} for timer {t}.");
            }
        }

        [Command("RemoveNotification")]
        [Alias("RN")]
        [Summary("Removes a notification channel the message from a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task RemoveNotification(ulong guildId, string name)
        {     
            var t = await _databaseService.GetTimerAsync(name, guildId);
            if(t != null)
            {
                if(t.IsActive)
                    await StopTimer(name);

                t.ChannelId = null;
                t.Message = null;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"The notification for timer {t} is deleted.");
            }
        }

        [Command("StartTimer")]
        [Alias("Start")]
        [Summary("Starts a timer, which rings once after timerspan and every timerspan")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task StartTimer(string name, string timeToFirstRing, string timeInterval = null, double? timeOffSet = null)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);
            if(t != null)
            {
                if(t.Message == null || t.ChannelId == null)
                {
                    await ReplyAsync($"You can't run a timer without a message and a textchannel.");
                    return;
                }

                if(t.IsActive)
                {
                    await ReplyAsync($"This timer is already running.... idiot!");
                    return;
                }

                //var ulto = timeOffSet ?? (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => x.Discord == Context.Message.Author.GetUserAndDiscriminator())?.LocalTimeOffSet;
                //if(ulto is null)
                //{
                //    await ReplyAsync($"Please specify your local time offset compared to utc. You can do this either by changing the LocalTimeOffSet property in your profile or by calling this command with an additional offset value");
                //    return;
                //}

                t.IsActive = true;
                t.StartTime = DateTime.UtcNow;
                t.RingSpan = TimeSpan.Parse(timeInterval ?? timeToFirstRing);
                var toFirstRingSpan = TimeSpan.Parse(timeToFirstRing);

                t.EndTime = t.StartTime + toFirstRingSpan;
                await _databaseService.SaveDataAsync();

                await _timerService?.Start(t, false, toFirstRingSpan);
                await ReplyAsync($"Timer {t} is running.");
            }
        }

        [Command("StopTimer")]
        [Alias("Stop")]
        [Summary("Stops a timer")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task StopTimer(string name)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);
            if(t != null)
            {
                await _timerService?.Stop(t);
                t.IsActive = false;
                t.StartTime = null;
                t.RingSpan = null;
                t.EndTime = null;
                await _databaseService.SaveDataAsync();
                await ReplyAsync($"Timer {t} stopped.");
            }
        }

        [Command("ShowTimeLeft")]
        [Alias("sc")]
        [Summary("Starts a countdown")]
        [RequireUserPermission(ChannelPermission.ManageRoles)]
        public async Task ShowTimeLeft(string name)
        {
            var t = await _databaseService.GetTimerAsync(name, Context.Guild.Id);
            if(t != null)
            {
                var timeLeft = await _timerService?.GetCountDown(t);
                await ReplyAsync($"Countdown for {t}: {timeLeft}");
            }
        }
    }
}
