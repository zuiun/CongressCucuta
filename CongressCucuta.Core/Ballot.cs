using System.Diagnostics;
using System.Text.Json.Serialization;

namespace CongressCucuta.Core;

public readonly record struct Ballot (IDType ID, Ballot.Result Pass, Ballot.Result Fail, bool IsIncident = false) : IID {
    public readonly record struct Effect {
        public enum EffectType {
            // Regions are not intended to change
            FoundParty, // Targets Factions, elects PARTY_LEADER (if present)
            DissolveParty, // Targets Factions, elects PARTY_LEADER (if present)
            //AddProcedure, // Targets Procedures (single)
            //ReplaceParty, // Targets Factions (original, new), retains PARTY_LEADER (if present)
            RemoveProcedure, // Targets Procedures
            ReplaceProcedure, // Targets Procedures (original, new)
            ModifyCurrency, // Targets Currencies (Faction ID, REGION [one], PARTY [one], STATE [one])
        }

        public EffectType Type { get; }
        public IDType[] TargetIDs { get; }
        public sbyte Value { get; }

        [JsonConstructor]
        public Effect (EffectType type, IDType[] targetIds, sbyte value = 0) {
            if (targetIds.Length == 0) {
                throw new ArgumentException ("Target IDs must be populated", nameof (targetIds));
            }

            //if (type is EffectType.ReplaceParty && targetIds.Length != 2) {
            //    throw new ArgumentException ("Target IDs must have two IDs for Action ReplaceParty", nameof (targetIds));
            //}

            if (type is EffectType.ReplaceProcedure && targetIds.Length != 2) {
                throw new ArgumentException ("Target IDs must have two IDs for Action ReplaceProcedure", nameof (targetIds));
            }

            if (type is EffectType.ModifyCurrency && value == 0) {
                throw new ArgumentException ("Value must be non-zero for Action ModifyCurrency", nameof (targetIds));
            }

            Type = type;
            TargetIDs = targetIds;
            Value = value;
        }

        public string ToString (Simulation simulation, ref readonly Localisation localisation) {
            switch (Type) {
                case EffectType.FoundParty: {
                    List<string> parties = [];

                    foreach (IDType t in TargetIDs) {
                        parties.Add (localisation.GetFactionAndAbbreviation (t));
                    }

                    return $"Found {string.Join (", ", parties)}";
                }
                case EffectType.DissolveParty: {
                    List<string> parties = [];

                    foreach (IDType t in TargetIDs) {
                        parties.Add (localisation.GetFactionAndAbbreviation (t));
                    }

                    return $"Dissolve {string.Join (", ", parties)}";
                }
                //case EffectType.ReplaceParty: {
                //    string partyOriginal = localisation.GetFactionAndAbbreviation (TargetIDs[0]);
                //    string partyNew = localisation.GetFactionAndAbbreviation (TargetIDs[1]);

                //    return $"Replace {partyOriginal} with {partyNew}";
                //}
                case EffectType.RemoveProcedure: {
                    List<string> procedures = [];

                    foreach (IDType t in TargetIDs) {
                        procedures.Add (localisation.Procedures[t].Item1);
                    }

                    return $"Remove {string.Join (", ", procedures)}";
                }
                case EffectType.ReplaceProcedure: {
                    List<string> result = [];
                    string procedureOriginal = localisation.Procedures[TargetIDs[0]].Item1;
                    string procedureNew = localisation.Procedures[TargetIDs[1]].Item1;
                    string action;

                    if (procedureOriginal == procedureNew) {
                        action = $"Modify {procedureOriginal}:";
                    } else {
                        action = $"Replace {procedureOriginal} with {procedureNew}:";
                    }

                    int procedureIdx = TargetIDs[1] - simulation.ProceduresGovernmental.Count;
                    string procedure = simulation.ProceduresSpecial[procedureIdx].ToString (simulation, in localisation);
                    string[] effect = procedure.Split ('\n');

                    result.Add (action);
                    result.AddRange (effect[1 ..]);
                    return string.Join ('\n', result);
                }
                case EffectType.ModifyCurrency: {
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

    public readonly record struct Result (List<Effect> Effects, List<Link<Ballot>> Links);

    public static readonly IDType END = byte.MaxValue;
}
