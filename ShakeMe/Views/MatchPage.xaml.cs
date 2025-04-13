using ShakeMe.ViewModels;

namespace ShakeMe.Views;

public partial class MatchPage : ContentPage
{
    public MatchPage()
    {
        InitializeComponent();
        Console.WriteLine("🌀 MatchPage construite");

        BindingContext = App.Services.GetService<MatchPageViewModel>();
    }
}