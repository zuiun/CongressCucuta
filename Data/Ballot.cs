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
            CreateParty, // Targets Factions
            DissolveParty, // Targets Factions
            // Regions are not intended to change
            RemoveProcedure, // Targets Procedures
            ReplaceProcedure, // Targets Procedures [original, new]
            // Action *Currency should only Target same-name/-category Currencies
            AddCurrency, // Targets Currencies (Faction ID, REGION, PARTY, STATE)
            SubtractCurrency, // Targets Currencies (Faction ID, REGION, PARTY, STATE)
        }

        public ActionType Action { get; }
        public List<IDType> TargetIDs { get; }
        public byte? Value { get; }

        [JsonConstructor]
        public Effect (ActionType action, List<IDType> targetIds, byte? value = null) {
            if (targetIds.Count == 0) {
                throw new ArgumentException ("Target IDs must be populated", nameof (targetIds));
            }

            if (action is ActionType.ReplaceProcedure && targetIds.Count != 2) {
                throw new ArgumentException ("Target IDs must have two IDs for Action ReplaceProcedure", nameof (targetIds));
            }

            Action = action;
            TargetIDs = targetIds;
            Value = value;
        }

        public string ToString (ref readonly Simulation simulation, ref readonly Localisation localisation) {
            switch (Action) {
                case ActionType.CreateParty: {
                    List<string> parties = [];

                    foreach (IDType t in TargetIDs) {
                        parties.Add (localisation.GetFactionAndAbbreviation (t));
                    }

                    return $"Create {string.Join (", ", parties)}";
                }
                case ActionType.DissolveParty: {
                    List<string> parties = [];

                    foreach (IDType t in TargetIDs) {
                        parties.Add (localisation.GetFactionAndAbbreviation (t));
                    }

                    return $"Dissolve {string.Join (", ", parties)}";
                }
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
                case ActionType.AddCurrency: {
                    string currency = localisation.Currencies[TargetIDs[0]];

                    if (TargetIDs[0] == Currency.STATE) {
                        return $"Gain {Value} {currency}";
                    } else if (TargetIDs[0] == Currency.PARTY) {
                        return $"Every {localisation.Party.Item1}:\n{StringLineFormatter.Indent ($"Gains {Value} {currency}", 1)}";
                    } else if (TargetIDs[0] == Currency.REGION) {
                        return $"Every {localisation.Region.Item1}:\n{StringLineFormatter.Indent ($"Gains {Value} {currency}", 1)}";
                    } else {
                        List<string> owners = [];
    
                        foreach (IDType t in TargetIDs) {
                            owners.Add (localisation.GetFactionOrAbbreviation (t));
                        }

                        return $"{string.Join (", ", owners)}:\n{StringLineFormatter.Indent ($"Gain {Value} {currency}", 1)}";
                    }

                }
                case ActionType.SubtractCurrency: {
                    string currency = localisation.Currencies[TargetIDs[0]];

                    if (TargetIDs[0] == Currency.STATE) {
                        return $"Lose {Value} {currency}";
                    } else if (TargetIDs[0] == Currency.PARTY) {
                        return $"Every {localisation.Party.Item1}:\n{StringLineFormatter.Indent ($"Loses {Value} {currency}", 1)}";
                    } else if (TargetIDs[0] == Currency.REGION) {
                        return $"Every {localisation.Region.Item1}:\n{StringLineFormatter.Indent ($"Loses {Value} {currency}", 1)}";
                    } else {
                        List<string> owners = [];

                        foreach (IDType t in TargetIDs) {
                            owners.Add (localisation.GetFactionOrAbbreviation (t));
                        }

                        return $"{string.Join (", ", owners)}:\n{StringLineFormatter.Indent ($"Lose {Value} {currency}", 1)}";
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
