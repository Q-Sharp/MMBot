using TTMMBot.Data;

namespace TTMMBot.Services
{
    public interface IAdminService
    {
        Context Context { get; set; }
        GlobalSettings Settings { get; set; }
        void Reorder();
    }
}