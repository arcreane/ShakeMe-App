using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel ?? throw new NullReferenceException("RegisterViewModel is null");
        Console.WriteLine("RegisterPage loaded");
    }
}
