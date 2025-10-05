using congress_cucuta.Converters;

namespace congress_cucuta.Data;

internal struct Permissions (
    bool canVote,
    byte votes = 1,
    bool canPass = false,
    bool canVeto = false,
    bool canSpeak = true
) {
    internal readonly record struct Composition (
        bool? CanVote = null,
        byte? Votes = null,
        bool? CanPass = null,
        bool? CanVeto = null,
        bool? CanSpeak = null
    );

    public bool CanVote { get; set; } = canVote;
    public byte Votes { get; set; } = votes;
    public bool CanPass { get; set; } = canPass;
    public bool CanVeto { get; set; } = canVeto;
    // Currently not used
    public bool CanSpeak { get; set; } = canSpeak;

    // Compose left with changes from right
    public static Permissions operator + (Permissions left, Composition right) {
        return new (
            right.CanVote ?? left.CanVote,
            (byte) (left.Votes + (right.Votes ?? 0)),
            right.CanPass ?? left.CanPass,
            right.CanVeto ?? left.CanVeto,
            right.CanSpeak ?? left.CanSpeak
        );
    }

    public override readonly string ToString () {
        List<string> result = [CanVote ? $"Can Vote" : "Cannot Vote"];

        if (CanVote) {
            result.Add (StringLineFormatter.Indent ($"Has {Votes} Vote(s)", 1));
        }

        if (CanPass) {
            result.Add ("Can Forcibly Pass Ballots");
        }

        if (CanVeto) {
            result.Add ("Can Veto Ballots");
        }

        return string.Join ('\n', result);
    }
}
