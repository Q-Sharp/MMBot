using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMBot.Services.GoogleForms
{
    public class GoogleFormsAnswers
    {
        public IList<GoogleFormField> AllRequireFields { get; set; } = new List<GoogleFormField>();
        public IList<GoogleFormField> AnsweredFields { get; set; } = new List<GoogleFormField>();
        public IList<GoogleFormField> OpenFields => AllRequireFields.Where(x => !AnsweredFields.Contains(x)).ToList();
        public string Title { get; set; }
        public Dictionary<string, string> AllAnswers { get; set; } = new Dictionary<string, string>();
        public string FormId { get; set; }
        public bool AllFieldsAreFilledWithAnswers => AllAnswers.Count >= AllRequireFields?.Count;

    }

    public static class GoogleFormsAnswersExt
    {
        public static async Task<GoogleFormsAnswers> AddPlayerTagToAnswers(this GoogleFormsAnswers gfa, string playerTag, bool clearData = true)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(playerTag) || gfa is null)
                    return null;

                var filled = gfa.AllRequireFields.FirstOrDefault(x => x.QuestionText.Contains("tag", StringComparison.InvariantCultureIgnoreCase) && x.QuestionType == GoogleFormsFieldType.ShortAnswerField);
                if (filled != null)
                {
                    var entryId = $"entry.{filled.AnswerSubmissionId}";

                    if (clearData)
                    {
                        gfa.AllAnswers.Remove(entryId);
                        gfa.AnsweredFields.Remove(filled);
                    }
                        
                    gfa.AllAnswers.Add(entryId, playerTag);
                    gfa.AnsweredFields.Add(filled);
                }

                return gfa;
            });
        }

        public static async Task<GoogleFormsAnswers> AnswerQuestionsAuto(this GoogleFormsAnswers gfa)
        {
            return await Task.Run(() =>
            {
                if (gfa is null)
                    return null;

                if (gfa.AllFieldsAreFilledWithAnswers)
                    return gfa;

                var filled = gfa.AllRequireFields.FirstOrDefault(x => x.QuestionText.Contains("rank", StringComparison.InvariantCultureIgnoreCase) && x.QuestionType == GoogleFormsFieldType.DropDownField);
                if (filled != null)
                {
                    var a = filled.AnswerOptionList.FirstOrDefault(x => x.Contains("14"));
                    var entryId = $"entry.{filled.AnswerSubmissionId}";

                    gfa.AllAnswers.Add(entryId, a);
                    gfa.AnsweredFields.Add(filled);
                }

                return gfa;
            });
        }

        public static async Task<GoogleFormsAnswers> AnswerQuestionManual(this GoogleFormsAnswers gfa, string answerSubmissionId, string answer)
        {
            return await Task.Run(() =>
            {
                if (gfa is null)
                    return null;

                if (gfa.AllFieldsAreFilledWithAnswers)
                    return gfa;

                var filled = gfa.AllRequireFields.FirstOrDefault(x => x.AnswerSubmissionId == answerSubmissionId);
                if (filled != null)
                {
                    var entryId = $"entry.{filled.AnswerSubmissionId}";
                    gfa.AllAnswers.Add(entryId, answer);
                    gfa.AnsweredFields.Add(filled);
                }

                return gfa;
            });
        }
    }
}
