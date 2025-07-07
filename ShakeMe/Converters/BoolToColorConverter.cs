using System.Globalization;

namespace ShakeMe.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool hasError && hasError)
        {
            return Color.FromArgb("#E74C3C"); // Rouge si erreur
        }
        return Color.FromArgb("#E9ECEF"); // Gris normal
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}