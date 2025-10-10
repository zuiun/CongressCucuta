using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (int), typeof (string))]
internal class CountToVisibilityConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            int count => count > 0 ? "Visible" : "Collapsed",
            _ => throw new NotSupportedException (),
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
