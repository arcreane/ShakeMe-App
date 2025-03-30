using ShakeMe.Core.Models;
using ShakeMe.Core.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShakeMe.ViewModels;

public class UserProfileViewModel : INotifyPropertyChanged
{
    private readonly IUserService _userService;

    public UserProfileViewModel(IUserService userService)
    {
        _userService = userService;

        // On simule un utilisateur existant dans le service
        var user = _userService.GetAllUsers().FirstOrDefault();

        if (user != null)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Pseudo = user.Pseudo;
            AvatarUrl = user.AvatarUrl;
        }
    }

    private string firstName;
    public string FirstName
    {
        get => firstName;
        set
        {
            firstName = value;
            OnPropertyChanged();
        }
    }

    private string lastName;
    public string LastName
    {
        get => lastName;
        set
        {
            lastName = value;
            OnPropertyChanged();
        }
    }

    private string email;
    public string Email
    {
        get => email;
        set
        {
            email = value;
            OnPropertyChanged();
        }
    }

    private string pseudo;
    public string Pseudo
    {
        get => pseudo;
        set
        {
            pseudo = value;
            OnPropertyChanged();
        }
    }

    private string avatarUrl;
    public string AvatarUrl
    {
        get => avatarUrl;
        set
        {
            avatarUrl = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}