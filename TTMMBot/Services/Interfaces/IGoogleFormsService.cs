using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TTMMBot.Services.Interfaces
{
    public interface IGoogleFormsService
    {
        Task<bool> SubmitAsync(string url, string playerTag);
    }
}