using Microsoft.Extensions.Logging;
using ShakeMe.Core.Services;
using ShakeMe.ViewModels;
using ShakeMe.Views;

namespace ShakeMe;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp(sp => new App(sp))
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        
        builder.Services.AddTransient<WelcomePage>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<MatchPage>();
        builder.Services.AddTransient<ChatPage>();

        builder.Services.AddSingleton<IUserService, UserService>();

        builder.Services.AddTransient<UserProfilePage>();
        builder.Services.AddTransient<UserProfileViewModel>();
        builder.Services.AddTransient<MainPage>();

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddSingleton<AppShell>();


        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();
        
        builder.Services.AddSingleton<ShakeDetectorService>();


        #if DEBUG
            builder.Logging.AddDebug();
        #endif

        var app = builder.Build();

        var userService = app.Services.GetService<IUserService>();

        userService?.CreateUser(new ShakeMe.Core.Dtos.CreateUserDto
        {
            FirstName = "Killian",
            LastName = "Carvalho",
            Email = "killian@example.com",
            DateOfBirth = new DateTime(1997, 1, 19),
            IsAnonymous = false,
            Pseudo = "kcdev",
            AvatarUrl = ""
        });

        return app;
    }
}
