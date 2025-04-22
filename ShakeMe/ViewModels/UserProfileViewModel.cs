using ShakeMe.Core.Dtos;
using ShakeMe.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Views;

namespace ShakeMe.ViewModels;

public partial class UserProfileViewModel : ObservableObject
{
    private readonly IUserService _userService;

    [ObservableProperty]
    private UserDto user;

    public UserProfileViewModel(IUserService userService)
    {
        _userService = userService;
        LoadUser();
    }

    private async void LoadUser()
    {
        User = await _userService.GetProfileAsync();
        Console.WriteLine($"👤 Utilisateur chargé : {User?.Pseudo}");
    }

    [RelayCommand]
    private async Task Save()
    {
        try
        {
            var updated = await _userService.UpdateProfileAsync(User);

            if (updated != null)
            {
                User = updated;
                await Shell.Current.DisplayAlert("Succès", "Ton profil a bien été mis à jour.", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Erreur", "Impossible de mettre à jour le profil.", "OK");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur UpdateProfile : {ex.Message}");
            await Shell.Current.DisplayAlert("Erreur", "Une erreur est survenue.", "OK");
        }
    }
    
    [RelayCommand]
    private async Task Logout()
    {
        try
        {
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_pseudo");

            Console.WriteLine("🔓 Déconnecté, redirection vers WelcomePage...");

            Application.Current.MainPage = new NavigationPage(new WelcomePage());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur Logout : {ex}");
            await Shell.Current.DisplayAlert("Erreur", "Impossible de se déconnecter.", "OK");
        }
    }

}