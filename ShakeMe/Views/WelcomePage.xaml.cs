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
        await Navigation.PushAsync(App.Services.GetService<RegisterPage>());
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(App.Services.GetService<LoginPage>());
    }
}