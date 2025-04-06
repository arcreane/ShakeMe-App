using System;
using System.Windows.Input;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Dtos;
using ShakeMe.Core.Services;

namespace ShakeMe.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty] private string firstName;
    [ObservableProperty] private string lastName;
    [ObservableProperty] private string pseudo;
    [ObservableProperty] private string email;
    [ObservableProperty] private string password;
    [ObservableProperty] private string confirmPassword;
    [ObservableProperty] private DateTime dateOfBirth = DateTime.Today.AddYears(-13); 

    [ObservableProperty] private string errorMessage;

    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    public async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        var dto = new RegisterDto
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            Pseudo = Pseudo,
            Password = Password,
            DateOfBirth = DateOfBirth
        };

        try
        {
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Les mots de passe ne correspondent pas.";
                return;
            }
            
            var user = await _authService.RegisterAsync(dto);

            await Shell.Current.DisplayAlert("Inscription", "Inscription réussie !", "OK");

        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}