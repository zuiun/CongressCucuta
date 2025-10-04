namespace congress_cucuta.Data;

internal class Simulation {
    public History History { get; }
    public Dictionary<Role, Permissions> RolesPermissions { get; }
    public List<Faction> Regions { get; }
    public List<Faction> Parties { get; }
    public Dictionary<Currency, sbyte> CurrenciesValues { get; }
    public List<ProcedureImmediate> ProceduresGovernmental { get; }
    public List<ProcedureTargeted> ProceduresSpecial { get; }
    public List<ProcedureDeclared> ProceduresDeclared { get; }
    public List<Ballot> Ballots { get; }
    public List<Result> Results { get; }
    public Localisation Localisation { get; }

    public Simulation (
        History history,
        Dictionary<Role, Permissions> rolesPermissions,
        List<Faction> regions,
        List<Faction> parties,
        Dictionary<Currency, sbyte> currenciesValues,
        List<ProcedureImmediate> proceduresGovernmental,
        List<ProcedureTargeted> proceduresSpecial,
        List<ProcedureDeclared> proceduresDeclared,
        List<Ballot> ballots,
        List<Result> results,
        Localisation localisation
    ) {
        History = history;
        RolesPermissions = rolesPermissions;
        Regions = regions;
        Parties = parties;
        CurrenciesValues = currenciesValues;
        ProceduresGovernmental = proceduresGovernmental;
        ProceduresSpecial = proceduresSpecial;
        ProceduresDeclared = proceduresDeclared;
        Ballots = ballots;
        Results = results;
        Localisation = localisation;
        Validate ();
    }

    /*
     * (1) There must be a MEMBER Role
     * (2) Role IDs must correspond one-to-one with Faction IDs (signifies Faction leadership), excepting reserved Role IDs
     * (3) Currency IDs must correspond one-to-one with Faction IDs (signifies Faction ownership), excepting reserved Currency IDs (only STATE)
     * (4) Either MEMBER or HEAD_GOVERNMENT must be able to vote
     * (5) Region IDs and Party IDs cannot overlap
     * (6) Every Ballot Link must have a valid Ballot ID
     * (7) Procedures must target or filter valid Ballot IDs, Role IDs, and Currency IDs
     * (8) If any Party has a Currency, then every Party must have a Currency; same restriction applies to Regions
     * (9) Every IID must have a Localisation entry
     * (10) Every IID of a certain type must have a unique ID
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
            if (
                ro.ID != Role.MEMBER
                && ro.ID != Role.HEAD_GOVERNMENT
                && ro.ID != Role.HEAD_STATE
                && ro.ID != Role.LEADER_PARTY
                && ro.ID != Role.LEADER_REGION
            ) {
                bool isRegion = false;
                bool isParty = false;

                foreach (Faction re in Regions) {
                    if (ro.ID == re.ID) {
                        isRegion = true;
                        break;
                    }
                }

                if (isRegion) {
                    continue;
                }

                foreach (Faction p in Parties) {
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
        foreach (Currency c in CurrenciesValues.Keys) {
            if (c.ID != Currency.STATE) {
                bool isRegion = false;
                bool isParty = false;

                foreach (Faction r in Regions) {
                    if (c.ID == r.ID) {
                        isRegion = true;
                        break;
                    }
                }

                if (isRegion) {
                    continue;
                }

                foreach (Faction p in Parties) {
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
            RolesPermissions.Where (k => k.Key.ID == Role.MEMBER).All (v => ! v.Value.CanVote)
            && RolesPermissions.Where (k => k.Key.ID == Role.HEAD_GOVERNMENT).All (v => ! v.Value.CanVote)
        ) {
            throw new ArgumentException ("Either MEMBER or HEAD_GOVERNMENT must be able to vote");
        }
#endregion

#region (5)
        foreach (Faction r in Regions) {
            foreach (Faction p in Parties) {
                if (r.ID == p.ID) {
                    throw new ArgumentException ($"Region ID {r.ID} overlaps with Party ID {p.ID}");
                }
            }
        }

        for (byte i = 0; i < Regions.Count; ++ i) {
            for (byte j = 0; j < Regions.Count; ++ j) {
                if (Regions[i].ID == Regions[j].ID && i != j) {
                    throw new ArgumentException ($"Region ID {Regions[i].ID} is repeated");
                }
            }
        }

        for (byte i = 0; i < Parties.Count; ++i) {
            for (byte j = 0; j < Parties.Count; ++j) {
                if (Parties[i].ID == Parties[j].ID && i != j) {
                    throw new ArgumentException ($"Party ID {Parties[i].ID} is repeated");
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

#region (8)


#endregion

#region (9)

#endregion

#region (10)

#endregion
    }
}
