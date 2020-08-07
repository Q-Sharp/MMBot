using Microsoft.EntityFrameworkCore;
using TTMMBot.Data.Entities;

namespace TTMMBot.Data
{
    public interface ITTMMBotContext
    {
        DbSet<Clan> Clan { get; set; }
        DbSet<Member> Member { get; set; }
    }
}
