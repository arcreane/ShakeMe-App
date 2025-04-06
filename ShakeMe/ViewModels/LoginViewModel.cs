using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Dtos;
using ShakeMe.Core.Services;

namespace ShakeMe.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string identifier;
    [ObservableProperty] private string password;
    [ObservableProperty] private string errorMessage;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        var dto = new LoginDto
        {
            Identifier = Identifier,
            Password = Password
        };

        try
        {
            var user = await _authService.LoginAsync(dto);

            await Shell.Current.DisplayAlert("Connexion", "Connexion réussie !", "OK");

            // Redirection vers la page principale avec menu
            // await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}