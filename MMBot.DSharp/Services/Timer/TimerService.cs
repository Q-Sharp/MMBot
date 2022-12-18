//namespace MMBot.DSharp.Services.Timer;

//public class TimerService : MMBotService<TimerService>, ITimerService
//{
//    private readonly IDatabaseService _databaseService;
//    private readonly DiscordSocketClient _dsc;
//    private readonly IList<TimerContainer> _timerList = new List<TimerContainer>();

//    public TimerService(IDatabaseService databaseService, DiscordSocketClient dsc, ILogger<TimerService> logger) : base(logger)
//    {
//        _databaseService = databaseService;
//        _dsc = dsc;
//    }

//    public async Task Start(MMTimer t, bool reInit = false, TimeSpan? firstStart = null)
//    {
//        if (t is null)
//            return;

//        await Task.Run(async () =>
//        {
//            var timeSpan = t.RingSpan.Value;
//            if (reInit)
//            {
//                firstStart = t.EndTime.Value - DateTime.UtcNow;
//                while (firstStart <= TimeSpan.Zero)
//                {
//                    t.StartTime = DateTime.UtcNow;
//                    t.EndTime += t.RingSpan;
//                    firstStart = t.EndTime - t.StartTime;
//                }
//                await _databaseService.SaveDataAsync();
//            }

//            var container = new TimerContainer { TimerInfos = t, IsTemp = reInit };
//            var systemTimer = new System.Threading.Timer(TimerCallback, container, firstStart ?? timeSpan, timeSpan);
//            container.Timer = systemTimer;

//            _timerList.Add(container);
//        });
//    }

//    public async void TimerCallback(object state)
//    {
//        if (state is TimerContainer tc && tc.TimerInfos.ChannelId.HasValue)
//        {
//            var g = _dsc?.GetGuild(tc.TimerInfos.GuildId);
//            var tch = g?.GetTextChannel(tc.TimerInfos.ChannelId.Value);

//            await tch?.SendMessageAsync(tc?.TimerInfos?.Message);

//            if (tc?.TimerInfos?.IsRecurring ?? false)
//            {
//                tc.TimerInfos.StartTime = DateTime.UtcNow;
//                tc.TimerInfos.EndTime = DateTime.UtcNow + tc.TimerInfos.RingSpan;
//                await _databaseService?.SaveDataAsync();
//            }
//            else
//                await Stop(tc?.TimerInfos);
//        }
//    }

//    public async Task Stop(MMTimer t)
//    {
//        if (t is null)
//            return;

//        await Task.Run(async () =>
//        {
//            var container = _timerList.FirstOrDefault(x => x.TimerInfos.Id == t.Id);
//            if (container is not null)
//            {
//                await container.Timer.DisposeAsync().AsTask();
//                _timerList.Remove(container);
//            }
//        });
//    }

//    public bool Check(MMTimer t) => DateTime.UtcNow < t.EndTime || t.IsRecurring;

//    public async Task<string> GetCountDown(MMTimer t)
//        => await Task.Run(() => (_timerList?.FirstOrDefault(tl => tl?.TimerInfos?.Id == t?.Id)?.TimerInfos?.EndTime.Value - DateTime.UtcNow)?.ToString(@"hh\:mm\:ss"));
//}
