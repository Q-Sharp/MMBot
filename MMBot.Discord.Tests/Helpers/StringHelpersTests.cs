using MMBot.Discord.Helpers;
using Xunit;

namespace MMBot.Helpers;

public class StringHelpersTests
{
    [Fact]
    public void ChangePropertyStringTest()
    {
        var rawString = "TT > ALL www.MMBot.com nice http://www.github.com continue to w//rite bullhttpshit wwwShit www.sheeet.com/google/awspfasfjasfj/";
        var urls = rawString.GetUrl();

        Assert.Contains("www.MMBot.com", urls);
        Assert.Contains("http://www.github.com", urls);
        Assert.Contains("www.sheeet.com/google/awspfasfjasfj/", urls);
    }
}
