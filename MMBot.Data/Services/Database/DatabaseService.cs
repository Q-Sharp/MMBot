using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MMBot.Data.Entities;
using MMBot.Data.Enums;
using MMBot.Data.Helpers;
using MMBot.Data.Services.Interfaces;
using static MMBot.Data.Helpers.EFCoreHelpers;

namespace MMBot.Data.Services.Database;

public class DatabaseService : IDatabaseService
{
    private readonly Context _context;
    private readonly ILogger<IDatabaseService> _logger;

    public DatabaseService(Context context, ILogger<IDatabaseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task MigrateAsync() => await _context?.MigrateAsync();
    public async Task SaveDataAsync() => await _context?.SaveChangesAsync(new CancellationToken());
    public async Task<GuildSettings> LoadGuildSettingsAsync(ulong guildId) => await _context.GuildSettings.Where(x => x.GuildId == guildId).FirstOrDefaultAsync();
    public async Task<IList<GuildSettings>> LoadAllGuildSettingsAsync() => await _context.GuildSettings.AsAsyncEnumerable().ToListAsync();
    public void DeleteGuildSettings(GuildSettings gs) => _context.Remove(gs);

    public Clan CreateClan(ulong guildId) => _context.Add(new Clan { GuildId = guildId }).Entity;
    public async Task<IList<Clan>> LoadClansAsync(ulong guildId) => await _context.Clan.Where(x => x.GuildId == guildId).ToListAsync();
    public void DeleteClan(Clan c) => _context.Remove(c);
    public void DeleteClan(int id) => DeleteClan(_context.Clan.FirstOrDefault(c => c.Id == id));

    public Member CreateMember(ulong guildId) => _context.Add(new Member { GuildId = guildId }).Entity;
    public async Task<IList<Member>> LoadMembersAsync(ulong guildId) => await _context.Member.Where(x => x.GuildId == guildId).ToListAsync();
    public void DeleteMember(Member m) => _context.Remove(m);
    public void DeleteMember(int id) => DeleteMember(_context.Member.FirstOrDefault(c => c.Id == id));

    public MMTimer CreateTimer(ulong guildId) => _context.Add(new MMTimer { GuildId = guildId }).Entity;
    public async Task<IList<MMTimer>> LoadTimerAsync(ulong? guildId = null) => await _context.Timer.Where(x => guildId == null || x.GuildId == guildId).ToListAsync();
    public async Task<MMTimer> GetTimerAsync(string name, ulong guildId) => await _context.Timer.Where(x => x.GuildId == guildId).FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
    public void DeleteTimer(MMTimer t) => _context.Remove(t);
    public void DeleteTimer(int id) => DeleteTimer(_context.Timer.FirstOrDefault(c => c.Id == id));

    public async Task<IList<MemberRoom>> LoadPersonalRooms(ulong guildId) => await _context.MemberRoom.Where(x => x.GuildId == guildId).ToListAsync();
    public void DeletePersonalRoom(MemberRoom room) => _context.Remove(room);
    public void RenamePersonalRoom(MemberRoom room, string newName) => room.Name = newName;
    public MemberRoom CreatePersonalRoom(ulong guildId) => _context.Add(new MemberRoom { GuildId = guildId }).Entity;

    public Restart AddRestart() => _context.Add(new Restart()).Entity;
    public async Task<Restart> ConsumeRestart()
    {
        try
        {
            var r = await _context.Restart.AsAsyncEnumerable().OrderBy(r => r.Id).FirstOrDefaultAsync();

            if (r != null)
            {
                _context.Restart.Remove(r);
                await _context.SaveChangesAsync(new CancellationToken());
                return r;
            }
            else
                return default;
        }
        catch
        {
            return default;
        }
    }

    public Channel CreateChannel(ulong guildId) => _context.Add(new Channel()).Entity;
    public async Task<IList<Channel>> LoadChannelsAsync(ulong? guildId = null, bool loadAll = false)
        => await _context.Channel.AsAsyncEnumerable().Where(x => guildId == null || x.GuildId == guildId || loadAll).ToListAsync();

    public void DeleteChannel(Channel c) => _context.Remove(c);
    public void DeleteChannel(int id) => DeleteChannel(_context.Channel.FirstOrDefault(c => c.Id == id));

    public void Truncate()
    {
        var lookup = typeof(DbSet<>);
        var dbSets = typeof(Context).GetProperties()
                        .Where(x => x.PropertyType.IsGenericType && x.PropertyType.Name.Contains("DbSet"))
                        .ToList();

        foreach (var dbSet in dbSets)
            try
            {
                _context.Database.ExecuteSqlRaw($"TRUNCATE TABLE public.\"{dbSet.Name.FirstCharToUpper()}\" CASCADE");
            }
            catch
            {
                continue;
            }

        _context.ChangeTracker.DetectChanges();
    }

    public async Task CleanDB(IEnumerable<SocketGuild> guilds = null)
    {
        var c = await _context.Clan.AsAsyncEnumerable().ToListAsync();
        var m = await _context.Member.AsAsyncEnumerable().ToListAsync();

        // clean dead member data
        if (c is null || c.Count == 0 || m is null || m.Count == 0)
            return;

        m.Where(x => x.ClanId == 0 || x.Name == string.Empty).ForEach(x =>
        {
            var id = x.ClanId;
            _context.Remove(x);

            if (id.HasValue)
                _context.Remove(c.FirstOrDefault(y => y.Id == id));
        });
        await _context.SaveChangesAsync();

        m.Where(x => !x.IsActive || x.ClanId == null || x.Role == Role.ExMember).ForEach(x =>
        {
            x.IsActive = false;
            x.ClanId = null;
            x.Role = Role.ExMember;
        });
        await _context.SaveChangesAsync();

        m.Where(x => string.IsNullOrWhiteSpace(x.Name)).ForEach(x => _context.Remove(x));
        await _context.SaveChangesAsync();

        // clean dead channel data
        if (guilds is not null)
        {
            foreach (var g in guilds)
                _logger?.LogInformation($"Name: {g.Name} IsConntected: {g.IsConnected}");

            var ch = await LoadChannelsAsync(loadAll: true);
            ch.Where(x => !guilds.Select(y => y.Id).Contains(x.GuildId)).ForEach(cha => _context.Remove(cha));
            await _context.SaveChangesAsync();

            var gs = await LoadAllGuildSettingsAsync();
            gs.Where(x => !guilds.Select(y => y.Id).Contains(x.GuildId)).ForEach(gss => _context.Remove(gss));
            await _context.SaveChangesAsync();

            gs = await LoadAllGuildSettingsAsync();
            gs.Where(x => x.GuildName == null).ForEach(gss => gss.GuildName = guilds.FirstOrDefault(y => y.Id == gss.GuildId).Name);
            await _context.SaveChangesAsync();
        }
    }
}
