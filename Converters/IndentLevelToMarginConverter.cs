using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (Byte), typeof (String))]
public class IndentLevelToMarginConverter : IValueConverter {
    const uint MARGIN_PER_INDENT_LEVEL = 24;

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is byte indentLevel) {
            return $"{indentLevel * MARGIN_PER_INDENT_LEVEL}, 0, 0, 0";
        } else {
            return "0";
        }
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
