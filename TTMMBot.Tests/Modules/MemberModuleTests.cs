using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TTMMBot.Modules.Member;
using TTMMBot.Services.Interfaces;
using Xunit;

namespace TTMMBot.Tests.Modules
{
    public class MemberModuleTests
    {
        private IGlobalSettingsService GlobalSettings;
        private IDatabaseService DatabaseService;
        private ICommandHandler CommandHandler;
        private IMemberSortService MemberSortService;

        private MemberModule GetMemberModule()
        {
            GlobalSettings = A.Fake<IGlobalSettingsService>();
            DatabaseService = A.Fake<IDatabaseService>();
            CommandHandler = A.Fake<ICommandHandler>();
            MemberSortService = A.Fake<IMemberSortService>();

            return A.Fake<MemberModule>(x => x.WithArgumentsForConstructor(() => new MemberModule(DatabaseService, CommandHandler, GlobalSettings, MemberSortService)));
        }


        [Fact]
        public async Task CreateMemberAsync()
        {
            var mm = GetMemberModule();
            await mm.Create("Member");

            A.CallTo(() => DatabaseService.CreateMemberAsync()).WithAnyArguments().MustHaveHappened();
            A.CallTo(() => DatabaseService.SaveDataAsync()).WithAnyArguments().MustHaveHappened();
            A.CallTo(mm).Where(x => x.Method.Name == "ReplyAsync").WithAnyArguments().MustHaveHappened();
        }
    }
}
