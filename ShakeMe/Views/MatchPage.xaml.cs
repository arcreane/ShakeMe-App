using ShakeMe.Services;
using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class MatchPage : ContentPage
{
    private readonly ShakeDetectorService _shakeDetectorService;
    private MatchPageViewModel ViewModel => (MatchPageViewModel)BindingContext;

    public MatchPage()
    {
        InitializeComponent();
        Console.WriteLine("🌀 MatchPage construite");

        BindingContext = App.Services.GetService<MatchPageViewModel>();
        _shakeDetectorService = App.Services.GetService<ShakeDetectorService>()!;
    }

    private async void OnShakeDetected()
    {
        Console.WriteLine("🌀 Shake détecté dans la vue");

        // Lancer l’animation
        await CircleEffect.AnimateExpansion();

        // Laisser le temps à l’animation de bien se lancer
        await Task.Delay(500);

        // Lancer le matchmaking (via ViewModel)
        await ViewModel.HandleMatchAsync();

        // Fin de l’animation
        await CircleEffect.AnimateCollapse();

        // Aller vers la conversation
        await Navigation.PushAsync(new ChatPage());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.StartShakeDetection();
        _shakeDetectorService.ShakeDetected += OnShakeDetected;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _shakeDetectorService.ShakeDetected -= OnShakeDetected;
    }
}