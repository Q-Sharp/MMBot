using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using MMBot.Data;
using MMBot.Data.Entities;
using MMBot.Services.Interfaces;
using Nito.AsyncEx;

namespace MMBot.Services.Timer
{
    public class TimerService : ITimerService
    {
        private readonly IDatabaseService _databaseService;
        private readonly DiscordSocketClient _dsc;
        private readonly IList<TimerContainer> _timerList = new List<TimerContainer>();

        public TimerService(IDatabaseService databaseService, DiscordSocketClient dsc)
        {
            _databaseService = databaseService;
            _dsc = dsc;
        }

        public async Task Start(MMTimer t, bool reInit = false, TimeSpan? firstStart = null)
        {
            await Task.Run(async () =>
            {
                var timeSpan = t.RingSpan.Value;
                if(reInit)
                {
                    firstStart = t.EndTime.Value - DateTime.UtcNow;
                    while(firstStart <= TimeSpan.Zero)
                    {
                        t.StartTime = DateTime.UtcNow;
                        t.EndTime += t.RingSpan;
                        firstStart = t.EndTime - t.StartTime;
                    }
                    await _databaseService.SaveDataAsync();
                }
                    
                var container = new TimerContainer { TimerInfos = t, IsTemp = reInit };
                var systemTimer = new System.Threading.Timer(TimerCallback, container, firstStart ?? timeSpan, timeSpan);
                container.Timer = systemTimer;
                
                _timerList.Add(container);
            });
        }
        
        public async void TimerCallback(object state)
        {
            if(state is TimerContainer tc)
            {
                var g = _dsc.GetGuild(tc.TimerInfos.GuildId);
                var tch = g?.GetTextChannel(tc.TimerInfos.ChannelId.Value);

                await tch?.SendMessageAsync(tc.TimerInfos.Message);

                if(tc.TimerInfos.IsRecurring)
                {
                    tc.TimerInfos.StartTime = DateTime.UtcNow;
                    tc.TimerInfos.EndTime = DateTime.UtcNow + tc.TimerInfos.RingSpan;
                    await _databaseService.SaveDataAsync();
                }
                else
                    await Stop(tc.TimerInfos);
            } 
        }

        public async Task Stop(MMTimer t)
        {
            await Task.Run(async () =>
            {
                var container = _timerList.FirstOrDefault(x => x.TimerInfos.Id == t.Id);
                if(container != null)
                {
                    await container.Timer.DisposeAsync().AsTask();
                    _timerList.Remove(container);
                }
            });
        }

        public bool Check(MMTimer t) => DateTime.UtcNow < t.EndTime || t.IsRecurring;
    }
}