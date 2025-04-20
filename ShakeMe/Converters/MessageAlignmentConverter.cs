using System.Globalization;
using Microsoft.Maui.Controls;

namespace ShakeMe.Converters;

public class MessageAlignmentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isMine)
            return isMine ? LayoutOptions.End : LayoutOptions.Start;

        return LayoutOptions.Start;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
