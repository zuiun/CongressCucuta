namespace congress_cucuta.Data;

internal class Currency (IDType id) : IID {
    public static readonly IDType STATE = byte.MaxValue;
    /*
     * The following reserved IDs exist for Target purposes and should not be used to create a Currency
     *
     * When creating Localisation, they should be used for common Currencies used by PARTY and/or REGION,
     * alongside the corresponding Currency for each Faction ID
     */
    public static readonly IDType PARTY = STATE - 1;
    public static readonly IDType REGION = PARTY - 1;

    public IDType ID => id;
}
