using FakeItEasy;
using TTMMBot.Services;
using TTMMBot.Services.Interfaces;
using Xunit;

namespace TTMMBot.Tests.Services
{
    public class MemberSortServiceTests
    {
        [Fact]
        public void GetFutureClan()
        {
            var mm = GetMemberSortService();

            Assert.Equal(1, mm.GetFutureClan(1, 20));
            Assert.Equal(1, mm.GetFutureClan(20, 20));
            Assert.Equal(2, mm.GetFutureClan(21, 20));
            Assert.Equal(3, mm.GetFutureClan(41, 20));
            Assert.Equal(1, mm.GetFutureClan(19, 19));
            Assert.Equal(1, mm.GetFutureClan(100, 100));
        }

        private MemberSortService GetMemberSortService()
        {
            var dbs = A.Fake<IDatabaseService>();
            var gss = A.Fake<IGuildSettingsService>();
            var mss = new MemberSortService(dbs, gss);

            return mss;
        }
    }
}
