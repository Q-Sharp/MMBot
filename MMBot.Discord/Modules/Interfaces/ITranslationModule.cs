using System.Threading.Tasks;
using Discord.Commands;

namespace MMBot.Discord.Modules.Interfaces
{
    public interface ITranslationModule
    {
        Task<RuntimeResult> CreateTimer([Remainder] string text);
    }
}