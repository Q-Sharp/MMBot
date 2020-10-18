using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TTMMBot.Helpers;
using TTMMBot.Services.Interfaces;

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
                var fields = await LoadGoogleFormStructureAsync(url);
                var answerReq = fields.QuestionFieldList.Where(x => x.IsAnswerRequired).ToList();

                var answersFilled = new List<KeyValuePair<string, string>>();

                var filled = answerReq.Where(x => x.QuestionText.Contains("tag", StringComparison.InvariantCultureIgnoreCase) && x.QuestionType == GoogleFormsFieldType.ShortAnswerField);
                answersFilled.AddRange(filled.Select(x => new KeyValuePair<string, string>($"entry.{x.AnswerSubmissionId}", playerTag)));

                filled.ForEach(x => answerReq.Remove(x));
                //var notfilled = fields.QuestionFieldList.Where(x => !x.IsAnswerRequired).Select(x => new KeyValuePair<string, string>($"entry.{x.AnswerSubmissionId}", ""));

                while(answerReq.Any())
                {
                    await Task.Delay(TimeSpan.FromSeconds(30));
                }

                return await SubmitToGoogleFormAsyncTest(fields.FormId, new Dictionary<string, string>(answersFilled/*.Concat(notfilled)*/));
            }
            catch(Exception e)
            { 
                _logger.LogError(e.Message);
            }

            return false;
        }

        private async Task<bool> SubmitToGoogleFormAsyncTest(string formID, Dictionary<string, string> formData)
        {
            var url = $"https://docs.google.com/forms/d/e/{formID}/formResponse";
            var client = _clientFactory?.CreateClient("forms");
            var content = new FormUrlEncodedContent(formData);
            var response = await client.PostAsync(url, content);

            if (response.StatusCode == HttpStatusCode.OK)
                return true;

            return false;
        }

        /// <summary>
        /// Loading Google Form's generic information
        /// and Question Field list data including
        /// Question Type, Answer Options, Submission Id, etc
        /// </summary>
        /// <param name="yourGoogleFormsUrl"></param>
        public async Task<GoogleForm> LoadGoogleFormStructureAsync(string yourGoogleFormsUrl)
        {
            var web = new HtmlWeb();
            var htmlDoc = await web.LoadFromWebAsync(yourGoogleFormsUrl).ConfigureAwait(false);

            var publicLoadData = htmlDoc.DocumentNode.SelectNodes("//script")
                .Where(x => x.GetAttributeValue("type", "").Equals("text/javascript") && x.InnerHtml.Contains("FB_PUBLIC_LOAD_DATA_"))
                .First().InnerHtml;

            var beginIndex = publicLoadData.IndexOf("[", StringComparison.Ordinal);
            var lastIndex = publicLoadData.LastIndexOf(";", StringComparison.Ordinal);
            var fbPublicJsScriptContentCleanedUp = publicLoadData[beginIndex..lastIndex].Trim();

            var jArray = JArray.Parse(fbPublicJsScriptContentCleanedUp);

            var googleForm = new GoogleForm
            {
                QuestionFieldList = new List<GoogleFormField>(),
                Description = jArray[1][0].ToObject<string>(),
                Title = jArray[1][8].ToObject<string>(),
                FormId = jArray[14].ToObject<string>().Substring(2),
                FormDocName = jArray[3].ToObject<string>()
            };

            foreach (var field in jArray[1][1])
            {
                // Check if this Field is submittable or not
                // index [4] contains the Field Answer 
                // Submit Id of a Field Object 
                // ex: ignore Fields used as Description panels
                // ex: ignore Image banner fields
                if (field.Count() < 4 && !field[4].HasValues)
                    continue;

                var googleFormField = new GoogleFormField();

                // Load the Question Field data
                var questionTextValue = field[1]; // Get Question Text
                var questionText = questionTextValue.ToObject<string>();

                var questionTypeCodeValue = field[3].ToObject<int>(); // Get Question Type Code   
                var isRecognizedFieldType = Enum.TryParse(questionTypeCodeValue.ToString(), out GoogleFormsFieldType questionTypeEnum);

                var answerOptionsList = new List<string>();
                var answerOptionsListValue = field[4][0][1].ToList(); // Get Answers List
                // List of Answers Available
                if (answerOptionsListValue.Count > 0)
                {
                    foreach (var answerOption in answerOptionsListValue)
                        answerOptionsList.Add(answerOption[0].ToString());
                }

                var answerSubmitIdValue = field[4][0][0]; // Get Answer Submit Id
                var isAnswerRequiredValue = field[4][0][2]; // Get if Answer is Required to be Submitted
                var answerSubmissionId = answerSubmitIdValue.ToObject<string>();
                var isAnswerRequired = isAnswerRequiredValue.ToObject<int>() == 1; // 1 or 0

                googleFormField.QuestionText = questionText;
                googleFormField.QuestionType = questionTypeEnum;
                googleFormField.AnswerOptionList = answerOptionsList;
                googleFormField.AnswerSubmissionId = answerSubmissionId;
                googleFormField.IsAnswerRequired = isAnswerRequired;

                googleForm.QuestionFieldList.Add(googleFormField);
            }

            return googleForm;
        }
    }
}
