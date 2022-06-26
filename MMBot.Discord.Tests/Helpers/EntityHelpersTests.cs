using MMBot.Data.Contracts.Entities;
using MMBot.Data.Contracts.Enums;
using MMBot.Discord.Helpers;
using Xunit;

namespace MMBot.Helpers;

public class EntityHelpersTests
{
    private static Member M => new() { Id = 1, Name = "ReMember", Role = Role.Member/*, SHigh = 9999*/ };

    [Fact]
    public void ChangePropertyEnumTest()
    {
        var m = M;

        var msg = m.ChangeProperty(nameof(m.Role), "Leader");

        Assert.Equal(Role.Leader, m.Role);
        Assert.Equal("The Member ReMember now uses Leader instead of Member as Role.", msg);
    }

    [Fact]
    public void ChangePropertyStringTest()
    {
        var m = M;

        var msg = m.ChangeProperty(nameof(m.Name), "Member");

        Assert.Equal("Member", m.Name);
        Assert.Equal("The Member Member now uses Member instead of ReMember as Name.", msg);
    }

    //[Fact]
    //public void ChangePropertyIntTest()
    //{
    //    var m = M;

    //    var msg = m.ChangeProperty(nameof(m.SHigh), "1337");

    //    Assert.Equal(1337, m.SHigh);
    //    Assert.Equal("The Member ReMember now uses 1337 instead of 9999 as SHigh.", msg);
    //}
}
