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
    
    [Fact]
    public void CreateMatch_WithEmptyList_ReturnsNull()
    {
        var service = new MatchmakingService();
        var users = new List<string>();

        var result = service.CreateMatch(users);

        Assert.Null(result);
    }
    
    [Fact]
    public void CreateMatch_WithSingleUser_ReturnsNull()
    {
        var service = new MatchmakingService();
        var users = new List<string> { "Alice" };

        var result = service.CreateMatch(users);

        Assert.Null(result);
    }
    
    [Fact]
    public void CreateMatch_MultipleCalls_ReturnsDifferentPairs()
    {
        var service = new MatchmakingService();
        var users = new List<string> { "Alice", "Bob", "Charlie", "Dave" };

        var pairs = new HashSet<(string, string)>();

        for (int i = 0; i < 10; i++)
        {
            var match = service.CreateMatch(users);
            Assert.NotNull(match);
            Assert.NotEqual(match.Value.Item1, match.Value.Item2);

            var orderedPair = (match.Value.Item1, match.Value.Item2);
            pairs.Add(orderedPair);
        }

        Assert.True(pairs.Count > 1, "Matches should not always be the same.");
    }

}