using System;
using Microsoft.UI.Xaml.Data;

namespace WinUI.Converters;

public sealed class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => value is string stringValue && !string.IsNullOrEmpty(stringValue);

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotSupportedException();
}
