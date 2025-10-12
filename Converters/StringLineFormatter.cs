using System;
using System.Text;

namespace congress_cucuta.Converters;

internal class StringLineFormatter {
    private const char INDENT = '#';
    private const char DELIMITER = '|';
    private const string SPACE = "    ";

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
            throw new ArgumentException ($"Expected '{INDENT}{DELIMITER}Text' format", nameof (text));
        }

        return (line, indentLevel);
    }

    public static string Convert (string text) {
        string clean = text.Replace ($"{DELIMITER}", string.Empty);
        string[] lines = clean.Split ('\n');
        string[] reduced = [.. lines.Select (l => l[1 ..])];
        string joined = string.Join ('\n', reduced);
        string tabs = joined.Replace ($"{INDENT}", SPACE);

        return tabs;
    }

    public static string Outdent (string text) {
        string[] lines = text.Split ('\n');
        string[] trimmed = [.. lines.Select (l => l[1..])];
        string result = string.Join ('\n', trimmed);

        return result;
    }
}
