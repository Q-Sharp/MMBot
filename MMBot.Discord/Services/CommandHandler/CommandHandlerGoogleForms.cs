using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Helpers;
using MMBot.Discord.Services.GoogleForms;
using MMBot.Discord.Services.Interfaces;
using Discord.Addons.Interactive;
using Microsoft.Extensions.Logging;

namespace MMBot.Discord.Services.CommandHandler
{
    public partial class CommandHandler : ICommandHandler
    {
        private void CheckForUseableLinks(SocketMessage arg, ulong guildId)
        {
            _ = Task.Run(async () =>
            {
                if (_formsChannelList.Select(x => x.Item1).Contains(arg.Channel))
                {
                    await HandleGoogleForms(guildId, arg, _formsChannelList.Where(x => x.Item1 == arg.Channel).FirstOrDefault().Item2);
                    await HandleYoutubeStream(guildId, arg, _formsChannelList.Where(x => x.Item1 == arg.Channel).FirstOrDefault().Item2);
                }
            });
        }

        private Task HandleYoutubeStream(ulong guildId, SocketMessage arg, ISocketMessageChannel item2) => throw new NotImplementedException();

        private async Task ReInitGoogleFormsAsync()
        {
            var ch = await _databaseService.LoadChannelsAsync();

            if(ch is null)
                return;

            ch?.ForEach(async cha =>
            {
                var obserChannel = _client?.GetGuild(cha.GuildId)?.GetTextChannel(cha.TextChannelId);
                var answerChannel = _client?.GetGuild(cha.GuildId)?.GetTextChannel(cha.AnswerTextChannelId);

                if(obserChannel is not null && answerChannel !=  null)
                    await AddChannelToGoogleFormsWatchList(obserChannel, answerChannel);
            });            
        }

        private async Task HandleGoogleForms(ulong guildId, SocketMessage arg, ISocketMessageChannel questionsChannel)
        {
            var urls = arg.Content.GetUrl();
            var m = await _databaseService.LoadMembersAsync(guildId);

            if(!urls.Any() || !m.Any(z => z.AutoSignUpForFightNight && z.PlayerTag is not null))                
                return;

            var member = m.Where(z => z.AutoSignUpForFightNight && z.PlayerTag is not null).ToList();
            
            foreach(var u in urls)
                await FillForm(u, member.Select(x => x.PlayerTag).ToArray(), questionsChannel, guildId);   
        }

        public async Task FillForm(string url, string[] playerTags, ISocketMessageChannel questionsChannel, ulong guildId)
        {
            var gfa = await _googleFormsSubmissionService.LoadAsync(url);
                
            if(gfa is null)
            {
                await questionsChannel.SendMessageAsync($"Google forms parse error for {url}!");
                return;
            }

            await gfa.AddPlayerTagToAnswers(playerTags.FirstOrDefault());
            await gfa.AnswerQuestionsAuto();

            if(!gfa.AllFieldsAreFilledWithAnswers)
            {
                try
                {
                    var users = await questionsChannel.GetUsersAsync(CacheMode.AllowDownload).FlattenAsync();
                    var mentionAll = string.Join(", ", users.Select(y => y.Mention));
                    var msg = await questionsChannel.SendMessageAsync($"I have problems to auto fill of the latest form with the tile: {gfa.Title}." + Environment.NewLine + 
                                                                      $"Wake up and help me out: {mentionAll}" + Environment.NewLine + 
                                                                      $"What should we do now? (1 = answer, 2 = cancel)");

                    var emojis = new IEmote[] 
                    {
                       new Emoji("1️⃣"),
                       new Emoji("2️⃣")
                    }
                    .ToArray();

                    var cancel = false;
                    IUser qaU = null;
                    await msg.AddReactionsAsync(emojis);
            
                    await AddToReactionList(guildId, msg, async (r, u) =>
                    {
                        cancel = r.Name == "2️⃣";
                        qaU = u;
                        await msg.DeleteAsync();
                    }, 
                    false);

                    if(cancel)
                        return;

                    await questionsChannel.SendMessageAsync($"Questions for the form: {gfa.Title}");
                    foreach(var q in gfa.OpenFields)
                    {
                        var qMsg = await questionsChannel.SendMessageAsync($"Question: {q.QuestionText}");
                        var sC = new SocketCommandContext(_client, questionsChannel.GetCachedMessage(qMsg.Id) as SocketUserMessage);

                        var response = await _interactiveService.NextMessageAsync(sC, new EnsureFromUserCriterion(qaU), timeout: TimeSpan.FromHours(2));
                        if (response is not null)
                        {
                            await gfa.AnswerQuestionManual(q.AnswerSubmissionId, response.Content);
                            await questionsChannel.SendMessageAsync($"Answer added: {response.Content}");
                        }  
                        else
                            return;
                    }
                }
                catch(Exception e)
                {
                    // ignore
                    _logger.LogError(e.Message, e);
                }
            }

            foreach(var tag in playerTags.Shuffle())
            {
                await gfa.AddPlayerTagToAnswers(tag);
                var success = await _googleFormsSubmissionService.SubmitToGoogleFormAsync(gfa);
                if(success)
                    await questionsChannel.SendMessageAsync($"{tag} was successfully added in {gfa.Title}");
            
                var random = new Random((int)DateTime.UtcNow.Ticks);
                await Task.Delay(TimeSpan.FromSeconds(random.Next(2, 5)));
            }
        }

        public async Task AddChannelToGoogleFormsWatchList(IGuildChannel channel, IGuildChannel questionsChannel)
        {
            using (await _mutex.LockAsync())
                _formsChannelList.Add(Tuple.Create(channel as ISocketMessageChannel, questionsChannel as ISocketMessageChannel));
        }

        public async Task RemoveChannelFromGoogleFormsWatchList(IGuildChannel channel)
        {
            using (await _mutex.LockAsync())
                _formsChannelList.Remove(_formsChannelList.FirstOrDefault(x => x.Item1 == channel as ISocketMessageChannel));
        }
    }
}
