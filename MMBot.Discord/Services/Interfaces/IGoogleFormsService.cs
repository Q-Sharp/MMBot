using System.Threading.Tasks;
using MMBot.Discord.Services.GoogleForms;

namespace MMBot.Discord.Services.Interfaces
{
    public interface IGoogleFormsService
    {
        Task<GoogleFormsAnswers> LoadAsync(string url);
        Task<bool> SubmitToGoogleFormAsync(GoogleFormsAnswers gfa);
    }
}