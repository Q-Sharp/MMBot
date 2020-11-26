using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using MMBot.Modules.Interfaces;
using MMBot.Services.Interfaces;
using MMBot.Services.Raid;

namespace MMBot.Modules.Raid
{
    //[Name("Raid")]
    //[Group("Raid")]
    //[Alias("R")]
    //public class RaidModule : MMBotModule, IRaidModule
    //{
    //    private readonly IRaidService _raidService;

    //    public RaidModule(IDatabaseService databaseService, ICommandHandler commandHandler, IGuildSettingsService guildSettings, IRaidService raidService)
    //        : base(databaseService, guildSettings, commandHandler)
    //    {
    //        _raidService = raidService;
    //    }

    //    [Command("Init")]
    //    public async Task<RuntimeResult> Init()
    //    {
    //        await _raidService?.ConnectAsync();
    //        var result = await _raidService?.GetTacticPicture();

    //        await ReplyAsync(result.ToString());
    //        return FromSuccess();
    //    }
    //}
}
