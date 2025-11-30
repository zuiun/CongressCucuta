using System.Text.Json.Serialization;

namespace CongressCucuta.Core.Procedures;

public abstract class Procedure : IID {
    public readonly record struct Effect {
        public enum EffectType {
            /*
             * Adds Value votes for the passage of Filter Ballot
             *
             * Targeted
             */
            VotePassAdd,
            /*
             * Adds Value votes for the failure of Filter Ballot
             *
             * Targeted
             */
            VoteFailAdd,
            /*
             * Causes Filter Ballot to require a two-thirds majority to pass
             *
             * Targeted
             */
            VotePassTwoThirds,
            /*
             * Adds Value to Target Currency during Filter Ballot
             * Can't mix every scope with individual scopes
             *
             * Targeted, Declared
             * Targets Factions (populated: specified), Currencies (empty: STATE or declarer)
             */
            CurrencyAdd,
            /*
             * Subtracts Value from Target Currency during Filter Ballot
             * Can't mix every scope with individual scopes
             *
             * Targeted, Declared
             * Targets Factions (populated: specified), Currencies (empty: STATE or declarer)
             */
            CurrencySubtract,
            /*
             * Initialises Currencies
             * This is only used for display and is handled elsewhere 
             *
             * Targeted
             */
            CurrencyInitialise,
            /*
             * Activates Target ProcedureImmediate during Filter Ballot
             *
             * Targeted
             * Targets Procedure (populated: specified)
             */
            ProcedureActivate,
            /*
             * Assigns Target Roles to Regions
             * Elects LEADER_REGION if present
             *
             * Immediate, Declared
             * Targets Roles (populated: excluded [only from leadership])
             * Value (0: choose LEADER_REGION, anything else: random LEADER_REGION)
             */
            ElectionRegion,
            /*
             * Assigns Target Roles to Parties
             * Elects LEADER_PARTY if present
             *
             * Immediate, Declared
             * Targets Roles (populated: excluded)
             * Value (0: choose LEADER_PARTY, anything else: random LEADER_PARTY)
             */
            ElectionParty,
            /*
             * Elects Target Role
             *
             * Immediate, Declared
             * Targets Roles (populated: specified [first], excluded [remainder])
             */
            ElectionNominated,
            /*
             * Appoints Target Role
             *
             * Immediate, Declared
             * Targets Roles (populated: specified [first], excluded [remainder])
             * Value (0: choose Target, anything else: random Target)
             */
            ElectionAppointed,
            /*
             * Only allows Target Roles or Factions to vote on current Ballot
             *
             * Declared
             * Targets Roles and Factions (empty: declarer, populated: specified [whichever is found at that ID, with Faction taking priority])
             */
            BallotLimit,
            /*
             * Immediately passes current Ballot
             *
             * Declared
             */
            BallotPass,
            /*
             * Immediately fails current Ballot
             *
             * Declared
             */
            BallotFail,
            /*
             * Sets Target Role voting Permissions to Value during Filter Ballot (when Targeted)
             *
             * Immediate, Targeted
             * Targets Roles (empty: random [person, only Targeted], populated: specified)
             * Value (0: false, other: true)
             */
            PermissionsCanVote,
            /*
             * Sets Target Role votes Permissions to Value during Filter Ballot (when Targeted)
             *
             * Immediate, Targeted
             * Targets Roles (empty: random [person, only Targeted], populated: specified)
             */
            PermissionsVotes,
            /*
             * Sets Target Role speaking Permissions to Value during Filter Ballot (when Targeted)
             *
             * Immediate, Targeted
             * Targets Roles (empty: random [person, only Targeted], populated: specified)
             * Value (0: false, other: true)
             */
            PermissionsCanSpeak,
        }

        public readonly EffectType Type { get; }
        public readonly IDType[] TargetIDs { get; }
        public readonly byte Value { get; }

        [JsonConstructor]
        public Effect (EffectType type, IDType[] targetIDs, byte value = 0) {
            switch (type) {
                case EffectType.ProcedureActivate:
                    if (targetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureActivate Target must be populated", nameof (targetIDs));
                    }

                    break;
                case EffectType.CurrencyAdd:
                case EffectType.CurrencySubtract:
                    if (targetIDs.Contains (Currency.STATE)) {
                        throw new ArgumentException ($"Currency* Target cannot be State");
                    }

                    if (targetIDs.Contains (Currency.REGION) && targetIDs.Length != 1) {
                        throw new ArgumentException ($"Currency* Target cannot include both Region and other targets");
                    }

                    if (targetIDs.Contains (Currency.PARTY) && targetIDs.Length != 1) {
                        throw new ArgumentException ($"Currency* Target cannot include both Party and other targets");
                    }

                    break;
                case EffectType.ElectionNominated:
                    if (targetIDs.Length == 0) {
                        throw new ArgumentException ($"ElectionNominated Target must be populated", nameof (targetIDs));
                    }

                    break;
                case EffectType.ElectionAppointed:
                    if (targetIDs.Length == 0) {
                        throw new ArgumentException ($"ElectionAppointed Target must be populated", nameof (targetIDs));
                    }

                    break;
            }

            Type = type;
            TargetIDs = targetIDs;
            Value = value;
        }

        private static string TargetToString (Effect effect, ref readonly Localisation localisation) {
            switch (effect.Type) {
                case EffectType.CurrencyAdd:
                case EffectType.CurrencySubtract:
                    if (effect.TargetIDs[0] == Currency.REGION) {
                        return $"Every {localisation.Region.Item1}:";
                    } else if (effect.TargetIDs[0] == Currency.PARTY) {
                        return $"Every {localisation.Party.Item1}:";
                    } else {
                        List<string> factions = [];
                        var regionsIter = localisation.Regions.Keys.Intersect (effect.TargetIDs);
                        var partiesIter = localisation.Parties.Keys.Intersect (effect.TargetIDs);

                        foreach (var r in regionsIter) {
                            factions.Add (localisation.GetFactionOrAbbreviation (r));
                        }

                        foreach (var p in partiesIter) {
                            factions.Add (localisation.GetFactionOrAbbreviation (p));
                        }

                        return $"{string.Join (", ", factions)}:";
                    }
                case EffectType.ProcedureActivate:
                    return localisation.Procedures[effect.TargetIDs[0]].Item1;
                case EffectType.ElectionRegion:
                case EffectType.ElectionParty: {
                    if (effect.TargetIDs.Length > 0) {
                        List<string> excluded = [];

                        foreach (IDType t in effect.TargetIDs) {
                            excluded.Add (localisation.Roles[t].Item2);
                        }

                        return $"Everyone except {string.Join (", ", excluded)}:";
                    } else {
                        return "Everyone:";
                    }
                }
                case EffectType.ElectionNominated:
                case EffectType.ElectionAppointed: {
                    string target = localisation.Roles[effect.TargetIDs[0]].Item1;

                    if (effect.TargetIDs.Length > 1) {
                        List<string> excluded = [];

                        for (byte i = 1; i < effect.TargetIDs.Length; ++i) {
                            excluded.Add (localisation.Roles[effect.TargetIDs[i]].Item2);
                        }

                        return $"{target}\nEveryone except {string.Join (", ", excluded)}:";
                    } else {
                        return $"{target}\nEveryone:";
                    }
                }
                case EffectType.BallotLimit: {
                    if (effect.TargetIDs.Length > 0) {
                        List<string> excluded = ["Declarer"];

                        foreach (IDType t in effect.TargetIDs) {
                            excluded.Add (localisation.Roles[t].Item2);
                        }

                        return $"Everyone except {string.Join (", ", excluded)}:";
                    } else {
                        return "Everyone except Declarer:";
                    }
                }
                case EffectType.PermissionsCanVote:
                case EffectType.PermissionsVotes:
                case EffectType.PermissionsCanSpeak: {
                    if (effect.TargetIDs.Length > 0) {
                        List<string> specified = [];

                        foreach (IDType t in effect.TargetIDs) {
                            specified.Add (localisation.Roles[t].Item2);
                        }

                        return $"{string.Join (", ", specified)}:";
                    } else {
                        return "Random person:";
                    }
                }
                default:
                    throw new NotSupportedException ();
            }
        }

        public string ToString (Simulation simulation, ref readonly Localisation localisation) {
            List<string> result = [];

            switch (Type) {
                case EffectType.VotePassAdd: {
                    string action = StringLineFormatter.Indent ($"Gains {Value} vote(s) in favour", 2);

                    result.Add (action);
                    break;
                }
                case EffectType.VoteFailAdd: {
                    string action = StringLineFormatter.Indent ($"Gains {Value} vote(s) in opposition", 2);

                    result.Add (action);
                    break;
                }
                case EffectType.VotePassTwoThirds: {
                    string action = StringLineFormatter.Indent ($"Needs a two-thirds majority to pass", 2);

                    result.Add (action);
                    break;
                }
                case EffectType.CurrencyAdd: {
                    if (TargetIDs.Length > 0) {
                        string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                        string action = StringLineFormatter.Indent ($"Gains {Value} {localisation.Currencies[TargetIDs[0]]}", 3);

                        result.Add (target);
                        result.Add (action);
                    } else {
                        string action = StringLineFormatter.Indent ($"Gain {Value} {localisation.Currencies[Currency.STATE]}", 2);

                        result.Add (action);
                    }

                    break;
                }
                case EffectType.CurrencySubtract: {
                    if (TargetIDs.Length > 0) {
                        string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                        string action = StringLineFormatter.Indent ($"Loses {Value} {localisation.Currencies[TargetIDs[0]]}", 3);

                        result.Add (target);
                        result.Add (action);
                    } else {
                        string action = StringLineFormatter.Indent ($"Lose {Value} {localisation.Currencies[Currency.STATE]}", 2);

                        result.Add (action);
                    }

                    break;
                }
                case EffectType.CurrencyInitialise: {
                    if (simulation.CurrenciesValues.Keys.Any (c => c.ID == Currency.STATE)) {
                        string currencyState = localisation.Currencies[Currency.STATE];
                        sbyte value = simulation.CurrenciesValues.Where (k => k.Key.ID == Currency.STATE)
                                .Select (k => k.Value)
                                .First ();

                        result.Add (StringLineFormatter.Indent ($"{currencyState} begins at {value}", 1));
                    }

                    SortedList<sbyte, List<string>> currencyRegions = [];
                    string currencyRegion = string.Empty;

                    foreach (Faction region in simulation.Regions) {
                        if (simulation.CurrenciesValues.Any (k => k.Key.ID == region.ID)) {
                            sbyte value = simulation.CurrenciesValues.Where (k => k.Key.ID == region.ID)
                                .Select (k => k.Value)
                                .First ();

                            currencyRegion = localisation.Currencies[region.ID];

                            if (currencyRegions.TryGetValue (value, out var regions)) {
                                regions.Add (localisation.Regions[region.ID].Item1);
                            } else {
                                currencyRegions[value] = [localisation.Regions[region.ID].Item1];
                            }
                        }
                    }

                    if (currencyRegions.Count == 1) {
                        sbyte value = currencyRegions.Keys.First ();

                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Region.Item1}:", 1));
                        result.Add (StringLineFormatter.Indent ($"{currencyRegion} begins at {value}", 2));
                    } else if (currencyRegions.Count > 1) {
                        foreach (var pair in currencyRegions.Reverse ()) {
                            string regions = string.Join (", ", pair.Value);

                            result.Add (StringLineFormatter.Indent ($"{regions}:", 1));
                            result.Add (StringLineFormatter.Indent ($"{currencyRegion} begins at {pair.Key}", 2));
                        }
                    }

                    SortedList<sbyte, List<string>> currencyParties = [];
                    string currencyParty = string.Empty;

                    foreach (Faction party in simulation.Parties) {
                        if (simulation.CurrenciesValues.Any (k => k.Key.ID == party.ID)) {
                            sbyte value = simulation.CurrenciesValues.Where (k => k.Key.ID == party.ID)
                                .Select (k => k.Value)
                                .First ();

                            currencyParty = localisation.Currencies[party.ID];

                            if (currencyParties.TryGetValue (value, out var parties)) {
                                parties.Add (localisation.Parties[party.ID].Item1);
                            } else {
                                currencyParties[value] = [localisation.Parties[party.ID].Item1];
                            }
                        }
                    }

                    if (currencyParties.Count == 1) {
                        sbyte value = currencyParties.Keys.First ();

                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Party.Item1}:", 1));
                        result.Add (StringLineFormatter.Indent ($"{currencyParty} begins at {value}", 2));
                    } else if (currencyParties.Count > 1) {
                        foreach (var pair in currencyParties.Reverse ()) {
                            string parties = string.Join (", ", pair.Value);

                            result.Add (StringLineFormatter.Indent ($"{parties}:", 1));
                            result.Add (StringLineFormatter.Indent ($"{currencyParty} begins at {pair.Key}", 2));
                        }
                    }

                    break;
                }
                case EffectType.ProcedureActivate: {
                    string target = TargetToString (this, in localisation);
                    string action = StringLineFormatter.Indent ($"Hold new {target}", 2);

                    result.Add (action);
                    break;
                }
                case EffectType.ElectionRegion: {
                    string target = StringLineFormatter.Indent ("Everyone:", 1);
                    string action = StringLineFormatter.Indent ($"Randomly aligns with a {localisation.Region.Item1}", 2);
                    string candidates = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    bool isRandom = Value > 0;

                    result.Add (target);
                    result.Add (action);

                    if (localisation.Roles.TryGetValue (Role.LEADER_REGION, out (string, string) leader)) {
                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Region.Item1}:", 1));

                        if (isRandom) {
                            result.Add (StringLineFormatter.Indent ($"Randomly appoints {leader.Item1}", 2));
                        } else {
                            result.Add (StringLineFormatter.Indent ($"Elects {leader.Item1}", 2));
                        }

                        result.Add (candidates);
                        result.Add (StringLineFormatter.Indent ("Can be nominated", 3));
                    }

                    break;
                }
                case EffectType.ElectionParty: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 1);
                    string action = StringLineFormatter.Indent ($"Randomly aligns with a {localisation.Party.Item1}", 2);
                    bool isRandom = Value > 0;

                    result.Add (target);
                    result.Add (action);

                    if (localisation.Roles.TryGetValue (Role.LEADER_PARTY, out (string, string) leader)) {
                        result.Add (StringLineFormatter.Indent ($"Every {localisation.Party.Item1}:", 1));

                        if (isRandom) {
                            result.Add (StringLineFormatter.Indent ($"Randomly appoints {leader.Item1}", 2));
                        } else {
                            result.Add (StringLineFormatter.Indent ($"Elects {leader.Item1}", 2));
                        }
                    }

                    break;
                }
                case EffectType.ElectionNominated: {
                    string[] targets = TargetToString (this, in localisation).Split ('\n');
                    string target = targets[0];
                    string candidates = targets[1];

                    result.Add (StringLineFormatter.Indent ($"Elect {target}:", 1));
                    result.Add (StringLineFormatter.Indent (candidates, 2));
                    result.Add (StringLineFormatter.Indent ("Can be nominated", 3));
                    break;
                }
                case EffectType.ElectionAppointed: {
                    string[] targets = TargetToString (this, in localisation).Split ('\n');
                    string target = targets[0];
                    string candidates = targets[1];
                    bool isRandom = Value > 0;

                    if (isRandom) {
                        result.Add (StringLineFormatter.Indent ($"Randomly appoint {target}:", 1));
                    } else {
                        result.Add (StringLineFormatter.Indent ($"Appoint {target}:", 1));
                    }

                    result.Add (StringLineFormatter.Indent (candidates, 2));
                    result.Add (StringLineFormatter.Indent ("Can be appointed", 3));
                    break;
                }
                case EffectType.BallotLimit: {
                    string filter = StringLineFormatter.Indent ("Current ballot:", 1);
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action = StringLineFormatter.Indent ("Cannot vote", 3);

                    result.Add (filter);
                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case EffectType.BallotPass: {
                    string filter = StringLineFormatter.Indent ("Current ballot:", 1);
                    string action = StringLineFormatter.Indent ("Immediately passes", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case EffectType.BallotFail: {
                    string filter = StringLineFormatter.Indent ("Current ballot:", 1);
                    string action = StringLineFormatter.Indent ("Immediately fails", 2);

                    result.Add (filter);
                    result.Add (action);
                    break;
                }
                case EffectType.PermissionsCanVote: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action;

                    if (Value > 0) {
                        action = StringLineFormatter.Indent ("Can vote", 3);
                    } else {
                        action = StringLineFormatter.Indent ("Cannot vote", 3);
                    }

                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case EffectType.PermissionsVotes: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action = StringLineFormatter.Indent ($"Gain {Value} vote(s)", 3);

                    result.Add (target);
                    result.Add (action);
                    break;
                }
                case EffectType.PermissionsCanSpeak: {
                    string target = StringLineFormatter.Indent (TargetToString (this, in localisation), 2);
                    string action;

                    if (Value > 0) {
                        action = StringLineFormatter.Indent ("Can speak", 3);
                    } else {
                        action = StringLineFormatter.Indent ("Cannot speak", 3);
                    }

                    result.Add (target);
                    result.Add (action);
                    break;
                }
            }

            return string.Join ('\n', result);
        }
    }

    public readonly record struct EffectBundle (Effect[] Effects, Confirmation? Confirmation = null);

    public Procedure (IDType id, Effect[] effects, bool isActiveStart = true) {
        if (effects.Length == 0) {
            throw new ArgumentException ($"ProcedureImmediate ID {id}: ProcedureImmediate must have Effect", nameof (effects));
        }

        ID = id;
        Effects = effects;
        IsActiveStart = isActiveStart;
    }

    public IDType ID { get; }
    public Effect[] Effects { get; }
    // Only used for ProcedureTargeted
    public bool IsActiveStart { get; }

    public abstract EffectBundle? YieldEffects (IDType ballotId);
    public abstract string ToString (Simulation simulation, ref readonly Localisation localisation);
}
