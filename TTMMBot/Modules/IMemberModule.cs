using System.Threading.Tasks;
using Discord.Commands;
using TTMMBot.Services;

namespace TTMMBot.Modules
{
    public interface IMemberModule
    {
        IDatabaseService DatabaseService { get; set; }
        ICommandHandler CommandHandler { get; set; }
        IGlobalSettings GlobalSettings { get; set; }
        Task List();
        Task Sort();
        Task Changes();
        Task Show(string name = null);
        Task Delete(string name);
        Task Set(string name, string propertyName, [Remainder] string value);
        Task Create(string name);
    }
}