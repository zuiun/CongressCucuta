namespace congress_cucuta.Data;

internal struct Permissions (
    bool? canVote = true,
    byte? votes = 1,
    bool? canPass = false,
    bool? canVeto = false,
    bool? canSpeak = true
) {
    // These fields are used as backups
    //private readonly bool? _canVote = canVote;
    //private readonly byte? _votes = votes;
    //private readonly bool? _canPass = canPass;
    //private readonly bool? _canVeto = canVeto;
    //private readonly bool? _canSpeak = canSpeak;

    public bool? CanVote { get; set; } = canVote;
    public byte? Votes { get; set; } = votes;
    public bool? CanPass { get; set; } = canPass;
    public bool? CanVeto { get; set; } = canVeto;
    public bool? CanSpeak { get; set; } = canSpeak;

    // Compose left with changes from right
    public static Permissions operator + (Permissions left, Permissions right) {
        return new Permissions () {
            CanVote = right.CanVote ?? left.CanVote,
            Votes = (byte) (left.Votes! + (right.Votes ?? 0)),
            CanPass = right.CanPass ?? left.CanPass,
            CanVeto = right.CanVeto ?? left.CanVeto,
            CanSpeak = right.CanSpeak ?? left.CanSpeak,
        };
    }

    // Revert left where right has changes
    //public static Permissions operator - (Permissions left, Permissions right) {
    //    return new Permissions () {
    //        CanVote = right.CanVote is null ? left.CanVote : left._canVote,
    //        Votes = right.Votes is null ? left.Votes : left._votes,
    //        CanPass = right.CanPass is null ? left.CanPass : left._canPass,
    //        CanVeto = right.CanVeto is null ? left.CanVeto : left._canVeto,
    //        CanSpeak = right.CanSpeak is null ? left.CanSpeak : left._canSpeak,
    //    };
    //}
}
