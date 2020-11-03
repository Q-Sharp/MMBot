using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MMBot.Services.GoogleForms;

namespace MMBot.Services.Interfaces
{
    public interface IGoogleFormsService
    {
        Task<GoogleFormsAnswers> LoadAsync(string url);
        Task<bool> SubmitToGoogleFormAsync(GoogleFormsAnswers gfa);
    }
}