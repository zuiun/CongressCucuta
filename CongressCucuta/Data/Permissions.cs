using CongressCucuta.Converters;

namespace CongressCucuta.Data;

internal readonly record struct Permissions (
    bool CanVote,
    byte Votes = 1,
    bool CanSpeak = true
) {
    internal readonly record struct Composition (
        bool? CanVote = null,
        byte? Votes = null,
        bool? CanSpeak = null
    );

    // Always prefer right
    public static Permissions operator + (Permissions left, Composition right) => new (
            right.CanVote ?? left.CanVote,
            (byte) (left.Votes + (right.Votes ?? 0)),
            right.CanSpeak ?? left.CanSpeak
        );

    public static Permissions operator + (Permissions left, Permissions right) => new (
            // Prefer restrictive
            left.CanVote && right.CanVote,
            // Prefer permissive
            byte.Max (left.Votes, right.Votes),
            // Prefer restrictive
            left.CanSpeak && right.CanSpeak
        );

    public override readonly string ToString () {
        List<string> result = [];

        if (CanVote) {
            result.Add ("Can vote");
            result.Add (StringLineFormatter.Indent ($"Has {Votes} vote(s)", 1));
        } else {
            result.Add ("Cannot vote");
        }

        if (! CanSpeak) {
            result.Add ("Cannot speak");
        }

        return string.Join ('\n', result);
    }
}
