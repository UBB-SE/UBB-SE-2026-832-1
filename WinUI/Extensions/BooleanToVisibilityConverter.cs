using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace WinUI.Extensions;

public sealed class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isVisible = value switch
        {
            bool booleanValue => booleanValue,
            int integerValue => integerValue != 0,
            long longValue => longValue != 0,
            short shortValue => shortValue != 0,
            byte byteValue => byteValue != 0,
            _ => value is not null,
        };
        var shouldInvert = parameter is string text && string.Equals(text, "Invert", StringComparison.OrdinalIgnoreCase);

        if (shouldInvert)
        {
            isVisible = !isVisible;
        }

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        var isVisible = value is Visibility visibility && visibility == Visibility.Visible;
        var shouldInvert = parameter is string text && string.Equals(text, "Invert", StringComparison.OrdinalIgnoreCase);

        if (shouldInvert)
        {
            isVisible = !isVisible;
        }

        return isVisible;
    }
}