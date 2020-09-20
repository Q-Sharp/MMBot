using System.Collections.Generic;
using TTMMBot.Data;

namespace TTMMBot.Services
{
    public interface IAdminService
    {
        Context Context { get; set; }
        IGlobalSettingsService Settings { get; set; }
        ICommandHandler CommandHandler { get; set; }
        void Reorder();
    }
}