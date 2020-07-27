using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using TTMMBot.Data.Entities;
using TTMMBot.TypeReaders;

namespace TTMMBot.Services
{
	public class CommandHandler
	{
		private readonly DiscordSocketClient _client;
		private readonly CommandService _commands;
		private readonly IServiceProvider _services;

		public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
		{
			_commands = commands;
			_services = services;
			_client = client;
		}

		public async Task InitializeAsync()
		{
			_commands.AddTypeReader(typeof(Member), new MemberTypeReader());
			_commands.AddTypeReader(typeof(Clan), new ClanTypeReader());

			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

			_commands.CommandExecuted += CommandExecutedAsync;
			_client.MessageReceived += HandleCommandAsync;
		}

		public async Task HandleCommandAsync(SocketMessage arg)
		{
			if (!(arg is SocketUserMessage msg) || msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

			int pos = 0;
			if (msg.HasStringPrefix("m.", ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
			{
				var context = new SocketCommandContext(_client, msg);
				var result = await _commands.ExecuteAsync(context, pos, _services);

				if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
					await msg.Channel.SendMessageAsync(result.ErrorReason);
			}
		}

		public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
		{
			if (!command.IsSpecified || result.IsSuccess)
				return;

			await context.Channel.SendMessageAsync($"error: {result}");
		}
	}
}
