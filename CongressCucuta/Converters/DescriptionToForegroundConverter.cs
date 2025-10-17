using System.Globalization;
using System.Windows.Data;

namespace CongressCucuta.Converters;

[ValueConversion (typeof (string), typeof (string))]
[ValueConversion (typeof (Nullable), typeof (string))]
internal class DescriptionToForegroundConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            string => "Blue",
            null => "Black",
            _ => throw new NotSupportedException (),
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
