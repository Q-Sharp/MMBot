using System.Threading.Tasks;
using Discord.Commands;
using MMBot.Services;

namespace MMBot.Modules.Interfaces
{
    public interface IHelpModule
    {
        Task HelpAsync();
        Task HelpAsync([Remainder] string command);
    }
}
