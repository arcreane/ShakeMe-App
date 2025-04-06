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
        try
        {
            var page = App.Services.GetService<RegisterPage>();
            if (page is null)
                throw new Exception("RegisterPage non trouvée dans le container");

            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur navigation RegisterPage : {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur", ex.Message, "OK");
        }
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(App.Services.GetService<LoginPage>());
    }
}