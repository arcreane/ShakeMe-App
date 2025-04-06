using ShakeMe.Views;

namespace ShakeMe;

public partial class MainPage : ContentPage
{
    private readonly UserProfilePage _profilePage;

    public MainPage(UserProfilePage profilePage)
    {
        InitializeComponent();
        _profilePage = profilePage;
    }

    private async void OnViewProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(_profilePage);
    }
}
