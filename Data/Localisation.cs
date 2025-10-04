namespace congress_cucuta.Data;

internal readonly struct Localisation (
    string state,
    string government,
    string[] context,
    string date,
    string situation,
    string period,
    (string, string) member,
    string speaker,
    (string, string) region,
    (string, string) party
) {
    public const string UNUSED = "UNUSED";

    public string State => state;
    public string Government => government;
    public string[] Context => context;
    public string Date => date;
    public string Situation => situation;
    public string Period => period;
    public string MemberSingular => member.Item1;
    public string MemberPlural => member.Item2;
    public string Speaker => speaker;
    public string RegionSingular => region.Item1;
    public string RegionPlural => region.Item2;
    public string PartySingular => party.Item1;
    public string PartyPlural => party.Item2;
}
