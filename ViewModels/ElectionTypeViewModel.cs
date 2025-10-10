using System.Collections.ObjectModel;
using System.Diagnostics;
using congress_cucuta.Data;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal abstract class ElectionTypeViewModel (
    Dictionary<IDType, SortedSet<IDType>> peopleRoles,
    Dictionary<IDType, (IDType?, IDType?)> peopleFactions,
    Localisation localisation,
    bool isLeaderNeeded
) : ViewModel {
    protected static readonly Random _random = new ();
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
            Election.ElectionType.ShuffleRemove =>
                new ElectionShuffleRemoveViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    partiesActive,
                    localisation,
                    people
                ),
            Election.ElectionType.ShuffleAdd =>
                new ElectionShuffleAddViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    partiesActive,
                    localisation,
                    people
                ),
            Election.ElectionType.Region =>
                new ElectionRegionViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    regionsActive,
                    localisation,
                    people
                ),
            Election.ElectionType.Party =>
                new ElectionPartyViewModel (
                    election,
                    peopleRoles,
                    peopleFactions,
                    partiesActive,
                    localisation,
                    people
                ),
            Election.ElectionType.Nominated =>
                new ElectionNominatedViewModel (
                    election,
                    peopleRoles,
                    localisation,
                    people
                ),
            Election.ElectionType.Appointed =>
                new ElectionAppointedViewModel (
                    election,
                    peopleRoles,
                    localisation,
                    people
                ),
            _ => throw new NotSupportedException (),
        };

    private void PersonGroup_SelectedChangedEventHandler (GroupViewModel.PersonGroup sender, bool e) {
        List<IDType> roleIds = [.. _targetIds, sender.FactionID];

        foreach (IDType roleId in roleIds) {
            if (e) {
                if (PeopleRolesNew.TryGetValue (sender.ID, out var r)) {
                    r.Add (roleId);
                } else {
                    // Everyone should have a MEMBER Role
                    throw new UnreachableException ();
                    // PeopleRolesNew[sender.ID] = [roleId];
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

    protected void AddPerson (IDType factionId, IDType personId, string personName, bool isCandidate = false) {
        GroupViewModel.PersonGroup person = new (personId, factionId, personName, isCandidate);

        person.SelectedChanged += PersonGroup_SelectedChangedEventHandler;

        if (_groupsPeople.All (f => f.ID != factionId)) {
            _groupsPeople.Add (new (factionId, _localisation.GetFactionAndAbbreviation (factionId), IsLeaderNeeded));
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
                int partyIdx = _random.Next (partiesActive.Count);
                IDType partyId = partiesIds[partyIdx];

                PeopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                AddPerson (partyId, kv.Key, people[kv.Key]);
            }
        }

        Title = $"Dissolution of {string.Join (", ", election.FilterIDs.Select (p => _localisation.GetFactionAndAbbreviation (p)))}";
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
                int partyIdx = _random.Next (partiesActive.Count);

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

        foreach (IDType f in election.FilterIDs) {
            if (! factionsAssigned.Contains (f)) {
                int unassignedIdx = _random.Next (unassignedIds.Count);
                IDType unassignedId = unassignedIds[unassignedIdx];

                PeopleFactionsNew[unassignedId] = (f, PeopleFactionsNew[unassignedId].Item2);
                AddPerson (unassignedId, f, people[unassignedId], IsLeaderNeeded);
            }
        }

        Title = $"Founding of {string.Join (", ", election.FilterIDs.Select (p => _localisation.GetFactionAndAbbreviation (p)))}";
        Sort ();
    }

    public override bool CanContinue () => ! IsLeaderNeeded || IsSelected ();
}

internal class ElectionRegionViewModel : ElectionTypeViewModel {
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

        foreach (var kv in peopleFactions) {
            if (peopleRoles[kv.Key].All (r => ! election.FilterIDs.Contains (r))) {
                int regionIdx;
                IDType regionId;
                
                if (regionsUnassigned.Count > 0) {
                    regionIdx = _random.Next (regionsUnassigned.Count);
                    regionId = regionsUnassigned[regionIdx];
                    regionsUnassigned.RemoveAt (regionIdx);
                } else {
                    regionIdx = _random.Next (regionsActive.Count);
                    regionId = regionIds[regionIdx];
                }

                PeopleFactionsNew[kv.Key] = (kv.Value.Item1, regionId);
                AddPerson (regionId, kv.Key, people[kv.Key], IsLeaderNeeded);
            }
        }

        Title = _localisation.Procedures[(IDType) election.ProcedureID!].Item1;
        Sort ();
    }

    public override bool CanContinue () => ! IsLeaderNeeded || IsSelected ();
}

internal class ElectionPartyViewModel : ElectionTypeViewModel {
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
        }

        foreach (var kv in peopleFactions) {
            if (peopleRoles[kv.Key].All (r => ! election.FilterIDs.Contains (r))) {
                int partyIdx;
                IDType partyId;

                if (partiesUnassigned.Count > 0) {
                    partyIdx = _random.Next (partiesUnassigned.Count);
                    partyId = partiesUnassigned[partyIdx];
                    partiesUnassigned.RemoveAt (partyIdx);
                } else {
                    partyIdx = _random.Next (partiesActive.Count);
                    partyId = partyIds[partyIdx];
                }

                PeopleFactionsNew[kv.Key] = (partyId, kv.Value.Item2);
                AddPerson (partyId, kv.Key, people[kv.Key], IsLeaderNeeded);
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
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, [], localisation, true) {
        _targetIds = [election.TargetID];
        TryAddFaction (election.TargetID, "Candidates");

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
        Localisation localisation,
        List<string> people
    ) : base (peopleRoles, [], localisation, ! election.IsRandom) {
        _targetIds = [election.TargetID];
        _isRandom = election.IsRandom;

        if (_isRandom) {
            List<IDType> peopleIds = [];

            foreach (var kv in peopleRoles) {
                if (kv.Value.All (r => !election.FilterIDs.Contains (r))) {
                    peopleIds.Add (kv.Key);
                }
            }

            int personIdx = _random.Next (peopleIds.Count);
            IDType personId = peopleIds[personIdx];

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
