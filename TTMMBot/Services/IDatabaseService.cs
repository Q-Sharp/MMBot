using TTMMBot.Data.Entities;
using System.Threading.Tasks;

namespace TTMMBot.Services
{
    public interface IDatabaseService
    {
        Task Migrate();
        Task<Member> LoadAllMemberData();
        Task SaveData();

    }   
}