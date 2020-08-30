using System.Threading.Tasks;
using Discord.Commands;

namespace TTMMBot.Modules
{
    public interface IHelpModule
    {
        GlobalSettings Gm { get; set; }
        Task HelpAsync();
        Task HelpAsync([Remainder] string command);
    }
}