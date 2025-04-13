using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShakeMe.Core.Services;

namespace ShakeMe.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string identifier;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string identifierError;

    [ObservableProperty]
    private string passwordError;

    [ObservableProperty]
    private bool hasIdentifierError;

    [ObservableProperty]
    private bool hasPasswordError;

    public LoginViewModel(IUserService userService)
    {
        _userService = userService;
        LoginCommand = new AsyncRelayCommand(OnLoginAsync);
    }

    public ICommand LoginCommand { get; }
    private readonly IUserService _userService;

    private async Task OnLoginAsync()
    {
        // Reset erreurs
        HasIdentifierError = false;
        HasPasswordError = false;

        bool hasError = false;

        if (string.IsNullOrWhiteSpace(Identifier))
        {
            IdentifierError = "Veuillez entrer votre pseudo ou email.";
            HasIdentifierError = true;
            hasError = true;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            PasswordError = "Veuillez entrer votre mot de passe.";
            HasPasswordError = true;
            hasError = true;
        }

        if (hasError)
            return;

        var user = await _userService.AuthenticateUserAsync(Identifier, Password);

        if (user == null)
        {
            IdentifierError = "Identifiant ou mot de passe incorrect.";
            HasIdentifierError = true;
            HasPasswordError = true;
            return;
        }

        Console.WriteLine($"✅ Connexion réussie pour l'utilisateur : {user.Pseudo}");

        // TODO : Naviguer vers la page principale ou stocker l'utilisateur en session
    }

}