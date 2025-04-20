using ShakeMe.ViewModels;
using ShakeMe.Views;

namespace ShakeMe;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;

        var isLoggedIn = Preferences.ContainsKey("user_id");

        if (isLoggedIn)
        {
            MainPage = new AppShell(); 
        }
        else
        {
            MainPage = Services.GetService<AppShell>();
            Shell.Current.GoToAsync("//WelcomePage"); 
        }
    }
}