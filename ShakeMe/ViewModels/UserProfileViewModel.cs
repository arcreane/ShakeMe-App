using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShakeMe.ViewModels;

public partial class UserProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private string pseudo;

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string avatarUrl;

    public UserProfileViewModel()
    {
        // Simule des données utilisateur à afficher
        Pseudo = "john_doe_1234";
        Email = "john@example.com";
        AvatarUrl = "https://example.com/avatar.png";
    }

    [RelayCommand]
    private async Task UpdateProfile()
    {
        await Application.Current.MainPage.DisplayAlert("Succès", "Profil mis à jour !", "OK");
    }
}