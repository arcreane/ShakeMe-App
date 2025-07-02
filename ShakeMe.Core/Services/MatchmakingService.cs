namespace ShakeMe.Core.Services;

public class MatchmakingService
{
    public (string, string)? CreateMatch(List<string> users)
    {
        if (users == null || users.Count < 2)
            return null;

        var random = new Random();
        int index1 = random.Next(users.Count);
        int index2;

        do
        {
            index2 = random.Next(users.Count);
        } while (index2 == index1);

        return (users[index1], users[index2]);
    }
}