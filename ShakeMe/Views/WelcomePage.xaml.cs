using ShakeMe.ViewModels;
using ShakeMe.Views;

namespace ShakeMe.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RegisterPage>();
        await Navigation.PushAsync(page);
    }


    private async void OnRegisterTestClicked(object sender, EventArgs e)
    {
        var page = App.Services.GetService<RegisterViewModelTestPage>();
        await Navigation.PushAsync(page);
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(App.Services.GetService<LoginPage>());
    }
    private async void OnGuestModeClicked(object sender, EventArgs e)
    {
        Console.WriteLine("👤 Mode invité activé");

        await SecureStorage.SetAsync("guest_mode", "true");

        // await Shell.Current.GoToAsync("//match");
        await Navigation.PushAsync(new MatchPage());
    }
}