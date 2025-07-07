using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Dtos;
using ShakeMe.Core.Services;
using Microsoft.Maui.Storage;
using System.Text.Json;


namespace ShakeMe.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string identifier;
    [ObservableProperty] private string password;

    [ObservableProperty] private string identifierError;
    [ObservableProperty] private string passwordError;
    [ObservableProperty] private bool hasIdentifierError;
    [ObservableProperty] private bool hasPasswordError;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task Login()
    {
        Console.WriteLine("🔐 Tentative de login...");
        HasIdentifierError = false;
        HasPasswordError = false;

        if (string.IsNullOrWhiteSpace(Identifier))
        {
            IdentifierError = "Veuillez entrer votre pseudo ou email.";
            HasIdentifierError = true;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordError = "Veuillez entrer votre mot de passe.";
            HasPasswordError = true;
        }

        if (HasIdentifierError || HasPasswordError)
            return;

        try
        {
            var result = await _authService.LoginAsync(Identifier, Password);
            Console.WriteLine($"📦 Résultat brut : {JsonSerializer.Serialize(result)}");
            if (result?.User == null || string.IsNullOrEmpty(result.Token))
            {
                IdentifierError = "Identifiant ou mot de passe incorrect.";
                HasIdentifierError = true;
                HasPasswordError = true;
                return;
            }

            // Stockage sécurisé des données
            await SecureStorage.SetAsync("auth_token", result.Token);
            await SecureStorage.SetAsync("user_id", result.User.Id.ToString());
            await SecureStorage.SetAsync("user_pseudo", result.User.Pseudo);

            Console.WriteLine($"✅ Connexion réussie pour : {result.User.Pseudo}");

            // Navigation vers la page principale
            await Shell.Current.GoToAsync("//match");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors de la connexion : {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur", "Impossible de se connecter. Vérifiez votre connexion internet.", "OK");
        }
    }
    
    [RelayCommand]
    private async Task NavigateToRegister()
    {
        await Shell.Current.GoToAsync("//register"); // Adapte selon ta navigation
    }

    [RelayCommand]
    private async Task ForgotPassword()
    {
        await Shell.Current.DisplayAlert("Mot de passe oublié", 
            "Fonctionnalité en cours de développement", "OK");
        // Ou navigue vers une page de récupération de mot de passe
    }
}
