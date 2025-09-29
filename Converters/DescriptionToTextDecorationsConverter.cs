using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (string), typeof (string))]
[ValueConversion (typeof (Nullable), typeof (string))]
internal class DescriptionToTextDecorationsConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string description) {
            return description.Length > 0 ? "Underline" : "None";
        } else if (value is null) {
            return "None";
        } else {
            throw new NotSupportedException ();
        }
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
