using CommunityToolkit.Mvvm.ComponentModel;
using ShakeMe.Core.Services;

namespace ShakeMe.ViewModels;

public partial class MatchPageViewModel : ObservableObject
{
    private readonly ShakeDetectorService _shakeService;
    private readonly MatchmakingService _matchmakingService;
    private readonly IUserService _userService;

    public MatchPageViewModel(
        ShakeDetectorService shakeService,
        MatchmakingService matchmakingService,
        IUserService userService)
    {
        Console.WriteLine("✅ Constructeur appelé");
        _shakeService = shakeService;
        _matchmakingService = matchmakingService;
        _userService = userService;

        _shakeService.ShakeDetected += OnShake;
        _shakeService.Start();
    }
    private async void OnShake()
    {
        Console.WriteLine("🎯 Shake détecté");

        var users = _userService.GetAllUsers()
            .Select(u => u.Pseudo)
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .ToList();

        var match = _matchmakingService.CreateMatch(users);

        if (match == null)
        {
            await Shell.Current.DisplayAlert("Matchmaking", "Pas assez d'utilisateurs pour matcher 😢", "OK");
            return;
        }

        var (user1, user2) = match.Value;

        await Shell.Current.DisplayAlert("Nouveau match 🎉",
            $"{user1} & {user2} ont été matchés !", "OK");
    }

}
