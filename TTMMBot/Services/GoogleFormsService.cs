using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace TTMMBot.Services
{
    public class GoogleFormsService : IGoogleFormsService
    {
        private ILogger<IGoogleFormsService> _logger;

        public GoogleFormsService(ILogger<IGoogleFormsService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SubmitAsync(string url, string playerTag)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(playerTag))
                return false;

            try
            {
                var gf = new GoogleFormsToolkitLibrary.GoogleFormsToolkitLibrary();
                var fields = await gf.LoadGoogleFormStructureAsync(url);

                var filled = fields.QuestionFieldList.Where(x => x.IsAnswerRequired).Select(x => new KeyValuePair<string, string>($"entry.{x.AnswerSubmissionId}", playerTag));
                
                return await gf.SubmitToGoogleFormAsync(url, new Dictionary<string, string>(filled));
            }
            catch(Exception e)
            { 
                _logger.LogError(e.Message);
            }

            return false;
        }
    }
}
