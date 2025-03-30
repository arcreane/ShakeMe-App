using ShakeMe.Views;

namespace ShakeMe;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnViewProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserProfilePage());
    }

}