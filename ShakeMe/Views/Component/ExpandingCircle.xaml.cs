using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace ShakeMe.Views.Component;

public partial class ExpandingCircle : ContentView
{
    public ExpandingCircle()
    {
        InitializeComponent();
    }

    public async Task AnimateExpansion()
    {
        IsVisible = true;
        BackgroundColor = Colors.Transparent;

        Circle.Opacity = 0.8;
        Circle.Scale = 1;
        Circle.WidthRequest = 100;
        Circle.HeightRequest = 100;
        Circle.CornerRadius = 50;

        await Task.Delay(10);

        double centerX = Width / 2 - 50;
        double centerY = Height / 2 - 50;
        AbsoluteLayout.SetLayoutBounds(Circle, new Rect(centerX, centerY, 100, 100));

        double finalSize = Math.Max(Width, Height) * 2;

        await Circle.ScaleTo(finalSize / 100, 800, Easing.CubicInOut);

        Circle.WidthRequest = finalSize;
        Circle.HeightRequest = finalSize;
        Circle.CornerRadius = (float)(finalSize / 2);

        BackgroundColor = Colors.DeepSkyBlue.WithAlpha(0.8f);
    }

    public async Task AnimateCollapse()
    {
        await Circle.ScaleTo(1, 500, Easing.CubicInOut);

        Circle.Opacity = 0.8;
        Circle.Scale = 1;
        Circle.WidthRequest = 100;
        Circle.HeightRequest = 100;
        Circle.CornerRadius = 50;

        IsVisible = false;
        BackgroundColor = Colors.Transparent;
    }
}