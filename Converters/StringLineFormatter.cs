using System.Text;

namespace congress_cucuta.Converters;

internal class StringLineFormatter {
    private const char INDENT = '#';
    private const char DELIMITER = '.';

    public static string Indent (string line, byte indentLevel) {
        StringBuilder result = new ();

        for (byte i = 0; i < indentLevel; ++ i) {
            result.Append (INDENT);
        }

        result.Append (DELIMITER);
        result.Append (line);
        return result.ToString ();
    }

    public static (string, byte) Split (string text) {
        string[] parameters = text.Split (DELIMITER);
        string line;
        byte indentLevel;

        if (parameters.Length == 1) {
            // IndentLevel also controls font size
            indentLevel = 0;
            line = parameters[0];
        } else if (parameters.Length == 2) {
            indentLevel = (byte) parameters[0].Count (c => c == INDENT);
            line = parameters[1];
        } else {
            throw new ArgumentException ("Expected '#.Text' format", nameof (text));
        }

        return (line, indentLevel);
    }
}
