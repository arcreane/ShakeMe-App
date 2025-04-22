using ShakeMe.Core.Dtos;
using ShakeMe.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        Console.WriteLine("💾 Profil enregistré !");
        await Shell.Current.DisplayAlert("Succès", "Ton profil a bien été enregistré.", "OK");
    }
}