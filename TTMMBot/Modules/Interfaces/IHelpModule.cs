using System.Threading.Tasks;
using Discord.Commands;
using TTMMBot.Services;

namespace TTMMBot.Modules.Interfaces
{
    public interface IHelpModule
    {
        Task HelpAsync();
        Task HelpAsync([Remainder] string command);
    }
}
