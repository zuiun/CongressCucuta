using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (String), typeof (String))]
public class DescriptionToTextDecorationsConverter : IValueConverter {
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string description) {
            if (description.Length > 0) {
                return "Underline";
            } else {
                return "None";
            }
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
