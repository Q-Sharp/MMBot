using System.Threading.Tasks;
using Discord.Commands;

namespace MMBot.Modules.Interfaces
{
    public interface IHelpModule
    {
        Task<RuntimeResult> HelpAsync();
        Task<RuntimeResult> HelpAsync([Remainder] string command);
    } 
}
