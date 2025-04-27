using ShakeMe.ViewModels;
using ShakeMe.Views;

namespace ShakeMe;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }
    public static string? PendingIceBreaker { get; set; }


    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;

        // Affiche d'abord une page neutre pendant le test
        MainPage = new LoadingPage();

        // Ensuite, vérifie l'authentification
        _ = InitAppAsync(); // fire & forget
    }

    private async Task InitAppAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");

            if (!string.IsNullOrEmpty(token))
            {
                Console.WriteLine("🔐 Utilisateur connecté.");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = Services.GetService<AppShell>();
                });
            }
            else
            {
                Console.WriteLine("🔓 Aucun token, affichage WelcomePage.");
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    MainPage = new NavigationPage(new WelcomePage());
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Erreur InitAppAsync : " + ex);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MainPage = new NavigationPage(new WelcomePage());
            });
        }
    }
}