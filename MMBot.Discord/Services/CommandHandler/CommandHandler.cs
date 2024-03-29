﻿using System.Diagnostics;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MMBot.Data.Contracts;
using MMBot.Data.Contracts.Entities;
using MMBot.Data.Contracts.Helpers;
using MMBot.Data.Helpers;
using MMBot.Discord.Helpers;
using MMBot.Discord.Modules;
using MMBot.Discord.Services.Interfaces;

namespace MMBot.Discord.Services.CommandHandler;

public partial class CommandHandler : ICommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _services;
    private readonly ILogger<CommandHandler> _logger;

    private IGuildSettingsService _gs;
    private IDatabaseService _databaseService;
    private IAdminService _adminService;
    private ITimerService _timerService;

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
        _timerService = _services.GetService<ITimerService>();
        _adminService = _services.GetService<IAdminService>();

        _commands.CommandExecuted += CommandExecutedAsync;
        _commands.Log += LogAsync;

        _client.MessageReceived += Client_HandleCommandAsync;
        _client.ReactionAdded += Client_ReactionAdded;
        _client.Ready += Client_Ready;
        _client.Log += LogAsync;
        _client.Disconnected += Client_Disconnected;

        _client.ChannelDestroyed += Client_ChannelDestroyed;
    }

    private async Task Client_ChannelDestroyed(SocketChannel arg)
    {
        if (arg is IGuildChannel gc)
        {
            var gs = await _databaseService.LoadGuildSettingsAsync(gc.GuildId);
            var dbRooms = await _databaseService.LoadPersonalRooms(gc.Id);

            var dc = dbRooms.FirstOrDefault(x => x.RoomId == gc.Id);

            if (dc != null)
                _databaseService.DeletePersonalRoom(dc);
        }
    }

    public async Task LogAsync(LogMessage logMessage)
    {
        if (logMessage.Exception is CommandException cmdException)
        {
             await cmdException.Context.Channel.SendMessageAsync("Something went catastrophically wrong!");
            _logger.LogError($"{cmdException.Context.User} failed to execute '{cmdException.Command.Name}' in {cmdException.Context.Channel}.", logMessage.Exception);
        }
    }

    public async Task Client_Ready()
    {
        _logger.Log(LogLevel.Information, "Bot is connected!");

        // clean db if needed
        try
        {
            _logger.Log(LogLevel.Information, "Starting CleanUp");
            await _databaseService.CleanDB(_client.Guilds.Select(x => new Guild(x.Id, x.Name)));
            _logger.Log(LogLevel.Information, "CleanUp finished!");
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, e, "Error in cleanup");
        }

        // handle restart information
        var r = await _databaseService.ConsumeRestart();
        if (r is not null)
        {
            var dest = _client.GetGuild(r.Guild).GetTextChannel(r.Channel);

             _ = r.DBImport
                ? await dest.SendMessageAsync("Database imported!")
                : await dest.SendMessageAsync("Bot service has been restarted!");
        }

        // reinit timers
        (await _databaseService.LoadTimerAsync())?
            .Where(x => x.IsActive && _timerService.Check(x))
            .ForEach(x => _timerService.Start(x, true));

        // set status
        await _client.SetGameAsync($"Member Manager 2022");
        _logger.Log(LogLevel.Information, "Bot is online!");
    }

    public async Task Client_HandleCommandAsync(SocketMessage arg)
    {
        if (arg is not SocketUserMessage msg)
            return;

        var context = new SocketCommandContext(_client, msg);

        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot)
            return;

        var settings = await _gs.GetGuildSettingsAsync(context.Guild.Id);
        var pos = 0;

        if (msg.HasStringPrefix(settings.Prefix, ref pos, StringComparison.OrdinalIgnoreCase) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
        {
             await _commands.ExecuteAsync(context, pos, _services);
        }
    }

    public async Task Client_Disconnected(Exception arg)
    {
        _logger.LogError(arg.Message, arg);

        await Task.Run(() =>
        {
            var mmBot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);
            Process.Start(mmBot);
            Environment.Exit(0);
        });
    }

    public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        // error happened
        if (!command.IsSpecified)
        {
             await context.Channel.SendMessageAsync($"I don't know this command: {context.Message}");
            return;
        }

        if (result is MMBotResult runTimeResult)
        {
            if (command.Value.Module.Name == "Admin")
            {
                _ = DeleteMessage(context.Message, runTimeResult.AnswerSent);
                return;
            }

            if (result.IsSuccess)
            {
                if (runTimeResult.Reason is not null)
                {
                    var m = await context.Channel.SendMessageAsync(runTimeResult.Reason);

                    if (command.Value.Module.Name == "Admin")
                         _ = DeleteMessage(context.Message, m);
                }
                return;
            }

            //switch (runTimeResult?.Error ?? 0)
            //{
            //    case CommandError.BadArgCount:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;

            //    case CommandError.Exception:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;

            //    case CommandError.MultipleMatches:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;

            //    case CommandError.ObjectNotFound:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;

            //    case CommandError.ParseFailed:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;

            //    case CommandError.UnknownCommand:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;

            //    case CommandError.UnmetPrecondition:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;

            //    case CommandError.Unsuccessful:
            //        await context.Channel.SendMessageAsync(runTimeResult.Reason);
            //        break;
            //}

             await context.Channel.SendMessageAsync($"{runTimeResult.Error}: {runTimeResult.Reason}");

            var member = context.User.GetUserAndDiscriminator();
            var moduleName = command.Value.Module.Name;
            var commandName = command.Value.Name;

            _logger.LogError($"{member} tried to use {commandName} (module: {moduleName}) this resultet in a {runTimeResult?.Error?.ToString()}");
        }
    }

    private static async Task DeleteMessage(IMessage userMsg, IMessage answer)
    {
        await Task.Delay(TimeSpan.FromMinutes(2));

        if (userMsg is not null)
            await userMsg.DeleteAsync(new RequestOptions { AuditLogReason = "Autoremoved" });

        if (answer is not null)
            await answer.DeleteAsync(new RequestOptions { AuditLogReason = "Autoremoved" });
    }
}
