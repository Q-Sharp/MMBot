using TTMMBot.Data;

namespace TTMMBot.Services.Interfaces
{
    public interface IAdminService
    {
        Context Context { get; set; }
        IGlobalSettingsService Settings { get; set; }
        ICommandHandler CommandHandler { get; set; }
        void Reorder();
    }
}