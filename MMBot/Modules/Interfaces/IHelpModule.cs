using System.Threading.Tasks;
using Discord.Commands;

namespace MMBot.Modules.Interfaces
{
    public interface IHelpModule
    {
        Task HelpAsync();
        Task HelpAsync([Remainder] string command);
    }
}
