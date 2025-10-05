namespace congress_cucuta.Data;

internal static class Role {
    /*
     * MEMBER must always exist
     */
    public static readonly IDType MEMBER = byte.MaxValue;
    public static readonly IDType HEAD_GOVERNMENT = MEMBER - 1;
    public static readonly IDType HEAD_STATE = HEAD_GOVERNMENT - 1;
    /*
     * The following reserved IDs exist for Target purposes and should not be used to create a Role
     *
     * When creating Localisation and Permissions, these should be used for common Roles used by PARTY and/or REGION,
     * alongside the corresponding Role for each Faction ID
     *
     * If there are no common Localisations between Roles,
     * then these should not be defined in Localisation or Permissions at all
     */
    public static readonly IDType LEADER_PARTY = HEAD_STATE - 1;
    public static readonly IDType LEADER_REGION = LEADER_PARTY - 1;
}
