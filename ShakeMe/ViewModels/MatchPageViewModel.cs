using CommunityToolkit.Mvvm.ComponentModel;
using ShakeMe.Core.Services;
using ShakeMe.Services;

namespace ShakeMe.ViewModels;

public partial class MatchPageViewModel : ObservableObject
{
    private readonly ShakeDetectorService _shakeService;
    private readonly MatchmakingService _matchmakingService;
    private readonly IUserService _userService;
    private bool _isMatching = false;

    public MatchPageViewModel(
        ShakeDetectorService shakeService,
        MatchmakingService matchmakingService,
        IUserService userService)
    {
        Console.WriteLine("✅ MatchPageViewModel instancié");
        _shakeService = shakeService;
        _matchmakingService = matchmakingService;
        _userService = userService;
    }

    public void StartShakeDetection()
    {
        if (!_shakeService.IsRunning)
        {
            _shakeService.Start();
        }
    }

    public async Task HandleMatchAsync()
    {
        if (_isMatching)
        {
            Console.WriteLine("⏳ Matchmaking en cours, shake ignoré");
            return;
        }

        _isMatching = true;
        Console.WriteLine("🎯 Début du matchmaking...");

        try
        {
            /*var users = _userService.GetAllUsers()
                .Select(u => u.Pseudo)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();*/

            // var match = _matchmakingService.CreateMatch(users);
            var match = "ok";

            if (match == null)
            {
                await Shell.Current.DisplayAlert("Matchmaking", "Pas assez d'utilisateurs pour matcher 😢", "OK");
            }
            else
            {
                var user1 = "user 1";
                var user2 = "user 2";
                await Shell.Current.DisplayAlert("Nouveau match 🎉", $"{user1} & {user2} ont été matchés !", "OK");
            }
        }
        finally
        {
            await Task.Delay(3000); // délai pour éviter spam shake
            _isMatching = false;
        }
    }
}