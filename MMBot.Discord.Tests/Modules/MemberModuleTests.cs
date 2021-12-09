using FakeItEasy;
using System.Linq;
using System.Threading.Tasks;
using MMBot.Discord.Modules.Member;
using MMBot.Discord.Services.Interfaces;
using Xunit;
using Discord.Commands;
using MMBot.Data.Services.Interfaces;

namespace MMBot.Discord.Tests.Modules
{
    public class MemberModuleTests
    {
        private IDatabaseService _dbs;
        private ICommandHandler _ch;
        private IGuildSettingsService _gs;
        private IMemberSortService _mss;

        private MemberModule GetMemberModule()
        {
            _dbs = A.Fake<IDatabaseService>();
            _ch = A.Fake<ICommandHandler>();
            _gs = A.Fake<IGuildSettingsService>();
            _mss = A.Fake<IMemberSortService>();

            return A.Fake<MemberModule>(x => x.WithArgumentsForConstructor(() => new MemberModule(_dbs, _ch, _mss, _gs)));
        }


        //[Fact]
        //public async Task CreateMemberAsync()
        //{
        //    var mm = GetMemberModule();
        //    await mm.Create("Member");

        //    A.CallTo(() => _dbs.CreateMemberAsync(0)).WithAnyArguments().MustHaveHappened();
        //    A.CallTo(() => _dbs.SaveDataAsync()).WithAnyArguments().MustHaveHappened();
        //    A.CallTo(mm).Where(x => x.Method.Name == "ReplyAsync").WithAnyArguments().MustHaveHappened();
        //}
    }
}
