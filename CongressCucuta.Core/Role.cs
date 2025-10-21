namespace CongressCucuta.Core;

public static class Role {
    /*
     * MEMBER must always exist
     */
    // MEMBER must always exist
    public static readonly IDType MEMBER = byte.MaxValue;
    public static readonly IDType HEAD_GOVERNMENT = MEMBER - 1;
    public static readonly IDType HEAD_STATE = HEAD_GOVERNMENT - 1;
    /*
     * The following reserved IDs exist for Target purposes and should not be used to create a Role
     *
     * When creating Localisation and Permissions, these should be used to indicate the existence of such common Roles,
     * alongside the corresponding Role for each Faction ID
     */
    public static readonly IDType LEADER_PARTY = HEAD_STATE - 1;
    public static readonly IDType LEADER_REGION = LEADER_PARTY - 1;
    // The following reserved IDs are for roles outside of the Faction system
    public static readonly IDType RESERVED_3 = LEADER_REGION - 1;
    public static readonly IDType RESERVED_2 = RESERVED_3 - 1;
    public static readonly IDType RESERVED_1 = RESERVED_2 - 1;
}
