using System.Threading.Tasks;
using Discord.Commands;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    public interface IHelpModule
    {
        GlobalSettingsService Gm { get; set; }
        Task HelpAsync();
        Task HelpAsync([Remainder] string command);
    }
}