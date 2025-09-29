using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (Byte), typeof (String))]
public class IndentLevelToFontSizeConverter : IValueConverter {
    const byte MARGIN_MAX = 28;
    const byte MARGIN_MIN = 16;
    const byte MARGIN_PER_INDENT_LEVEL = 4;

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is byte indentLevel) {
            return $"{Math.Max (MARGIN_MAX - (indentLevel * MARGIN_PER_INDENT_LEVEL), MARGIN_MIN)}";
        } else {
            throw new NotSupportedException ();
        }
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
