using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CongressCucuta.Converters;

[ValueConversion (typeof (string), typeof (string))]
[ValueConversion (typeof (Nullable), typeof (string))]
internal class DescriptionToTextDecorationsConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            string description => description.Length > 0 ? "Underline" : "None",
            null => "None",
            _ => DependencyProperty.UnsetValue,
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
