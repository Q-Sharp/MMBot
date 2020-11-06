using System.Threading.Tasks;
using Discord.Commands;
using MMBot.Modules.Énum;

namespace MMBot.Modules.Interfaces
{
    public interface IMemberModule
    {
        Task<RuntimeResult> List(SortBy sortBy);
        Task<RuntimeResult> Sort();
        Task<RuntimeResult> Changes(string compact = null);
        Task<RuntimeResult> Profile(string name = null);
        Task<RuntimeResult> ShowAll(string propertyName, [Remainder] string value);
        Task<RuntimeResult> Delete(string name);
        Task<RuntimeResult> Set(string name, string propertyName, [Remainder] string value);
        Task<RuntimeResult> Create(string name);
    }
}
