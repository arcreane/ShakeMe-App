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
}