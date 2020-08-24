using TTMMBot.Data.Entities;
using TTMMBot.Data.Enums;
using Xunit;

namespace TTMMBot.Tests.Data.Entities
{
    public class EntityHelpersTests
    {
        private static Member M => new Member { MemberID = 1, Name = "ReMember", Role = Role.Member, SHigh = 9999 };

        [Fact]
        public void ChangePropertyEnumTest()
        {
            var m = M;

            m.ChangeProperty(nameof(m.Role), "Leader");

            Assert.Equal(Role.Leader, m.Role);
        }

        [Fact]
        public void ChangePropertyStringTest()
        {
            var m = M;

            m.ChangeProperty(nameof(m.Name), "Member");

            Assert.Equal("Member", m.Name);
        }

        [Fact]
        public void ChangePropertyIntTest()
        {
            var m = M;

            m.ChangeProperty(nameof(m.SHigh), "1337");

            Assert.Equal(1337, m.SHigh);
        }
    }
}
