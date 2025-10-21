using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CongressCucuta.Converters;

[ValueConversion (typeof (int), typeof (string))]
internal class CountToVisibilityConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            int count => count > 0 ? "Visible" : "Collapsed",
            _ => DependencyProperty.UnsetValue,
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
