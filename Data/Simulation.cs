namespace congress_cucuta.Data;

internal class Simulation {
    public History History { get; }
    public Dictionary<IDType, Permissions> RolesPermissions { get; }
    public List<Faction> Regions { get; }
    public List<Faction> Parties { get; }
    public Dictionary<IDType, sbyte> CurrenciesValues { get; }
    public List<ProcedureImmediate> ProceduresGovernmental { get; }
    public List<ProcedureTargeted> ProceduresSpecial { get; }
    public List<ProcedureDeclared> ProceduresDeclared { get; }
    public List<Ballot> Ballots { get; }
    public List<Result> Results { get; }
    public Localisation Localisation { get; }

    public Simulation (
        History history,
        Dictionary<IDType, Permissions> rolesPermissions,
        List<Faction> regions,
        List<Faction> parties,
        Dictionary<IDType, sbyte> currenciesValues,
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
     * (8) If any Region has a Currency, then every Region must have a Currency; same restriction applies to Party
     * (9) Every IID must have a Localisation entry
     * (10) Every IID of a certain type must have a unique ID that matches its container's index
     * (11) If there are Currencies, then the first ProcedureImmediate must have Action CurrencyInitialise
     */
    private void Validate () {
#region (1)
        if (RolesPermissions.Keys.All (r => r.ID != Role.MEMBER)) {
            throw new ArgumentException ("There must be a MEMBER Role");
        }
#endregion

#region (2)
        foreach (IDType ro in RolesPermissions.Keys) {
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
        foreach (IDType c in CurrenciesValues.Keys) {
            if (c != Currency.STATE) {
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
        foreach (ProcedureImmediate pi in ProceduresGovernmental) {
            foreach (Procedure.Effect e in pi.Effects) {
                if (e.TargetIDs.Any (t => RolesPermissions.Keys.All (r => t != r.ID))) {
                    throw new ArgumentException ($"ProcedureImmediate ID {pi.ID} targets an invalid Role ID");
                }
            }
        }

        foreach (ProcedureTargeted pt in ProceduresSpecial) {
            if (pt.BallotIDs.Any (t => Ballots.All (b => t != b.ID))) {
                throw new ArgumentException ($"ProcedureTargeted ID {pt.ID} filters an invalid Ballot ID");
            }

            foreach (Procedure.Effect e in pt.Effects) {
                switch (e.Action) {
                    case Procedure.Effect.ActionType.CurrencyAdd:
                    case Procedure.Effect.ActionType.CurrencySubtract:
                        if (
                            e.TargetIDs.Any (t => Regions.All (r => t != r.ID))
                            && e.TargetIDs.Any (t => Parties.All (p => t != p.ID))
                        ) {
                            throw new ArgumentException ($"ProcedureTargeted ID {pt.ID} targets an invalid Faction ID");
                        }

                        break;
                    case Procedure.Effect.ActionType.ProcedureActivate:
                        if (ProceduresGovernmental.All (pi => pi.ID != e.TargetIDs[0])) {
                            throw new ArgumentException ($"ProcedureTargeted ID {pt.ID} targets an invalid ProcedureImmediate ID");
                        }

                        break;
                }
            }
        }

        foreach (ProcedureDeclared pd in ProceduresDeclared) {
            if (pd.DeclarerIDs.Any (t => RolesPermissions.Keys.All (r => t != r))) {
                throw new ArgumentException ($"ProcedureDeclared ID {pd.ID} filters an invalid Role ID");
            }

            foreach (Procedure.Effect e in pd.Effects) {
                if (e.TargetIDs.Any (t => RolesPermissions.Keys.All (r => t != r.ID))) {
                    throw new ArgumentException ($"ProcedureDeclared ID {pd.ID} targets an invalid Role ID");
                }
            }
        }
#endregion

#region (8)
        if (Regions.Count > 0 && CurrenciesValues.Keys.Any (c => c.ID == Regions[0].ID)) {
            foreach (Faction r in Regions) {
                if (CurrenciesValues.Keys.All (c => r.ID != c.ID)) {
                    throw new ArgumentException ($"Region ID {r.ID} does not correspond with any Currency ID");
                }
            }
        }

        if (Parties.Count > 0 && CurrenciesValues.Keys.Any (c => c.ID == Parties[0].ID)) {
            foreach (Faction p in Parties) {
                if (CurrenciesValues.Keys.All (c => p.ID != c.ID)) {
                    throw new ArgumentException ($"Party ID {p.ID} does not correspond with any Currency ID");
                }
            }
        }
#endregion

#region (9)
        foreach (IDType r in RolesPermissions.Keys) {
            if (! Localisation.Roles.ContainsKey (r)) {
                throw new ArgumentException ($"Role ID {r} does not have a Localisation entry");
            }
        }

        foreach (Faction r in Regions) {
            if (! Localisation.Regions.ContainsKey (r.ID)) {
                throw new ArgumentException ($"Region ID {r.ID} does not have a Localisation entry");
            }
        }

        foreach (Faction p in Parties) {
            if (! Localisation.Parties.ContainsKey (p.ID)) {
                throw new ArgumentException ($"Party ID {p.ID} does not have a Localisation entry");
            }
        }

        foreach (IDType c in CurrenciesValues.Keys) {
            if (! Localisation.Currencies.ContainsKey (c.ID)) {
                throw new ArgumentException ($"Currency ID {c.ID} does not have a Localisation entry");
            }
        }

        foreach (ProcedureImmediate pi in ProceduresGovernmental) {
            if (! Localisation.Procedures.ContainsKey (pi.ID)) {
                throw new ArgumentException ($"ProcedureImmediate ID {pi.ID} does not have a Localisation entry");
            }
        }

        foreach (ProcedureTargeted pt in ProceduresSpecial) {
            if (! Localisation.Procedures.ContainsKey (pt.ID)) {
                throw new ArgumentException ($"ProcedureTargeted ID {pt.ID} does not have a Localisation entry");
            }
        }

        foreach (ProcedureDeclared pd in ProceduresDeclared) {
            if (! Localisation.Procedures.ContainsKey (pd.ID)) {
                throw new ArgumentException ($"ProcedureDeclared ID {pd.ID} does not have a Localisation entry");
            }
        }

        foreach (Ballot b in Ballots) {
            if (! Localisation.Ballots.ContainsKey (b.ID)) {
                throw new ArgumentException ($"Ballot ID {b.ID} does not have a Localisation entry");
            }
        }

        foreach (Result r in Results) {
            if (!Localisation.Results.ContainsKey (r.ID)) {
                throw new ArgumentException ($"Result ID {r.ID} does not have a Localisation entry");
            }
        }
#endregion

#region (10)
        for (byte i = 0; i < Regions.Count; ++ i) {
            byte regionIdxOffset = Regions[0].ID;

            if (Regions[i].ID - regionIdxOffset != i) {
                throw new ArgumentException ($"Region ID {Regions[i].ID} does not match its offset index in Regions");
            }
        }

        for (byte i = 0; i < Parties.Count; ++i) {
            byte partyIdxOffset = Parties[0].ID;

            if (Parties[i].ID - partyIdxOffset != i) {
                throw new ArgumentException ($"Party ID {Parties[i].ID} does not match its offset index in Parties");
            }
        }

        for (byte i = 0; i < ProceduresGovernmental.Count; ++i) {
            if (ProceduresGovernmental[i].ID != i) {
                throw new ArgumentException ($"ProcedureImmediate ID {ProceduresGovernmental[i].ID} does not match its index in ProceduresGovernmental");
            }
        }

        for (byte i = 0; i < ProceduresSpecial.Count; ++i) {
            byte procedureIdxOffset = ProceduresSpecial[0].ID;

            if (ProceduresSpecial[i].ID - procedureIdxOffset != i) {
                throw new ArgumentException ($"ProcedureTargeted ID {ProceduresSpecial[i].ID} does not match its offset index in ProceduresSpecial");
            }
        }

        for (byte i = 0; i < ProceduresDeclared.Count; ++i) {
            byte procedureIdxOffset = ProceduresDeclared[0].ID;

            if (ProceduresDeclared[i].ID - procedureIdxOffset != i) {
                throw new ArgumentException ($"ProcedureDeclared ID {ProceduresDeclared[i].ID} does not match its offset index in ProceduresDeclared");
            }
        }

        for (byte i = 0; i < Ballots.Count; ++i) {
            if (Ballots[i].ID != i) {
                throw new ArgumentException ($"Ballot ID {Ballots[i].ID} does not match its index in Ballots");
            }
        }

        for (byte i = 0; i < Results.Count; ++i) {
            if (Results[i].ID != i) {
                throw new ArgumentException ($"Result ID {Results[i].ID} does not match its index in Results");
            }
        }
#endregion

#region (11)
        if (CurrenciesValues.Count > 0) {
            if (ProceduresSpecial.Count == 0) {
                throw new ArgumentException ("If there are Currencies, then the first ProcedureImmediate must have Action CurrencyInitialise");
            } else {
                bool isCurrencyInitialise = false;

                foreach (Procedure.Effect e in ProceduresSpecial[0].Effects) {
                    if (e.Action is Procedure.Effect.ActionType.CurrencyInitialise) {
                        isCurrencyInitialise = true;
                        break;
                    }
                }

                if (! isCurrencyInitialise) {
                    throw new ArgumentException ("If there are Currencies, then the first ProcedureImmediate must have Action CurrencyInitialise");
                }
            }
        }
#endregion
    }
}
