using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CongressCucuta.Converters;

[ValueConversion (typeof (byte), typeof (string))]
internal class IndentLevelToFontSizeConverter : IValueConverter {
    private const byte MARGIN_MAX = 28;
    private const byte MARGIN_MIN = 16;
    private const byte MARGIN_PER_INDENT_LEVEL = 4;

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            byte indentLevel => $"{Math.Max (MARGIN_MAX - (indentLevel * MARGIN_PER_INDENT_LEVEL), MARGIN_MIN)}",
            _ => DependencyProperty.UnsetValue,
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
