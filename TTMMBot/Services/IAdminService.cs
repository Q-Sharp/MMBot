using TTMMBot.Data;

namespace TTMMBot.Services
{
    public interface IAdminService
    {
        Context Context { get; set; }
        GlobalSettingsService Settings { get; set; }
        void Reorder();
    }
}