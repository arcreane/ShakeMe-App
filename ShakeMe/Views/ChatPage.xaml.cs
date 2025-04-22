using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class ChatPage : ContentPage
{
    public ChatPage()
    {
        InitializeComponent();
        Console.WriteLine("✅ ChatPage construite");

        // Injection manuelle du ViewModel
        BindingContext = App.Services.GetService<ChatPageViewModel>();
    }
}