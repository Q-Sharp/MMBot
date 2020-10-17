using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace TTMMBot.Services.GoogleForms
{
    public class GoogleFormsService : IGoogleFormsService
    {
        private readonly ILogger<IGoogleFormsService> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public GoogleFormsService(ILogger<IGoogleFormsService> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
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
                var notfilled = fields.QuestionFieldList.Where(x => !x.IsAnswerRequired).Select(x => new KeyValuePair<string, string>($"entry.{x.AnswerSubmissionId}", ""));
                
                url += "/formResponse";
                return await SubmitToGoogleFormAsyncTest(url, new Dictionary<string, string>(filled.Concat(notfilled)));
            }
            catch(Exception e)
            { 
                _logger.LogError(e.Message);
            }

            return false;
        }

        public async Task<bool> SubmitToGoogleFormAsyncTest(string yourGoogleFormsUrl, Dictionary<string, string> formData)
        {
            // Init HttpClient to send the request
            var client = _clientFactory?.CreateClient("forms");

            // Encode object to application/x-www-form-urlencoded MIME type
            var content = new FormUrlEncodedContent(formData);

            // Post the request (replace with your Google Form link)
            var response = await client.PostAsync(
                yourGoogleFormsUrl,
                content);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return true;

            _logger.LogInformation(response.ReasonPhrase);
            return false;
        }
    }

}
