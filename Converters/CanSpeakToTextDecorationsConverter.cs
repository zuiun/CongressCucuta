using System.Globalization;
using System.Windows.Data;

namespace congress_cucuta.Converters;

[ValueConversion (typeof (bool), typeof (string))]
internal class CanSpeakToTextDecorationsConverter : IValueConverter {
    public object Convert (object? value, Type targetType, object parameter, CultureInfo culture) {
        return value switch {
            bool canSpeak => canSpeak ? "None" : "Strikethrough",
            _ => throw new NotSupportedException (),
        };
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotSupportedException ();
    }
}
