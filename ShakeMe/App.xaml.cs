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

        // Toujours initialiser le Shell comme conteneur principal
        MainPage = Services.GetService<AppShell>();

        // Lancer la logique de redirection
        MainThread.BeginInvokeOnMainThread(async () => await InitAppAsync());
    }

    private async Task InitAppAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("🔓 Aucun token trouvé, redirection vers WelcomePage.");

                // Laisse le temps à Shell de s'initialiser
                await Task.Delay(200);

                await Shell.Current.GoToAsync("WelcomePage");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Erreur pendant InitAppAsync : " + ex.ToString());

            try
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible de charger l'application :\n" + ex.Message, "OK");
            }
            catch (Exception alertEx)
            {
                Console.WriteLine("⚠️ Affichage alert échoué : " + alertEx.ToString());
            }
        }
    }
}