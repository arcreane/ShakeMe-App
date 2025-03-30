namespace ShakeMe;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; }

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;
        
        MainPage = new NavigationPage(Services.GetService<MainPage>());
    }
}
