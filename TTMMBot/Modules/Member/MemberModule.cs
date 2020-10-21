﻿using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using TTMMBot.Helpers;
using TTMMBot.Modules.Interfaces;
using TTMMBot.Services.Interfaces;

namespace TTMMBot.Modules.Member
{
    [Name("Member")]
    [Group("Member")]
    [Alias("M", "Members")]
    public partial class MemberModule : ModuleBase<SocketCommandContext>, IMemberModule
    {
        private readonly IDatabaseService _databaseService;
        private readonly ICommandHandler _commandHandler;
        private readonly IMemberSortService _memberSortService;

        public MemberModule(IDatabaseService databaseService, ICommandHandler commandHandler, IMemberSortService memberSortService)
        {
            _databaseService = databaseService;
            _commandHandler = commandHandler;
            _memberSortService = memberSortService;
        }

        [Command("Profile", RunMode = RunMode.Async)]
        [Alias("p")]
        [Summary("Shows all information of a member.")]
        public async Task Profile(string name = null)
        {
            try
            {
                var m = name != null
                    ? await (await _databaseService.LoadMembersAsync()).FindAndAskForMember(name, Context.Channel, _commandHandler)
                    : (await _databaseService.LoadMembersAsync()).FirstOrDefault(x => x.Discord == Context.User.GetUserAndDiscriminator());

                if (m == null)
                    return;

                var imageUrl = await Task.Run(() => Context.Guild.Users.FirstOrDefault(x => x.GetUserAndDiscriminator() == m.Discord)?.GetAvatarUrl());
                var e = m.GetEmbedPropertiesWithValues(imageUrl);
                await ReplyAsync("", false, e as Embed);

            }
            catch (Exception e)
            {
                await ReplyAsync($"{e.Message}");
            }
        }
    }
}
