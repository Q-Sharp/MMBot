using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Helpers;
using MMBot.Services.GoogleForms;
using MMBot.Services.Interfaces;

namespace MMBot.Services.CommandHandler
{
    public partial class CommandHandler : ICommandHandler
    {
        private void CheckGoogleForms(SocketMessage arg, ulong guildId)
        {
            if (_formsChannelList.Select(x => x.Item1).Contains(arg.Channel))
                _ = Task.Run(async () => await HandleGoogleForms(guildId, arg, _formsChannelList.Where(x => x.Item1 == arg.Channel).FirstOrDefault().Item2));
        }

        private async Task ReInitGoogleFormsAsync()
        {
            var ch = await _databaseService.LoadChannelsAsync();

            if(ch == null)
                return;

            ch?.ForEach(cha =>
            {
                var obserChannel = _client?.GetGuild(cha.GuildId)?.GetTextChannel(cha.TextChannelId);
                var answerChannel = _client?.GetGuild(cha.GuildId)?.GetTextChannel(cha.AnswerTextChannelId);

                if(obserChannel != null && answerChannel !=  null)
                    _formsChannelList.Add(new Tuple<ISocketMessageChannel, ISocketMessageChannel>(obserChannel, answerChannel));
            });            
        }

        private async Task HandleGoogleForms(ulong guildId, SocketMessage arg, ISocketMessageChannel questionsChannel)
        {
            var urls = arg.Content.GetUrl();
            var m = await _databaseService.LoadMembersAsync();

            if(!urls.Any() || !m.Any(z => z.AutoSignUpForFightNight && z.PlayerTag != null))
                
                return;

            var member = m.Where(z => z.AutoSignUpForFightNight && z.PlayerTag != null).ToList();

            GoogleFormsAnswers gfa = null;

            foreach(var u in urls)
            {
                gfa = await _googleFormsSubmissionService.LoadAsync(u);
                
                if(gfa == null)
                    continue;
            
                await gfa.AddPlayerTagToAnswers(member.FirstOrDefault().PlayerTag);
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
                        await msg.AddReactionsAsync(emojis);
                
                        await AddToReactionList(guildId, msg, async (r, u) =>
                        {
                            cancel = r.Name == "2️⃣";
                            await msg.DeleteAsync();
                        }, 
                        false);

                        if(cancel)
                            return;

                        await questionsChannel.SendMessageAsync($"Questions for the form: {gfa.Title}");
                        foreach(var q in gfa.OpenFields)
                        {
                            var qMsg = await questionsChannel.SendMessageAsync($"Question: {q.QuestionText}");

                            var context = new CommandContext(_client, qMsg);
                            var sC = new SocketCommandContext(_client, questionsChannel.GetCachedMessage(qMsg.Id) as SocketUserMessage);

                            var response = await _interactiveService.NextMessageAsync(sC, false, true, timeout: TimeSpan.FromHours(2));
                            if (response != null)
                            {
                                await gfa.AnswerQuestionManual(q.AnswerSubmissionId, response.Content);
                                await questionsChannel.SendMessageAsync($"Answer added: {response.Content}");
                            }  
                            else
                                return;
                        }
                    }
                    catch
                    {
                        // ignore
                    }
                }

                foreach(var me in member)
                {
                    await gfa.AddPlayerTagToAnswers(me.PlayerTag);
                    var success = await _googleFormsSubmissionService.SubmitToGoogleFormAsync(gfa);
                    if(success)
                        await questionsChannel.SendMessageAsync($"{me} using {me.PlayerTag} has been successfully joined {gfa.Title}");

                    var random = new Random((int)DateTime.UtcNow.Ticks);
                    await Task.Delay(TimeSpan.FromSeconds(random.Next(20, 120)));
                }
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
