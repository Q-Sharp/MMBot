using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TTMMBot.Helpers;
using TTMMBot.Services.GoogleForms;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Services.CommandHandler
{
    public partial class CommandHandler : ICommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IList<Tuple<ISocketMessageChannel, ISocketMessageChannel>> _channelList = new List<Tuple<ISocketMessageChannel, ISocketMessageChannel>>();
        private readonly IDictionary<string, string> Answers = new Dictionary<string, string>();
        private readonly IDictionary<string, string> Questions = new Dictionary<string, string>();

        private IGuildSettingsService _gs;
        private IDatabaseService _databaseService;
        private IGoogleFormsService _googleFormsSubmissionService;
        private InteractiveService _interactiveService;

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client, ILogger<CommandHandler> logger)
        {
            _commands = commands;
            _services = services;
            _client = client;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(GetType().Assembly, _services);

            _databaseService = _services.GetService<IDatabaseService>();
            _gs = _services.GetService<IGuildSettingsService>();
            _googleFormsSubmissionService = _services.GetService<IGoogleFormsService>();
            _interactiveService = _services.GetService<InteractiveService>();

            _commands.CommandExecuted += CommandExecutedAsync;

            _client.MessageReceived += Client_HandleCommandAsync;
            _client.ReactionAdded += Client_ReactionAdded;
            _client.Disconnected += Client_Disconnected;

            _client.Ready += Client_Ready;
        }

        private async Task Client_Ready()
        {
            var r = await _databaseService.ConsumeRestart();
            _logger.Log(LogLevel.Information, "Bot is connected!");

            if (r != null)
                await _client.GetGuild(r.Item1)
                    .GetTextChannel(r.Item2)
                    .SendMessageAsync("Bot service has been restarted!");

            await _databaseService.CleanDB();

           (await _databaseService.LoadChannelsAsync())?
                .Select(x => 
                    new Tuple<ISocketMessageChannel, ISocketMessageChannel>(_client.GetGuild(x.GuildId).GetTextChannel(x.TextChannelId), 
                                                                            _client.GetGuild(x.GuildId).GetTextChannel(x.AnswerTextChannelId)))?.ForEach(x => _channelList.Add(x));
        }

        private async Task Client_Disconnected(Exception arg)
        {
            _logger.LogError(arg.Message);

            await Task.Run(() =>
            { 
                Process.Start(AppDomain.CurrentDomain.FriendlyName);
                Environment.Exit(0);
            });
        }

        public async Task Client_HandleCommandAsync(SocketMessage arg)
        {
            if (_channelList.Select(x => x.Item1).Contains(arg.Channel))
            {
                _ = Task.Run(async () => await HandleGoogleForms(arg, _channelList.Where(x => x.Item1 == arg.Channel).FirstOrDefault().Item2));
                return;
            }

            if (!(arg is SocketUserMessage msg) || msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
            _gs?.SetGuild((msg.Channel as IGuildChannel).Id);

            var pos = 0;
            if (msg.HasStringPrefix(_gs.Prefix, ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos) || msg.Content.ToLower().StartsWith(_gs.Prefix.ToLower()))
            {
                var context = new SocketCommandContext(_client, msg);
                var result = await _commands.ExecuteAsync(context, pos, _services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private async Task HandleGoogleForms(SocketMessage arg, ISocketMessageChannel questionsChannel)
        {
            var urls = arg.Content.GetUrl();
            var m = await _databaseService.LoadMembersAsync();

            if(!urls.Any() || !m.Any(z => z.AutoSignUpForFightNight && z.PlayerTag != null))
                
                return;

            var mtags = m.Where(z => z.AutoSignUpForFightNight && z.PlayerTag != null).Select(x => x.PlayerTag).ToList();

            GoogleFormsAnswers gfa = null;

            foreach(var u in urls)
            {
                gfa = await _googleFormsSubmissionService.LoadAsync(u);
                
                if(gfa != null)
                    break;
            }

            await gfa.AddPlayerTagToAnswers(mtags.FirstOrDefault());
            await gfa.AnswerQuestionsAuto();

            if(!gfa.AllFieldsAreFilledWithAnswers)
            {
                try
                {
                    var msg = await questionsChannel.SendMessageAsync($"There are problems with auto fill of the latest form.{Environment.NewLine}What should we do now? (1 = answer, 2 = cancel)");

                    var emojis = new IEmote[] 
                    {
                       new Emoji("1️⃣"),
                       new Emoji("2️⃣")
                    }
                    .ToArray();

                    var cancel = false;
                    await msg.AddReactionsAsync(emojis);
                
                    await AddToReactionList(msg, async (r, u) =>
                    {
                        cancel = r.Name == "2️⃣";
                        await msg.DeleteAsync();
                    }, 
                    false);

                    if(cancel)
                        return;

                    foreach(var q in gfa.OpenFields)
                    {
                    
                        var qMsg = await questionsChannel.SendMessageAsync($"Question: {q.QuestionText}");

                        var context = new CommandContext(_client, qMsg);
                        var sC = new SocketCommandContext(_client, questionsChannel.GetCachedMessage(qMsg.Id) as SocketUserMessage);

                        var response = await _interactiveService.NextMessageAsync(sC, false, true, timeout: TimeSpan.FromHours(2));
                        if (response != null)
                            await gfa.AnswerQuestionManual(q.AnswerSubmissionId, response.Content);
                        else
                            return;
                    }
                }
                catch(Exception e)
                {

                }
            }

            foreach(var pTag in mtags)
            {
                await gfa.AddPlayerTagToAnswers(pTag);
                await _googleFormsSubmissionService.SubmitToGoogleFormAsync(gfa);
                var random = new Random((int)DateTime.Now.Ticks);
                await Task.Delay(TimeSpan.FromSeconds(random.Next(30, 300)));
            }
            
                
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (result.IsSuccess)
                return;

            if(!command.IsSpecified)
            {
                await context.Channel.SendMessageAsync($"I don't know this command: {context.Message}");
                return;
            }
                
            await context.Channel.SendMessageAsync($"error: {result}");
        }

        public void AddChannelToGoogleFormsWatchList(IGuildChannel channel, IGuildChannel questionsChannel)
        {
            lock (_messageIdWithReaction)
            {
                _channelList.Add(Tuple.Create(channel as ISocketMessageChannel, questionsChannel as ISocketMessageChannel));
            }
        }

        public void RemoveChannelFromGoogleFormsWatchList(IGuildChannel channel)
        {
            lock (_messageIdWithReaction)
            {
                _channelList.Remove(_channelList.FirstOrDefault(x => x.Item1 == channel as ISocketMessageChannel));
            }
        }
    }
}
