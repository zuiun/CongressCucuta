using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (string), typeof (string))]
[ValueConversion (typeof (Nullable), typeof (string))]
internal class DescriptionToTextDecorationsConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            string description => description.Length > 0 ? "Underline" : "None",
            null => "None",
            _ => throw new NotSupportedException (),
        };
    }
    
    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
