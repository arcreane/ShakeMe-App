using System.Globalization;
using Microsoft.Maui.Controls;

namespace ShakeMe.Converters;

public class MessageColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isMine = value is true;
        Console.WriteLine($"🎯 MessageColorConverter : isMine = {isMine}");

        var key = isMine ? "Primary" : "Gray400";

        if (Application.Current.Resources.TryGetValue(key, out var raw) && raw is Color color)
        {
            return color;
        }

        return Colors.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}