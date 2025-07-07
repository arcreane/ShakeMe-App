using System.Globalization;
using Microsoft.Maui.Controls;

namespace ShakeMe.Converters;

public class MessageColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isMine = value is true;
        Console.WriteLine($"🎯 MessageColorConverter : isMine = {isMine}");

        if (isMine)
        {
            // Messages de l'utilisateur - même couleur que le thème principal
            return Color.FromArgb("#667eea");
        }
        else
        {
            // Messages reçus - couleur complémentaire plus douce
            return Color.FromArgb("#764ba2");
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}