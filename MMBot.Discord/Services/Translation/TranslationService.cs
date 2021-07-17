using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.Translation
{
    public class TranslationService : MMBotService<TranslationService>, ITranslationService
    {
        private readonly IHttpClientFactory _clientFactory;

        public TranslationService(IHttpClientFactory clientFactory, ILogger<TranslationService> logger) : base(logger) => _clientFactory = clientFactory;

        public async Task<string> TranslateTextAsync(string input)
        {
            try
            {
                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q={Uri.EscapeUriString(input)}";
                var httpClient = _clientFactory.CreateClient();
                var result = await httpClient.GetStringAsync(url);

                return JsonDocument.Parse(result).RootElement[0][0][0].GetString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
