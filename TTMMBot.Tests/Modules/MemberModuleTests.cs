using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using TTMMBot.Data.Entities;
using TTMMBot.Helpers;
using TTMMBot.Modules.Enums;
using TTMMBot.Services;
using Xunit;
using FakeItEasy;
using TTMMBot.Modules;

namespace TTMMBot.Tests.Modules
{
    public class MemberModuleTests
    {
        private MemberModule getMemberModule()
        {
            var mm = new MemberModule();
            mm.GlobalSettings = A.Fake<GlobalSettings>();
            mm.CommandHandler = A.Fake<CommandHandler>();
            mm.DatabaseService = A.Fake<DatabaseService>();
           
            return mm;
        }


        [Fact]
        public async Task ChangesTest()
        {
            var foo = A.Fake<MemberModule>(x => x.WithArgumentsForConstructor(() => new MemberModule()));
        }
    }
}
