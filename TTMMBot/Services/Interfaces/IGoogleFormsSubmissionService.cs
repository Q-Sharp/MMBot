using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TTMMBot.Services.Interfaces
{
    public interface IGoogleFormsSubmissionService
    {
        void SetCheckboxValues(string key, params string[] values);
        void SetFieldValues(Dictionary<string, string> data);
        void SetUrl(string url);
        Task<HttpResponseMessage> SubmitAsync();
    }
}