using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TTMMBot.Services.GoogleForms;

namespace TTMMBot.Services.Interfaces
{
    public interface IGoogleFormsService : IMMBotInterface
    {
        Task<GoogleFormsAnswers> LoadAsync(string url);
        Task<bool> SubmitToGoogleFormAsync(GoogleFormsAnswers gfa);
    }
}