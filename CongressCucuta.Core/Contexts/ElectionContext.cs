using CongressCucuta.Core.Generators;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Core.Contexts;

public class ElectionContext : IComparable<ElectionContext> {
    public enum ElectionType {
        ShuffleRemove, // Filters: Parties (out)
        ShuffleAdd, // Filters: Parties (in)
        Region, // Filters: Roles (excluded)
        Party, // Filters: Roles (excluded)
        Nominated, // Elects: Role; Filters: Roles (excluded)
        Appointed, // Elects: Role; Filters: Roles (excluded)
    }

    /*
     * TargetIDs:
     * Last Target is displayed in election view
     *
     * PeopleAreCandidates:
     * (person, isCandidate)
     * person's presence indicates that they should/did participate in election
     * isCandidate incidates whether or not they should be electable
     * Elections that run completely randomly should have their participants present, but as non-candidates
     */
    public readonly record struct Group (IDType FactionID, IDType[] TargetIDs, SortedDictionary<IDType, bool> PeopleAreCandidates) {
        public static readonly IDType NOMINEES = byte.MaxValue;
        public static readonly IDType APPOINTEES = NOMINEES - 1;
    }

    private readonly IGenerator _generator;
    private readonly bool _isLeaderNeeded;
    public IDType? ProcedureID { get; }
    public ElectionType Type { get; }
    public IDType TargetID { get; } = 0;
    public IDType[] FilterIDs { get; } = [];
    public bool IsRandom { get; }

    public ElectionContext (ElectionType type, IDType[] filterIds, sbyte value = 0, bool isLeaderNeeded = false, IGenerator? generator = null) {
        if (filterIds.Length == 0) {
            throw new ArgumentException ("Election must have Filter", nameof (filterIds));
        }

        Type = type;
        FilterIDs = filterIds;
        IsRandom = value > 0;
        _generator = generator ?? new RandomGenerator ();
        _isLeaderNeeded = isLeaderNeeded;

        if (_isLeaderNeeded) {
            TargetID = Role.LEADER_PARTY;
        }
    }

    public ElectionContext (IDType procedureId, Procedure.Effect effect, bool isLeaderNeeded = false, IGenerator? generator = null) {
        ProcedureID = procedureId;
        IsRandom = effect.Value > 0;
        _generator = generator ?? new RandomGenerator ();
        _isLeaderNeeded = isLeaderNeeded;

        switch (effect.Type) {
            case Procedure.Effect.EffectType.ElectionRegion:
                Type = ElectionType.Region;
                FilterIDs = effect.TargetIDs;

                if (_isLeaderNeeded) {
                    TargetID = Role.LEADER_REGION;
                }

                break;
            case Procedure.Effect.EffectType.ElectionParty:
                Type = ElectionType.Party;
                FilterIDs = effect.TargetIDs;

                if (_isLeaderNeeded) {
                    TargetID = Role.LEADER_PARTY;
                }

                break;
            case Procedure.Effect.EffectType.ElectionNominated:
                Type = ElectionType.Nominated;
                TargetID = effect.TargetIDs[0];

                if (effect.TargetIDs.Length > 1) {
                    FilterIDs = effect.TargetIDs[1 ..];
                }

                break;
            case Procedure.Effect.EffectType.ElectionAppointed:
                Type = ElectionType.Appointed;
                TargetID = effect.TargetIDs[0];

                if (effect.TargetIDs.Length > 1) {
                    FilterIDs = effect.TargetIDs[1 ..];
                }

                break;
            default:
                throw new NotSupportedException ();
        }
    }

    public (Dictionary<IDType, SortedSet<IDType>>, Dictionary<IDType, (IDType?, IDType?)>, Dictionary<IDType, Group>) Run (
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        HashSet<IDType> partiesActive,
        HashSet<IDType> regionsActive
    ) {
        Dictionary<IDType, Group> groups = [];
        void AssignGroup (IDType factionId, IDType personId, IDType[] targetIds, bool isCandidate) {
            if (groups.TryGetValue (factionId, out Group group)) {
                group.PeopleAreCandidates[personId] = isCandidate;
            } else {
                groups[factionId] = new (factionId, targetIds, []);
                groups[factionId].PeopleAreCandidates[personId] = isCandidate;
            }
        }
        bool IsEligible (IDType personId) => ! FilterIDs.Intersect (peopleRoles[personId]).Any ();
        Dictionary<IDType, SortedSet<IDType>> peopleRolesNew = peopleRoles.ToDictionary (kv => kv.Key, kv => new SortedSet<IDType> ([.. kv.Value]));
        Dictionary<IDType, (IDType?, IDType?)> peopleFactionsNew = peopleFactions.ToDictionary (kv => kv.Key, kv => kv.Value);

        switch (Type) {
            case ElectionType.ShuffleRemove: {
                // Remove dissolved party leader Role
                foreach (IDType p in FilterIDs) {
                    foreach (var kv in peopleRoles) {
                        if (kv.Value.Contains (p)) {
                            peopleRolesNew[kv.Key] = [.. kv.Value.Where (r => p != r)];
                        }
                    }
                }

                // Randomly assign parties to dissolved party members
                List<IDType> partiesIds = [.. partiesActive];

                foreach (var kv in peopleFactions) {
                    if (FilterIDs.Any (p => kv.Value.Item1 == p)) {
                        int partyIdx = _generator.Choose (partiesActive.Count);
                        IDType partyId = partiesIds[partyIdx];

                        peopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                        AssignGroup (partyId, kv.Key, [partyId, Role.LEADER_PARTY], false);
                    }
                }

                break;
            }
            case ElectionType.ShuffleAdd: {
                HashSet<IDType> partiesAssigned = [];
                List<IDType> peopleUnassigned = [];
                bool isCandidate = _isLeaderNeeded && ! IsRandom;

                // Randomly assign non-party leaders to founded party
                foreach (var kv in peopleFactions) {
                    if (
                        kv.Value.Item1 is not null
                        && ! peopleRoles[kv.Key].Intersect (partiesActive).Any ()
                    ) {
                        int partyIdx = _generator.Choose (partiesActive.Count);

                        if (partyIdx < FilterIDs.Length) {
                            IDType partyId = FilterIDs[partyIdx];

                            peopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                            partiesAssigned.Add (partyId);
                            AssignGroup (partyId, kv.Key, [partyId, Role.LEADER_PARTY], isCandidate);
                        } else {
                            peopleUnassigned.Add (kv.Key);
                        }
                    }
                }

                // Forcibly assign a random unassigned person to founded party if empty
                if (peopleUnassigned.Count > 0) {
                    foreach (IDType f in FilterIDs) {
                        if (! partiesAssigned.Contains (f)) {
                            int unassignedIdx = _generator.Choose (peopleUnassigned.Count);
                            IDType unassignedId = peopleUnassigned[unassignedIdx];

                            peopleFactionsNew[unassignedId] = (f, peopleFactionsNew[unassignedId].Item2);
                            AssignGroup (f, unassignedId, [f, Role.LEADER_PARTY], isCandidate);
                        }
                    }
                }

                // Randomly appoint party leader for founded party if leader selection is random
                if (_isLeaderNeeded && IsRandom) {
                    foreach (IDType p in FilterIDs) {
                        List<IDType> peopleParty = [.. peopleFactionsNew.Where (kv => kv.Value.Item1 == p).Select (kv => kv.Key)];
                        int personIdx = _generator.Choose (peopleParty.Count);
                        IDType personId = peopleParty[personIdx];

                        peopleRolesNew[personId].Add (p);
                        peopleRolesNew[personId].Add (Role.LEADER_PARTY);
                        AssignGroup (p, personId, [p, Role.LEADER_PARTY], true);
                    }
                }

                break;
            }
            case ElectionType.Region: {
                List<IDType> regionIds = [.. regionsActive];
                List<IDType> regionsUnassigned = [.. regionsActive];
                bool isCandidate = _isLeaderNeeded && ! IsRandom;

                // Remove all region leader Role
                foreach (var rs in peopleRolesNew.Values) {
                    rs.Remove (Role.LEADER_REGION);

                    foreach (IDType l in regionsActive) {
                        rs.Remove (l);
                    }
                }

                // Randomly assign people (inclusive) to regions, prioritising empty regions
                foreach (var kv in peopleFactions) {
                    int regionIdx;
                    IDType regionId;

                    if (regionsUnassigned.Count > 0) {
                        regionIdx = _generator.Choose (regionsUnassigned.Count);
                        regionId = regionsUnassigned[regionIdx];
                        regionsUnassigned.RemoveAt (regionIdx);
                    } else {
                        regionIdx = _generator.Choose (regionsActive.Count);
                        regionId = regionIds[regionIdx];
                    }

                    peopleFactionsNew[kv.Key] = (kv.Value.Item1, regionId);
                    AssignGroup (regionId, kv.Key, [regionId, Role.LEADER_REGION], isCandidate && IsEligible (kv.Key));
                }

                // Randomly appoint region leader for each region if leader selection is random
                if (_isLeaderNeeded && IsRandom) {
                    foreach (Group g in groups.Values) {
                        // Not excluded by group assignemnt
                        List<IDType> peopleRegion = [.. g.PeopleAreCandidates.Keys.Where (p => IsEligible (p))];
                        int personIdx = _generator.Choose (peopleRegion.Count);
                        IDType personId = peopleRegion[personIdx];

                        peopleRolesNew[personId].Add (g.FactionID);
                        peopleRolesNew[personId].Add (Role.LEADER_REGION);
                        AssignGroup (g.FactionID, personId, [g.FactionID, Role.LEADER_REGION], true);
                    }
                }

                break;
            }
            case ElectionType.Party: {
                List<IDType> partyIds = [.. partiesActive];
                List<IDType> partiesUnassigned = [.. partiesActive];
                bool isCandidate = _isLeaderNeeded && ! IsRandom;

                // Remove all party leader Role
                foreach (var rs in peopleRolesNew.Values) {
                    rs.Remove (Role.LEADER_PARTY);

                    foreach (IDType l in partiesActive) {
                        rs.Remove (l);
                    }
                }

                // Randomly assign people (exclusive) to parties, prioritising empty parties
                foreach (var kv in peopleFactions.Where (kv => IsEligible (kv.Key))) {
                    int partyIdx;
                    IDType partyId;

                    if (partiesUnassigned.Count > 0) {
                        partyIdx = _generator.Choose (partiesUnassigned.Count);
                        partyId = partiesUnassigned[partyIdx];
                        partiesUnassigned.RemoveAt (partyIdx);
                    } else {
                        partyIdx = _generator.Choose (partiesActive.Count);
                        partyId = partyIds[partyIdx];
                    }

                    peopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                    AssignGroup (partyId, kv.Key, [partyId, Role.LEADER_PARTY], isCandidate);
                }

                // Randomly appoint party leader for each party if leader selection is random
                if (_isLeaderNeeded && IsRandom) {
                    foreach (Group g in groups.Values) {
                        // Already excluded by group assignment
                        List<IDType> peopleParty = [.. g.PeopleAreCandidates.Keys];
                        int personIdx = _generator.Choose (peopleParty.Count);
                        IDType personId = peopleParty[personIdx];

                        peopleRolesNew[personId].Add (g.FactionID);
                        peopleRolesNew[personId].Add (Role.LEADER_PARTY);
                        AssignGroup (g.FactionID, personId, [g.FactionID, Role.LEADER_PARTY], true);
                    }
                }

                break;
            }
            case ElectionType.Nominated: {
                // Remove all nominated Role
                foreach (var kv in peopleRolesNew) {
                    kv.Value.Remove (TargetID);
                }

                // Nominate candidates for Role
                foreach (var kv in peopleRoles.Where (kv => IsEligible (kv.Key))) {
                    AssignGroup (Group.NOMINEES, kv.Key, [TargetID], true);
                }

                break;
            }
            case ElectionType.Appointed: {
                // Remove all appointed Role
                foreach (var kv in peopleRolesNew) {
                    kv.Value.Remove (TargetID);
                }

                if (IsRandom) {
                    // Randomly appoint Role
                    List<IDType> peopleIds = [];

                    foreach (var kv in peopleRoles.Where (kv => IsEligible (kv.Key))) {
                        peopleIds.Add (kv.Key);
                    }

                    int personIdx = _generator.Choose (peopleIds.Count);
                    IDType personId = peopleIds[personIdx];

                    peopleRolesNew[personId].Add (TargetID);
                    AssignGroup (Group.APPOINTEES, personId, [TargetID], false);
                } else {
                    // Nominate candidates for Role
                    foreach (var kv in peopleRoles.Where (kv => IsEligible (kv.Key))) {
                        AssignGroup (Group.APPOINTEES, kv.Key, [TargetID], true);
                    }
                }

                break;
            }
        }

        return (peopleRolesNew, peopleFactionsNew, groups);
    }

    public int CompareTo (ElectionContext? other) {
        if (other is ElectionContext o) {
            if (
                Type is ElectionType.ShuffleRemove or ElectionType.ShuffleAdd
                || o.Type is ElectionType.ShuffleRemove or ElectionType.ShuffleAdd
            ) {
                return Type.CompareTo (o.Type);
            } else if (ProcedureID is IDType p1 && o.ProcedureID is IDType p2) {
                return p1.CompareTo (p2);
            } else {
                throw new NotSupportedException ();
            }
        } else {
            throw new NotSupportedException ();
        }
    }
}
