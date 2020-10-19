using System.Threading.Tasks;
using Discord.Commands;

namespace TTMMBot.Modules.Interfaces
{
    public interface IMemberModule
    {
        Task List();
        Task Sort();
        Task Changes(string compact = null);
        Task Profile(string name = null);
        Task ShowAll(string propertyName, [Remainder] string value);
        Task Delete(string name);
        Task Set(string name, string propertyName, [Remainder] string value);
        Task Create(string name);
    }
}
