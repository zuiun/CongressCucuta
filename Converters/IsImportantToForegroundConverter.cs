using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (bool), typeof (string))]
internal class IsImportantToForegroundConverter : IValueConverter {
    public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool isImportant) {
            return isImportant ? "Red" : "Black";
        } else {
            throw new NotSupportedException ();
        }
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
