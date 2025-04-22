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
        var token =  SecureStorage.GetAsync("auth_token");
        Console.WriteLine("voici le token : " + token);

        if (isLoggedIn)
        {
            MainPage = new AppShell(); 
        }
        else
        {
            MainPage = Services.GetService<AppShell>();
            Shell.Current.GoToAsync("//WelcomePage"); 
        }

        Test();
    }

    private async void Test()
    {
        var client = new HttpClient();

        try
        {
            var response = await client.GetAsync("http://10.0.2.2:3000/api/users/me");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"📨 Réponse API : {content}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur de test API : {ex.Message}");
        }
    }
}