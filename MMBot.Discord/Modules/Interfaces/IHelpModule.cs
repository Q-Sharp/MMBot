using System.Threading.Tasks;
using Discord.Commands;

namespace MMBot.Discord.Modules.Interfaces
{
    public interface IHelpModule
    {
        Task<RuntimeResult> HelpAsync();
        Task<RuntimeResult> HelpAsync([Remainder] string command);
    } 
}
