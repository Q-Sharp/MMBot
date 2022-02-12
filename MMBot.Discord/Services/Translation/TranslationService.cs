using System.Text.Json;
using Microsoft.Extensions.Logging;
using MMBot.Discord.Helpers;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.Translation;

public class TranslationService : MMBotService<TranslationService>, ITranslationService
{
    private readonly IHttpClientFactory _clientFactory;

    public TranslationService(IHttpClientFactory clientFactory, ILogger<TranslationService> logger) : base(logger) => _clientFactory = clientFactory;

    public async Task<string> TranslateTextAsync(string input)
    {
        try
        {
            input = input.PrepareForTranslate();

            if (string.IsNullOrWhiteSpace(input))
                return input;

            var url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=en&dt=t&q={Uri.EscapeDataString(input)}";
            var httpClient = _clientFactory.CreateClient();
            var result = await httpClient.GetStringAsync(url);

            return string.Join("", JsonDocument.Parse(result).RootElement[0].EnumerateArray().Select(y => y[0].GetString()));
        }
        catch
        {
            return string.Empty;
        }
    }
}
