using System.Threading.Tasks;
using Discord.Commands;

namespace TTMMBot.Modules
{
    public interface IHelpModule
    {
        IGlobalSettings Gm { get; set; }
        Task HelpAsync();
        Task HelpAsync([Remainder] string command);
    }
}