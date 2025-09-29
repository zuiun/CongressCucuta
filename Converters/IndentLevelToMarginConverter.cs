using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (byte), typeof (string))]
internal class IndentLevelToMarginConverter : IValueConverter {
    private const byte MARGIN_PER_INDENT_LEVEL = 24;

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is byte indentLevel) {
            return $"{indentLevel * MARGIN_PER_INDENT_LEVEL}, 0, 0, 0";
        } else {
            throw new NotSupportedException ();
        }
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
