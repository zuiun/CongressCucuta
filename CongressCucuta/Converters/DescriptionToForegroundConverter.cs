using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CongressCucuta.Converters;

[ValueConversion (typeof (string), typeof (string))]
[ValueConversion (typeof (Nullable), typeof (string))]
internal class DescriptionToForegroundConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            string => "Blue",
            null => "Black",
            _ => DependencyProperty.UnsetValue,
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
