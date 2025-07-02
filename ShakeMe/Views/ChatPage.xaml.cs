using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class ChatPage : ContentPage
{
    private ChatPageViewModel ViewModel => (ChatPageViewModel)BindingContext;
    public ChatPage()
    {
        InitializeComponent();
        Console.WriteLine("✅ ChatPage construite");

        // Injection manuelle du ViewModel
        BindingContext = App.Services.GetService<ChatPageViewModel>();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("📄 ChatPage apparaît");

        if (!string.IsNullOrEmpty(App.PendingIceBreaker))
        {
            Console.WriteLine("🧹 Reset conversation car nouveau match détecté");
            ViewModel.ResetConversation();
        }
    }
}