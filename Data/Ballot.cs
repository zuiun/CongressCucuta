using System.Diagnostics;
using System.Text.Json.Serialization;
using congress_cucuta.Converters;

namespace congress_cucuta.Data;

internal class Ballot (
    IDType id,
    Ballot.Result passResult,
    Ballot.Result failResult,
    bool isIncident = false
) : IID {
    internal readonly record struct Effect {
        internal enum ActionType {
            // Regions are not intended to change
            FoundParty, // Targets Factions, elects PARTY_LEADER (if present)
            DissolveParty, // Targets Factions, elects PARTY_LEADER (if present)
            //ReplaceParty, // Targets Factions (original, new), retains PARTY_LEADER (if present)
            RemoveProcedure, // Targets Procedures
            ReplaceProcedure, // Targets Procedures (original, new)
            ModifyCurrency, // Targets Currencies (Faction ID [only same-category], REGION [one], PARTY [one], STATE [one])
        }

        public ActionType Action { get; }
        public IDType[] TargetIDs { get; }
        public sbyte Value { get; }

        [JsonConstructor]
        public Effect (ActionType action, IDType[] targetIds, sbyte value = 0) {
            if (targetIds.Length == 0) {
                throw new ArgumentException ("Target IDs must be populated", nameof (targetIds));
            }

            //if (action is ActionType.ReplaceParty && targetIds.Length != 2) {
            //    throw new ArgumentException ("Target IDs must have two IDs for Action ReplaceParty", nameof (targetIds));
            //}

            if (action is ActionType.ReplaceProcedure && targetIds.Length != 2) {
                throw new ArgumentException ("Target IDs must have two IDs for Action ReplaceProcedure", nameof (targetIds));
            }

            if (action is ActionType.ModifyCurrency && value == 0) {
                throw new ArgumentException ("Value must be non-zero for Action ModifyCurrency", nameof (targetIds));
            }

            Action = action;
            TargetIDs = targetIds;
            Value = value;
        }

        public string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
            switch (Action) {
                case ActionType.FoundParty: {
                    List<string> parties = [];

                    foreach (IDType t in TargetIDs) {
                        parties.Add (localisation.GetFactionAndAbbreviation (t));
                    }

                    return $"Found {string.Join (", ", parties)}";
                }
                case ActionType.DissolveParty: {
                    List<string> parties = [];

                    foreach (IDType t in TargetIDs) {
                        parties.Add (localisation.GetFactionAndAbbreviation (t));
                    }

                    return $"Dissolve {string.Join (", ", parties)}";
                }
                //case ActionType.ReplaceParty: {
                //    string partyOriginal = localisation.GetFactionAndAbbreviation (TargetIDs[0]);
                //    string partyNew = localisation.GetFactionAndAbbreviation (TargetIDs[1]);

                //    return $"Replace {partyOriginal} with {partyNew}";
                //}
                case ActionType.RemoveProcedure: {
                    List<string> procedures = [];

                    foreach (IDType t in TargetIDs) {
                        procedures.Add (localisation.Procedures[t].Item1);
                    }

                    return $"Remove {string.Join (", ", procedures)}";
                }
                case ActionType.ReplaceProcedure: {
                    List<string> result = [];
                    string procedureOriginal = localisation.Procedures[TargetIDs[0]].Item1;
                    string procedureNew = localisation.Procedures[TargetIDs[1]].Item1;
                    string action = $"Replace {procedureOriginal} with {procedureNew}:";
                    Procedure procedure = simulation.ProceduresSpecial[TargetIDs[1] - simulation.ProceduresGovernmental.Count];
                    string procedureFull = procedure.ToString (in simulation, in localisation);
                    string[] procedureSplit = procedureFull.Split ('\n');

                    result.Add (action);
                    result.AddRange (procedureSplit[1 ..]);
                    return string.Join ('\n', result);
                }
                case ActionType.ModifyCurrency: {
                    string currency = localisation.Currencies[TargetIDs[0]];
                    string action = Value > 0 ? "Gain" : "Lose";
                    sbyte value = Math.Abs (Value);

                    if (TargetIDs[0] == Currency.STATE) {
                        return $"{action} {value} {currency}";
                    } else if (TargetIDs[0] == Currency.PARTY) {
                        return $"Every {localisation.Party.Item1}:\n{StringLineFormatter.Indent ($"{action}s {value} {currency}", 1)}";
                    } else if (TargetIDs[0] == Currency.REGION) {
                        return $"Every {localisation.Region.Item1}:\n{StringLineFormatter.Indent ($"{action}s {value} {currency}", 1)}";
                    } else {
                        List<string> owners = [];
    
                        foreach (IDType t in TargetIDs) {
                            owners.Add (localisation.GetFactionOrAbbreviation (t));
                        }

                        return $"{string.Join (", ", owners)}:\n{StringLineFormatter.Indent ($"{action} {value} {currency}", 1)}";
                    }
                }
                default:
                    throw new UnreachableException ();
            }
        }
    }

    internal readonly record struct Result (List <Effect> Effects, List<Link<Ballot>> Links);

    public IDType ID => id;
    public bool IsIncident => isIncident;
    public Result PassResult => passResult;
    public Result FailResult => failResult;
}
