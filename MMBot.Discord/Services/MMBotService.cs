using Microsoft.Extensions.Logging;

namespace MMBot.Discord.Services
{
    public abstract class MMBotService<T> 
        where T : class
    {
        protected ILogger<T> _logger;
        public MMBotService(ILogger<T> logger) => _logger = logger;
    }
}
