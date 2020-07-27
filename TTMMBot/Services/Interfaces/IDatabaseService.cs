using TTMMBot.Data.Entities;
using System.Threading.Tasks;

namespace TTMMBot.Services.Interfaces
{
    public interface IDatabaseService
    {
        IDatabaseClanService ClanService { get; set; }
        IDatabaseMemberService MemberService { get; set; }
        Task Migrate();
        Task SaveDataAsync();
    }
}