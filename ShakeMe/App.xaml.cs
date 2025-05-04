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

            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Utilise TOUJOURS AppShell
                var shell = Services.GetService<AppShell>();
                Application.Current.MainPage = shell;

                if (string.IsNullOrEmpty(token))
                {
                    // Et redirige vers WelcomePage manuellement
                    shell.GoToAsync("//welcome");
                }
                else
                {
                    Console.WriteLine("🔐 Utilisateur connecté.");
                    shell.GoToAsync("//match");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Erreur InitAppAsync : " + ex);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var shell = Services.GetService<AppShell>();
                Application.Current.MainPage = shell;
                shell.GoToAsync("//welcome");
            });
        }
    }

}