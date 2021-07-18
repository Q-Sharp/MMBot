using System.Threading.Tasks;
using Discord.Commands;

namespace MMBot.Discord.Modules.Interfaces
{
    public interface ITranslationModule
    {
        Task<RuntimeResult> Translate([Remainder] string text);
    }
}