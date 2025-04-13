using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class RegisterViewModelTestPage : ContentPage
{
    public RegisterViewModelTestPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        Console.WriteLine(">>> TEST : Construction RegisterViewModel");
        BindingContext = viewModel;
    }
}