using ShakeMe.Core.Services;
using Xunit;

namespace ShakeMe.Tests.Services;

public class MatchmakingServiceTests
{
    [Fact]
    public void CreateMatch_WithTwoUsers_ReturnsPair()
    {
        var service = new MatchmakingService();
        var users = new List<string> { "Alice", "Bob" };

        var result = service.CreateMatch(users);

        Assert.NotNull(result);
        Assert.Contains(result.Value.Item1, users);
        Assert.Contains(result.Value.Item2, users);
        Assert.NotEqual(result.Value.Item1, result.Value.Item2);
    }
}