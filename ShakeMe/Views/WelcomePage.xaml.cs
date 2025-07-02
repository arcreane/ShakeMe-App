using ShakeMe.ViewModels;
using ShakeMe.Views;

namespace ShakeMe.Views;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("match", typeof(MatchPage));
        Routing.RegisterRoute("profile", typeof(UserProfilePage));
        Routing.RegisterRoute("chat", typeof(ChatPage));
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("register");

    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("login");
    }
    private async void OnGuestModeClicked(object sender, EventArgs e)
    {
        Console.WriteLine("👤 Mode invité activé");

        await SecureStorage.SetAsync("guest_mode", "true");

        await Shell.Current.GoToAsync("match");
    }
}