namespace congress_cucuta.Data;

internal struct Permissions (
    bool canVote = true,
    byte votes = 1,
    bool canPass = false,
    bool canVeto = false,
    bool canSpeak = true
) {
    internal struct Composition (
    bool? canVote = null,
    byte? votes = null,
    bool? canPass = null,
    bool? canVeto = null,
    bool? canSpeak = null
) {
        public bool? CanVote { get; } = canVote;
        public byte? Votes { get; } = votes;
        public bool? CanPass { get; } = canPass;
        public bool? CanVeto { get; } = canVeto;
        public bool? CanSpeak { get; } = canSpeak;
    }
    
    public bool CanVote { get; set; } = canVote;
    public byte Votes { get; set; } = votes;
    public bool CanPass { get; set; } = canPass;
    public bool CanVeto { get; set; } = canVeto;
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
}
