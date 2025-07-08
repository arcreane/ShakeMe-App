using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Dtos;
using ShakeMe.Core.Services;
using Microsoft.Maui.Storage;

namespace ShakeMe.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    // Champs utilisateur
    [ObservableProperty] private string first_name;
    [ObservableProperty] private string last_name;
    [ObservableProperty] private string pseudo;
    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string confirmPassword;
    [ObservableProperty] private DateOnly date_of_birth = DateOnly.FromDateTime(DateTime.Today);

    // Messages d'erreur
    [ObservableProperty] private string first_name_error;
    [ObservableProperty] private string last_name_error;
    [ObservableProperty] private string pseudoError;
    [ObservableProperty] private string emailError;
    [ObservableProperty] private string passwordError;
    [ObservableProperty] private string confirmPasswordError;
    [ObservableProperty] private string date_of_birth_error;

    private readonly IAuthService _authService;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Console.WriteLine("✅ Constructeur RegisterViewModel appelé");
        Console.WriteLine($"➡️ _authService est null ? {_authService == null}");
    }

    [RelayCommand]
    private async Task Register()
    {
        ClearErrors();
        Console.WriteLine("📋 Validation des champs...");
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(first_name))
        {
            first_name_error = "Le prénom est requis.";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(last_name))
        {
            last_name_error = "Le nom est requis.";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Pseudo))
        {
            PseudoError = "Le pseudo est requis.";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
        {
            EmailError = "Email invalide.";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordError = "Mot de passe requis.";
            isValid = false;
        }

        if (Password != ConfirmPassword)
        {
            ConfirmPasswordError = "Les mots de passe ne correspondent pas.";
            isValid = false;
        }

        if (date_of_birth >= DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
        {
            date_of_birth_error = "Date invalide.";
            isValid = false;
        }
        Console.WriteLine($"➡️ first_name: {first_name}");
        Console.WriteLine($"➡️ last_name: {last_name}");
        Console.WriteLine($"➡️ Pseudo: {Pseudo}");
        Console.WriteLine($"➡️ Email: {Email}");
        Console.WriteLine($"➡️ Password: {Password}");
        Console.WriteLine($"➡️ ConfirmPassword: {ConfirmPassword}");
        Console.WriteLine($"➡️ date_of_birth: {date_of_birth}");

        if (!isValid)
        {
            Console.WriteLine("❌ Formulaire invalide, exit Register() !");
            return;
        }
        
        var dto = new RegisterDto
        {
            first_name = first_name,
            last_name = last_name,
            Pseudo = Pseudo,
            Email = Email,
            Password = Password,
            date_of_birth = date_of_birth
        };
        
        Console.WriteLine("📋 Champs validés, envoi inscription...");

        try
        {
            Console.WriteLine("📡 Envoi inscription au backend...");
            var result = await _authService.RegisterAsync(dto);
            Console.WriteLine($"📩 Réponse REGISTER : {System.Text.Json.JsonSerializer.Serialize(result)}");
            Console.WriteLine($"🧪 Token: {result.Token}");
            Console.WriteLine($"🧪 User: {result.User?.Pseudo}");
            if (result != null && result.Token != null && result.User != null)
            {
                try
                {
                    await SecureStorage.SetAsync("auth_token", result.Token);
                    Console.WriteLine("✅ Token enregistré");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur Token : {ex.Message}");
                }

                try
                {
                    await SecureStorage.SetAsync("user_id", result.User.Id.ToString());
                    Console.WriteLine("✅ ID enregistré");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur UserID : {ex.Message}");
                }

                try
                {
                    await SecureStorage.SetAsync("user_pseudo", result.User.Pseudo);
                    Console.WriteLine("✅ Pseudo enregistré");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Erreur Pseudo : {ex.Message}");
                }

                await Shell.Current.DisplayAlert("Succès", "Inscription réussie !", "OK");
                await Shell.Current.GoToAsync("//match");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Échec de l'inscription.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception Register : {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur", "Une erreur est survenue.", "OK");
        }
    }

    [RelayCommand]
    private async Task NavigateToLogin()
    {
        await Shell.Current.GoToAsync("login");
    }


    private void ClearErrors()
    {
        first_name_error = last_name_error = PseudoError =
        EmailError = PasswordError = ConfirmPasswordError =
        date_of_birth_error = string.Empty;
    }
}