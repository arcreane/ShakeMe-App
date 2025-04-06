using ShakeMe.Core.Models;
using ShakeMe.Core.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ShakeMe.Core.Dtos;

namespace ShakeMe.ViewModels;

public class UserProfileViewModel : INotifyPropertyChanged
{
    private readonly IUserService _userService;
    public ICommand SaveCommand { get; }
    public ICommand PickImageCommand { get; }
    
    public UserProfileViewModel(IUserService userService)
    {
        _userService = userService;

        var user = _userService.GetAllUsers().FirstOrDefault();

        if (user != null)
        {
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Pseudo = user.Pseudo;
        }
        
        SaveCommand = new Command(SaveProfile);
        PickImageCommand = new Command(async () => await PickImage());
        
    }
    
    private async Task PickImage()
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Choisissez un avatar",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                AvatarUrl = result.FullPath;
            }
        }
        catch (Exception ex)
        {
            // Optionnel : gérer l'erreur
            Console.WriteLine($"Erreur lors de la sélection de l'image : {ex.Message}");
        }
    }
    
    private Guid currentUserId;

    private void SaveProfile()
    {
        _userService.UpdateUser(currentUserId, new UpdateUserDto
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            DateOfBirth = DateTime.Now,
            IsAnonymous = false,
            Pseudo = Pseudo,
            AvatarUrl = AvatarUrl
        });
        
        Application.Current.MainPage.DisplayAlert("Succès", "Profil mis à jour avec succès.", "OK");
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
            OnPropertyChanged(nameof(AvatarImageSource)); // 🔥 pour que l'image se mette à jour
        }
    }
    
    public ImageSource AvatarImageSource
    {
        get
        {
            if (string.IsNullOrWhiteSpace(AvatarUrl))
            {
                return ImageSource.FromUri(new Uri("https://media.istockphoto.com/id/1189860501/fr/photo/voyageur-avec-un-sac-%C3%A0-dos-restant-sur-un-sommet-de-montagne-au-dessus-des-nuages.jpg?s=612x612&w=is&k=20&c=V41Pd6Hyg8pk8MKRl2qFix8QDqqN73cfkf_FaMOl1PE="));
            }

            // Si c'est une URL (commence par http ou https), on utilise FromUri
            if (Uri.IsWellFormedUriString(AvatarUrl, UriKind.Absolute))
            {
                return ImageSource.FromUri(new Uri(AvatarUrl));
            }

            // Sinon on suppose que c'est un chemin local (ex : après FilePicker)
            return ImageSource.FromFile(AvatarUrl);
        }
    }



    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}