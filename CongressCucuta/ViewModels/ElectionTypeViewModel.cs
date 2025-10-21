using System.Collections.ObjectModel;
using System.Diagnostics;
using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Generators;
using CongressCucuta.Models;

namespace CongressCucuta.ViewModels;

internal abstract class ElectionTypeViewModel (
    Dictionary<IDType, SortedSet<IDType>> peopleRoles,
    Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
    Localisation localisation,
    bool isLeaderNeeded,
    IGenerator? generator = null
) : ViewModel {
    protected readonly IGenerator _generator = generator ?? new RandomGenerator ();
    protected string _title = string.Empty;
    protected ObservableCollection<GroupViewModel> _groupsPeople = [];
    protected IDType[] _targetIds = [];
    protected Localisation _localisation = localisation;
    public Dictionary<IDType, SortedSet<IDType>> PeopleRolesNew => peopleRoles;
    public Dictionary<IDType, (IDType?, IDType?)> PeopleFactionsNew => peopleFactions;
    public string Title {
        get => _title;
        set {
            _title = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<GroupViewModel> GroupsPeople {
        get => _groupsPeople;
        set {
            _groupsPeople = value;
            OnPropertyChanged ();
        }
    }
    public bool IsLeaderNeeded => isLeaderNeeded;

    public abstract bool CanContinue ();

    public static ElectionTypeViewModel Create (
        ElectionModel election,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        HashSet<IDType> partiesActive,
        HashSet<IDType> regionsActive,
        Localisation localisation,
        List<string> people
    ) =>
        election.Type switch {
            ElectionContext.ElectionType.ShuffleRemove =>
                new ElectionShuffleRemoveViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    partiesActive,
                    localisation,
                    people
                ),
            ElectionContext.ElectionType.ShuffleAdd =>
                new ElectionShuffleAddViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    partiesActive,
                    localisation,
                    people
                ),
            ElectionContext.ElectionType.Region =>
                new ElectionRegionViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    regionsActive,
                    localisation,
                    people
                ),
            ElectionContext.ElectionType.Party =>
                new ElectionPartyViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    partiesActive,
                    localisation,
                    people
                ),
            ElectionContext.ElectionType.Nominated =>
                new ElectionNominatedViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    localisation,
                    people
                ),
            ElectionContext.ElectionType.Appointed =>
                new ElectionAppointedViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    localisation,
                    people
                ),
            _ => throw new NotSupportedException (),
        };

    private void PersonGroup_SelectedChangedEventHandler (GroupViewModel.PersonGroup sender, bool e) {
        List<IDType> roleIds = [.. _targetIds, sender.FactionID];

        foreach (IDType roleId in roleIds) {
            if (e) {
                if (PeopleRolesNew.TryGetValue (sender.ID, out var rs)) {
                    rs.Add (roleId);
                } else {
                    // Everyone should have a MEMBER Role
                    throw new UnreachableException ();
                }
            } else {
                PeopleRolesNew[sender.ID].Remove (roleId);
            }
        }
    }

    protected void TryAddFaction (IDType id, string name) {
        if (_groupsPeople.All (f => f.ID != id)) {
            _groupsPeople.Add (new (id, name, IsLeaderNeeded));
        }
    }

    protected string GenerateName (IDType personId, string name) {
        string result = name;

        if (PeopleFactionsNew[personId].Item1 is IDType p) {
            result += $" [{_localisation.GetFactionOrAbbreviation (p)}]";
        }

        if (PeopleFactionsNew[personId].Item2 is IDType r) {
            result += $" [{_localisation.GetFactionOrAbbreviation (r)}]";
        }

        return result;
    }

    protected void AddPerson (IDType factionId, IDType personId, string personName, bool isCandidate = false) {
        string name = GenerateName (personId, personName);
        GroupViewModel.PersonGroup person = new (personId, factionId, name, isCandidate);

        person.SelectedChanged += PersonGroup_SelectedChangedEventHandler;

        if (_groupsPeople.All (f => f.ID != factionId)) {
            _groupsPeople.Add (new (factionId, _localisation.GetFactionOrAbbreviation (factionId), IsLeaderNeeded));
        }

        _groupsPeople.Where (f => f.ID == factionId).First ().AddPerson (person);
    }

    public bool IsSelected () => _groupsPeople.All (f => f.IsSelected ());

    protected void Sort () {
        foreach (GroupViewModel f in _groupsPeople) {
            f.People = [.. f.People.OrderBy (p => p.ID)];
        }

        GroupsPeople = [.. _groupsPeople.OrderBy (f => f.ID)];
    }
}

internal class ElectionShuffleRemoveViewModel : ElectionTypeViewModel {
    public ElectionShuffleRemoveViewModel (
        ElectionModel election,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        HashSet<IDType> partiesActive,
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, peopleFactions, localisation, false) {
        foreach (IDType p in election.FilterIDs) {
            foreach (var kv in peopleRoles) {
                if (kv.Value.Contains (p)) {
                    PeopleRolesNew[kv.Key] = [.. kv.Value.Where (r => p != r)];
                }
            }
        }

        List<IDType> partiesIds = [.. partiesActive];

        foreach (var kv in peopleFactions) {
            if (election.FilterIDs.Any (p => kv.Value.Item1 == p)) {
                int partyIdx = _generator.Choose (partiesActive.Count);
                IDType partyId = partiesIds[partyIdx];
                string name = people[kv.Key];

                PeopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                AddPerson (partyId, kv.Key, people[kv.Key]);
            }
        }

        Title = $"Dissolution of {string.Join (", ", election.FilterIDs.Select (p => _localisation.GetFactionOrAbbreviation (p)))}";
        Sort ();
    }

    public override bool CanContinue () => true;
}

internal class ElectionShuffleAddViewModel : ElectionTypeViewModel {
    public ElectionShuffleAddViewModel (
        ElectionModel election,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        HashSet<IDType> partiesActive,
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, peopleFactions, localisation, localisation.Roles.ContainsKey (Role.LEADER_PARTY)) {
        List<IDType> unassignedIds = [];
        HashSet<IDType> factionsAssigned = [];

        foreach (var kv in peopleFactions) {
            if (
                kv.Value.Item1 is not null
                && peopleRoles[kv.Key].All (r => ! partiesActive.Contains (r))
            ) {
                int partyIdx = _generator.Choose (partiesActive.Count);

                if (partyIdx < election.FilterIDs.Length) {
                    IDType partyId = election.FilterIDs[partyIdx];

                    PeopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                    AddPerson (partyId, kv.Key, people[kv.Key], IsLeaderNeeded);
                    factionsAssigned.Add (partyId);
                } else {
                    unassignedIds.Add (kv.Key);
                }
            }
        }

        if (unassignedIds.Count > 0) {
            foreach (IDType f in election.FilterIDs) {
                if (! factionsAssigned.Contains (f)) {
                    int unassignedIdx = _generator.Choose (unassignedIds.Count);
                    IDType unassignedId = unassignedIds[unassignedIdx];

                    PeopleFactionsNew[unassignedId] = (f, PeopleFactionsNew[unassignedId].Item2);
                    AddPerson (f, unassignedId, people[unassignedId], IsLeaderNeeded);
                }
            }
        }

        Title = $"Founding of {string.Join (", ", election.FilterIDs.Select (p => _localisation.GetFactionOrAbbreviation (p)))}";
        Sort ();
    }

    public override bool CanContinue () => ! IsLeaderNeeded || IsSelected ();
}

internal class ElectionRegionViewModel : ElectionTypeViewModel {
    private readonly bool _isRandom;

    public ElectionRegionViewModel (
        ElectionModel election,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        HashSet<IDType> regionsActive,
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, peopleFactions, localisation, localisation.Roles.ContainsKey (Role.LEADER_REGION)) {
        List<IDType> regionIds = [.. regionsActive];
        List<IDType> regionsUnassigned = [.. regionsActive];

        if (IsLeaderNeeded) {
            _targetIds = [Role.LEADER_REGION];

            foreach (var rs in peopleRoles.Values) {
                rs.Remove (Role.LEADER_REGION);

                foreach (IDType l in regionsActive) {
                    rs.Remove (l);
                }
            }
        }

        _isRandom = election.IsRandom;

        foreach (var kv in peopleFactions) {
            if (peopleRoles[kv.Key].All (r => ! election.FilterIDs.Contains (r))) {
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

                PeopleFactionsNew[kv.Key] = (kv.Value.Item1, regionId);
                AddPerson (regionId, kv.Key, people[kv.Key], IsLeaderNeeded && !_isRandom);
            }
        }

        if (_isRandom) {
            foreach (GroupViewModel group in _groupsPeople) {
                int personIdx = _generator.Choose (group.People.Count);
                IDType personId = group.People[personIdx].ID;

                group.People[personIdx].IsCandidate = true;
            }
        }

        Title = _localisation.Procedures[(IDType) election.ProcedureID!].Item1;
        Sort ();
    }

    public override bool CanContinue () => ! IsLeaderNeeded || IsSelected ();
}

internal class ElectionPartyViewModel : ElectionTypeViewModel {
    private readonly bool _isRandom;

    public ElectionPartyViewModel (
        ElectionModel election,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        HashSet<IDType> partiesActive,
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, peopleFactions, localisation, localisation.Roles.ContainsKey (Role.LEADER_PARTY)) {
        List<IDType> partyIds = [.. partiesActive];
        List<IDType> partiesUnassigned = [.. partiesActive];

        if (IsLeaderNeeded) {
            _targetIds = [Role.LEADER_PARTY];

            foreach (var rs in peopleRoles.Values) {
                rs.Remove (Role.LEADER_PARTY);

                foreach (IDType l in partiesActive) {
                    rs.Remove (l);
                }
            }
        }

        _isRandom = election.IsRandom;

        foreach (var kv in peopleFactions) {
            if (peopleRoles[kv.Key].All (r => ! election.FilterIDs.Contains (r))) {
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

                PeopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                AddPerson (partyId, kv.Key, people[kv.Key], IsLeaderNeeded && ! _isRandom);
            }
        }

        if (_isRandom) {
            foreach (GroupViewModel group in _groupsPeople) {
                int personIdx = _generator.Choose (group.People.Count);
                IDType personId = group.People[personIdx].ID;

                group.People[personIdx].IsCandidate = true;
            }
        }

        Title = _localisation.Procedures[(IDType) election.ProcedureID!].Item1;
        Sort ();
    }

    public override bool CanContinue () => ! IsLeaderNeeded || IsSelected ();
}

internal class ElectionNominatedViewModel : ElectionTypeViewModel {
    public ElectionNominatedViewModel (
        ElectionModel election,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, peopleFactions, localisation, true) {
        _targetIds = [election.TargetID];
        TryAddFaction (election.TargetID, "Candidates");

        foreach (var kv in peopleRoles) {
            kv.Value.Remove (election.TargetID);
        }

        foreach (var kv in peopleRoles) {
            if (kv.Value.All (r => ! election.FilterIDs.Contains (r))) {
                AddPerson (election.TargetID, kv.Key, people[kv.Key], true);
            }
        }

        Title = _localisation.Procedures[(IDType) election.ProcedureID!].Item1;
        Sort ();
    }

    public override bool CanContinue () => IsSelected ();
}

internal class ElectionAppointedViewModel : ElectionTypeViewModel {
    private readonly bool _isRandom;

    public ElectionAppointedViewModel (
        ElectionModel election,
        Dictionary<IDType, SortedSet<IDType>> peopleRoles,
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, peopleFactions, localisation, ! election.IsRandom) {
        _targetIds = [election.TargetID];
        _isRandom = election.IsRandom;

        foreach (var kv in peopleRoles) {
            kv.Value.Remove (election.TargetID);
        }

        if (_isRandom) {
            List<IDType> peopleIds = [];

            foreach (var kv in peopleRoles) {
                if (kv.Value.All (r => ! election.FilterIDs.Contains (r))) {
                    peopleIds.Add (kv.Key);
                }
            }

            int personIdx = _generator.Choose (peopleIds.Count);
            IDType personId = peopleIds[personIdx];

            PeopleRolesNew[personId].Add (election.TargetID);
            TryAddFaction (election.TargetID, "Appointment");
            AddPerson (election.TargetID, personId, people[personId], false);
        } else {
            TryAddFaction (election.TargetID, "Candidates");

            foreach (var kv in peopleRoles) {
                if (kv.Value.All (r => ! election.FilterIDs.Contains (r))) {
                    AddPerson (election.TargetID, kv.Key, people[kv.Key], true);
                }
            }
        }

        Title = _localisation.Procedures[(IDType) election.ProcedureID!].Item1;
        Sort ();
    }

    public override bool CanContinue () => _isRandom || IsSelected ();
}
