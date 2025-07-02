using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class ChatPage : ContentPage
{
    private ChatPageViewModel ViewModel => (ChatPageViewModel)BindingContext;

    public ChatPage(ChatPageViewModel vm)
    {
        InitializeComponent();
        Console.WriteLine("✅ ChatPage construite");

        BindingContext = vm;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Console.WriteLine("📄 ChatPage apparaît");
        ViewModel.OnPageAppearing();
    }

}