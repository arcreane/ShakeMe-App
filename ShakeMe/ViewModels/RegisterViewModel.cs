        using System.Windows.Input;
        using CommunityToolkit.Mvvm.ComponentModel;
        using CommunityToolkit.Mvvm.Input;
        using ShakeMe.Core.Dtos;
        using ShakeMe.Core.Services;

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

        // Messages d'erreur associés à chaque champ
        [ObservableProperty] private string firstNameError;
        [ObservableProperty] private string lastNameError;
        [ObservableProperty] private string pseudoError;
        [ObservableProperty] private string emailError;
        [ObservableProperty] private string passwordError;
        [ObservableProperty] private string confirmPasswordError;
        [ObservableProperty] private string dateOfBirthError;

        private readonly IUserService _userService;

        public RegisterViewModel(IUserService userService)
        {
            Console.WriteLine($"🔧 RegisterViewModel: userService is null? {userService == null}");
            _userService = userService;
        }

        [RelayCommand]
        private async Task RegisterAsync()
        {
            ClearErrors();

            bool isValid = true;

            if (_userService == null)
            {
                Console.WriteLine("❌ _userService est NULL !");
                return;
            }


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

            var dto = new CreateUserDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Pseudo = Pseudo,
                Email = Email,
                DateOfBirth = DateOfBirth,
                IsAnonymous = false,
                AvatarUrl = ""
            };

            Console.WriteLine($"📤 DTO généré : {dto.FirstName}, {dto.LastName}, {dto.Pseudo}, {dto.Email}, {dto.DateOfBirth}, {dto.IsAnonymous}, {dto.AvatarUrl}");

            try
            {
                Console.WriteLine("📡 Avant appel CreateUser");
                await _userService.CreateUser(dto);

                Console.WriteLine($"🪟 Shell.Current null ? {Shell.Current == null}");

                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Succès", "Inscription réussie !", "OK");
                else
                    Console.WriteLine("⚠️ Shell.Current est null, pas d’alerte affichée.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception complète : {ex}");
            }

        }

        private void ClearErrors()
        {
            FirstNameError = LastNameError = PseudoError =
            EmailError = PasswordError = ConfirmPasswordError =
            DateOfBirthError = string.Empty;
        }
    }
