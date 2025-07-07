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
    [ObservableProperty] private string firstName;
    [ObservableProperty] private string lastName;
    [ObservableProperty] private string pseudo;
    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string confirmPassword;
    [ObservableProperty] private DateTime dateOfBirth = DateTime.Today;

    // Messages d'erreur
    [ObservableProperty] private string firstNameError;
    [ObservableProperty] private string lastNameError;
    [ObservableProperty] private string pseudoError;
    [ObservableProperty] private string emailError;
    [ObservableProperty] private string passwordError;
    [ObservableProperty] private string confirmPasswordError;
    [ObservableProperty] private string dateOfBirthError;

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

        bool isValid = true;

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            FirstNameError = "Le prénom est requis.";
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            LastNameError = "Le nom est requis.";
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

        if (DateOfBirth > DateTime.Today)
        {
            DateOfBirthError = "Date invalide.";
            isValid = false;
        }

        if (!isValid) return;

        var dto = new RegisterDto
        {
            FirstName = FirstName,
            LastName = LastName,
            Pseudo = Pseudo,
            Email = Email,
            Password = Password,
            DateOfBirth = DateOfBirth
        };

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
        FirstNameError = LastNameError = PseudoError =
        EmailError = PasswordError = ConfirmPasswordError =
        DateOfBirthError = string.Empty;
    }
}