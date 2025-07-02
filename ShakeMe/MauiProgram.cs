using Microsoft.Extensions.Logging;
using ShakeMe.Core.Http;
using ShakeMe.Core.Services;
using ShakeMe.Services;
using ShakeMe.ViewModels;
using ShakeMe.Views;
using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.Vibrate)]

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
        
        // Pages
        builder.Services.AddTransient<WelcomePage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<MatchPage>();
        builder.Services.AddTransient<ChatPage>();
        builder.Services.AddTransient<UserProfilePage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<LoginPage>();

        // ViewModels
        builder.Services.AddTransient<UserProfileViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<MatchPageViewModel>();
        builder.Services.AddTransient<ChatPageViewModel>();

        // Core services
        builder.Services.AddSingleton<IUserService, UserService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IApiClient, ApiClient>();
        builder.Services.AddSingleton<WebSocketService>();
        builder.Services.AddSingleton<ShakeDetectorService>();
        builder.Services.AddSingleton<MatchmakingService>();
        builder.Services.AddSingleton<AppShell>();


        #if DEBUG
            builder.Logging.AddDebug();
        #endif

        var app = builder.Build();

        var userService = app.Services.GetService<IUserService>();

        return app;
    }
}
