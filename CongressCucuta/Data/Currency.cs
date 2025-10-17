namespace CongressCucuta.Data;

internal static class Currency {
    public static readonly IDType STATE = byte.MaxValue;
    /*
     * The following reserved IDs exist for Target purposes and should not be used to create a Currency
     *
     * When creating Localisation, these should be used to indicate the existence of such common Currencies,
     * alongside the corresponding Currency for each Faction ID
     */
    public static readonly IDType PARTY = STATE - 1;
    public static readonly IDType REGION = PARTY - 1;
}
