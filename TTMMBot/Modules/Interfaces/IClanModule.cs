using Discord.Commands;
using System.Threading.Tasks;

namespace TTMMBot.Modules.Interfaces
{
    public interface IClanModule
    {
        Task List(string tag = null);
        Task Delete(string tag);
        Task SetCommand(string tag, string propertyName, [Remainder] string value);
        Task Create(string tag, [Remainder] string name);
        Task AddMember(string tag, string memberName);
    }
}