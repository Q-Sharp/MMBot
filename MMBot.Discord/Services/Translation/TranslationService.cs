using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using MMBot.Discord.Services.Interfaces;
using MMBot.Helpers;

namespace MMBot.Discord.Services.Translation
{
    public class TranslationService : MMBotService<TranslationService>, ITranslationService
    {
        private readonly IHttpClientFactory _clientFactory;

        public TranslationService(IHttpClientFactory clientFactory, ILogger<TranslationService> logger) : base(logger) => _clientFactory = clientFactory;

        public async Task<Tuple<string, string>> TranslateTextAsync(string input)
        {
            try
            {
                input = DiscordHelpers.SeperateMention(input, out var mention);

                var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q={Uri.EscapeUriString(input)}";
                var httpClient = _clientFactory.CreateClient();
                var result = await httpClient.GetStringAsync(url);

                return Tuple.Create(string.Concat(JsonDocument.Parse(result).RootElement[0].EnumerateArray().Select(y => y[0].GetString())), mention);
            }
            catch
            {
                return Tuple.Create<string, string>(string.Empty, null);
            }
        }
    }
}
