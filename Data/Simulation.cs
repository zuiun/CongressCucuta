namespace congress_cucuta.Data;

internal class Simulation {
    public History History { get; }
    public Dictionary<Role, Permissions> RolesPermissions { get; }
    public List<Procedure> ProceduresGovernmental { get; }
    public List<Procedure> ProceduresSpecial { get; }
    public List<Procedure> ProceduresDeclared { get; }
    public string RegionNameSingular { get; }
    public string RegionNamePlural { get; }
    public List<Region> Regions { get; }
    public string PartyNameSingular { get; }
    public string PartyNamePlural { get; }
    public List<Party> Parties { get; }
    public List<Ballot> Ballots { get; }

    public Simulation (
        History history,
        Dictionary<Role, Permissions> rolesPermissions,
        List<Procedure> proceduresGovernmental,
        List<Procedure> proceduresSpecial,
        List<Procedure> proceduresDeclared,
        string regionNameSingular,
        string regionNamePlural,
        List<Region> regions,
        string partyNameSingular,
        string partyNamePlural,
        List<Party> parties,
        List<Ballot> ballots
    ) {
        History = history;
        RolesPermissions = rolesPermissions;
        ProceduresGovernmental = proceduresGovernmental;
        ProceduresSpecial = proceduresSpecial;
        ProceduresDeclared = proceduresDeclared;
        RegionNameSingular = regionNameSingular;
        RegionNamePlural = regionNamePlural;
        Regions = regions;
        PartyNameSingular = partyNameSingular;
        PartyNamePlural = partyNamePlural;
        Parties = parties;
        Ballots = ballots;

        Validate ();
    }

    /*
     * There must be a MEMBER Role
     * Factions cannot share an ID with a reserved Role ID
     * Role IDs must correspond one-to-one with Faction IDs (signifies Faction leadership), excepting the reserved Roles
     * Either MEMBER or HEAD_GOVERNMENT must be able to vote
     * Region IDs and Party IDs cannot overlap
     * TODO: insane Procedures moment
     */
    private void Validate () {
        // TODO: Validate MUST throw an exception if invariants are broken. There is NO RECOVERING
        if (RolesPermissions.Keys.All (r => r.ID != Role.MEMBER)) {
            throw new ArgumentException ("There must be a MEMBER Role");
        }
        
        if (
            Regions.Any (r => r.ID == Role.MEMBER || r.ID == Role.HEAD_GOVERNMENT || r.ID == Role.HEAD_STATE)
            || Parties.Any (p => p.ID == Role.MEMBER || p.ID == Role.HEAD_GOVERNMENT || p.ID == Role.HEAD_STATE)
        ) {
            throw new ArgumentException ("Factions cannot share an ID with a reserved Role ID");
        }

        if (
            RolesPermissions.Keys.Any (
                ro =>
                    ro.ID != Role.MEMBER
                    && ro.ID != Role.HEAD_GOVERNMENT
                    && ro.ID != Role.HEAD_STATE
                    && Regions.All (re => ro.ID != re.ID)
                    && Parties.All (p => ro.ID != p.ID)
            )
        ) {
            throw new ArgumentException ("Role IDs must correspond one-to-one with Faction IDs (signifies Faction leadership), excepting the reserved Roles");
        }

        if (
            RolesPermissions.Where (k => k.Key.ID == Role.MEMBER).All (k => k.Value.CanVote is false)
            && RolesPermissions.Where (k => k.Key.ID == Role.HEAD_GOVERNMENT).All (k => k.Value.CanVote is false)
        ) {
            throw new ArgumentException ("Either MEMBER or HEAD_GOVERNMENT must be able to vote");
        }

        if (Regions.Any (r => Parties.Any (p => r.ID == p.ID))) {
            throw new ArgumentException ("Region IDs and Party IDs cannot overlap");
        }

        // TODO: Horrible Procedure rules :sob:
    }
}
