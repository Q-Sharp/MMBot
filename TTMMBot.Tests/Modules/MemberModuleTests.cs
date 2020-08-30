using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TTMMBot.Data;
using TTMMBot.Modules;
using TTMMBot.Services;
using Xunit;

namespace TTMMBot.Tests.Modules
{
    public class MemberModuleTests
    {
        private static MemberModule GetMemberModule()
        {
            var mm = A.Fake<MemberModule>();

            mm.GlobalSettings = A.Fake<IGlobalSettingsService>();
            mm.DatabaseService = A.Fake<IDatabaseService>();
            mm.CommandHandler = A.Fake<ICommandHandler>();
            return mm;
        }


        [Fact]
        public async Task CreateMemberAsync()
        {
            var mm = GetMemberModule();

            await mm.Create("Member");

            A.CallTo(() => mm.DatabaseService.CreateMemberAsync()).WithAnyArguments().MustHaveHappened();
            A.CallTo(() => mm.DatabaseService.SaveDataAsync()).WithAnyArguments().MustHaveHappened();
            A.CallTo(mm).Where(x => x.Method.Name == "ReplyAsync").WithAnyArguments().MustHaveHappened();
        }
    }
}
