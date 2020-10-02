using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Logging;
using TTMMBot.Services;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules
{
    public interface IClanModule
    {
        ILogger<ClanModule> Logger { get; set; }
        IDatabaseService DatabaseService { get; set; }
        Task List(string tag = null);
        Task Delete(string tag);
        Task SetCommand(string tag, string propertyName, [Remainder] string value);
        Task Create(string tag, [Remainder] string name);
        Task AddMember(string tag, string memberName);
    }
}