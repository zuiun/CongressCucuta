using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CongressCucuta.Converters;

[ValueConversion (typeof (byte), typeof (string))]
internal class IndentLevelToMarginConverter : IValueConverter {
    private const byte MARGIN_PER_INDENT_LEVEL = 24;

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            byte indentLevel => $"{indentLevel * MARGIN_PER_INDENT_LEVEL}, 0, 0, 0",
            _ => DependencyProperty.UnsetValue,
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
