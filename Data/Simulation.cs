namespace congress_cucuta.Data;

internal class Simulation {
    public History History { get; }
    public Dictionary<Role, Permissions> RolesPermissions { get; }
    public Dictionary<Currency, sbyte> Currencies { get; }
    public List<ProcedureImmediate> ProceduresGovernmental { get; }
    public List<ProcedureTargeted> ProceduresSpecial { get; }
    public List<ProcedureDeclared> ProceduresDeclared { get; }
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
        Dictionary<Currency, sbyte> currencies,
        List<ProcedureImmediate> proceduresGovernmental,
        List<ProcedureTargeted> proceduresSpecial,
        List<ProcedureDeclared> proceduresDeclared,
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
        Currencies = currencies;
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
     * (1) There must be a MEMBER Role
     * (2) Role IDs must correspond one-to-one with Faction IDs (signifies Faction leadership), excepting reserved Role IDs
     * (3) Currency IDs must correspond one-to-one with Faction IDs (signifies Faction ownership), excepting reserved Currency IDs
     * (4) Either MEMBER or HEAD_GOVERNMENT must be able to vote
     * (5) Region IDs and Party IDs cannot overlap
     * (6) Every Ballot Link must have a valid Ballot ID
     * (7) Procedures must target or filter valid Ballot IDs, Role IDs, and Currency IDs
     */
    private void Validate () {
        // TODO: test these FURIOUSLY
#region (1)
        if (RolesPermissions.Keys.All (r => r.ID != Role.MEMBER)) {
            throw new ArgumentException ("There must be a MEMBER Role");
        }
#endregion

#region (2)
        foreach (Role ro in RolesPermissions.Keys) {
            if (ro.ID != Role.MEMBER && ro.ID != Role.HEAD_GOVERNMENT && ro.ID != Role.HEAD_STATE) {
                bool isRegion = false;
                bool isParty = false;

                foreach (Region re in Regions) {
                    if (ro.ID == re.ID) {
                        isRegion = true;
                        break;
                    }
                }

                if (isRegion) {
                    continue;
                }

                foreach (Party p in Parties) {
                    if (ro.ID == p.ID) {
                        isParty = true;
                    }
                }

                if (!isParty) {
                    throw new ArgumentException ($"Role ID {ro.ID} does not correspond with any Faction ID");
                }
            }
        }
#endregion

#region (3)
        foreach (Currency c in Currencies.Keys) {
            if (c.ID != Currency.STATE && c.ID != Currency.PARTY && c.ID != Currency.REGION) {
                bool isRegion = false;
                bool isParty = false;

                foreach (Region r in Regions) {
                    if (c.ID == r.ID) {
                        isRegion = true;
                        break;
                    }
                }

                if (isRegion) {
                    continue;
                }

                foreach (Party p in Parties) {
                    if (c.ID == p.ID) {
                        isParty = true;
                    }
                }

                if (!isParty) {
                    throw new ArgumentException ($"Currency ID {c.ID} does not correspond with any Faction ID");
                }
            }
        }
#endregion

#region (4)
        if (
            RolesPermissions.Where (k => k.Key.ID == Role.MEMBER).All (k => k.Value.CanVote is false)
            && RolesPermissions.Where (k => k.Key.ID == Role.HEAD_GOVERNMENT).All (k => k.Value.CanVote is false)
        ) {
            throw new ArgumentException ("Either MEMBER or HEAD_GOVERNMENT must be able to vote");
        }
#endregion

#region (5)
        foreach (Region r in Regions) {
            foreach (Party p in Parties) {
                if (r.ID == p.ID) {
                    throw new ArgumentException ($"Region ID {r.ID} overlaps with Party ID {p.ID}");
                }
            }
        }
#endregion

#region (6)
        foreach (Ballot b in Ballots) {
            foreach (var l in b.PassResult.Links) {
                if (l.TargetID >= Ballots.Count) {
                    throw new ArgumentException ($"Ballot ID {b.ID} passage Link targeting {l.TargetID} does not correspond with any Ballot ID");
                }
            }

            foreach (var l in b.FailResult.Links) {
                if (l.TargetID >= Ballots.Count) {
                    throw new ArgumentException ($"Ballot ID {b.ID} failure Link targeting {l.TargetID} does not correspond with any Ballot ID");
                }
            }
        }
#endregion

#region (7)
        // TODO: Procedures must target or filter valid Ballot IDs, Role IDs, and Currency IDs

#endregion
    }
}
