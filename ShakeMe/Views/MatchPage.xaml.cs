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

        await CircleEffect.AnimateExpansion();
        await Task.Delay(500);
        await ViewModel.HandleMatchAsync();
        WaitingLabel.IsVisible = true;
        await CircleEffect.AnimateCollapse();

    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MatchPageViewModel viewModel)
        {
            await viewModel.HandleReentryAsync();
        }
        _shakeDetectorService.ShakeDetected -= OnShakeDetected;
        _shakeDetectorService.ShakeDetected += OnShakeDetected;
        ViewModel.StartShakeDetection();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _shakeDetectorService.ShakeDetected -= OnShakeDetected;
    }
}