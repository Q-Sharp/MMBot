using System;
using System.Threading.Tasks;

namespace MMBot.Discord.Services.Interfaces
{
    public interface ITranslationService
    {
        Task<Tuple<string, string>> TranslateTextAsync(string input);
    }
}
