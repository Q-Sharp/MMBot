using TTMMBot.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace TTMMBot.Data
{
    public interface ITTMMBotContext
    {
        DbSet<Clan> Clan { get; set; }
        DbSet<Member> Member { get; set; }
    }
}
