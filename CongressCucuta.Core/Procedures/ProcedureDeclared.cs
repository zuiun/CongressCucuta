using System.Diagnostics;

namespace CongressCucuta.Core.Procedures;

/*
 * Activates declaratively
 *
 * effects: at least one
 * action: Election*, Ballot*
 * declarerIds: Role (declarer)
 */
public class ProcedureDeclared : Procedure {
    public Confirmation Confirm { get; }
    // Filters Roles (empty: every, populated: specified)
    public IDType[] DeclarerIDs { get; }

    public ProcedureDeclared (
        IDType id,
        Effect[] effects,
        Confirmation confirm,
        IDType[] declarerIds
    ) : base (id, effects) {
        bool isPass = false;
        bool isFail = false;

        foreach (Effect e in effects) {
            switch (e.Type) {
                case Effect.EffectType.CurrencyAdd:
                case Effect.EffectType.CurrencySubtract:
                case Effect.EffectType.ElectionRegion:
                case Effect.EffectType.ElectionParty:
                case Effect.EffectType.ElectionNominated:
                case Effect.EffectType.ElectionAppointed:
                case Effect.EffectType.BallotLimit:
                    break;
                case Effect.EffectType.BallotPass:
                    if (isFail) {
                        throw new ArgumentException ($"ProcedureDeclared ID {id}: Cannot have both BallotPass and BallotFail");
                    } else {
                        isPass = true;
                    }

                    break;
                case Effect.EffectType.BallotFail:
                    if (isPass) {
                        throw new ArgumentException ($"ProcedureDeclared ID {id}: Cannot have both BallotPass and BallotFail");
                    } else {
                        isFail = true;
                    }

                    break;
                default:
                    throw new ArgumentException ($"ProcedureDeclared ID {id}: Action must be Election* or VotersLimit");
            }
        }

        Confirm = confirm;
        DeclarerIDs = declarerIds;
    }

    private string DeclarerToString (ref readonly Localisation localisation) {
        return DeclarerIDs.Length > 0
            ? string.Join (
                ", ",
                localisation.Roles.Where (k => DeclarerIDs.Contains (k.Key))
                    .Select (k => k.Value.Item2)
            ) + ":"
            : "Everyone:";
    }

    private string ConfirmationToString (ref readonly Localisation localisation) {
        HashSet<string> GetCurrencies (ref readonly Localisation localisation) {
            HashSet<string> currencies = [];

            foreach (IDType d in DeclarerIDs) {
                if (
                    d == Role.MEMBER
                    || d == Role.HEAD_GOVERNMENT
                    || d == Role.HEAD_STATE
                ) {
                    currencies.Add (localisation.Currencies[Currency.STATE]);
                } else if (d == Role.LEADER_PARTY) {
                    currencies.Add (localisation.Currencies[Currency.PARTY]);
                } else if (d == Role.LEADER_REGION) {
                    currencies.Add (localisation.Currencies[Currency.REGION]);
                } else {
                    currencies.Add (localisation.Currencies[d]);
                }
            }

            return currencies;
        }

        switch (Confirm.Type) {
            case Confirmation.ConfirmationType.Always:
                return "Always";
            case Confirmation.ConfirmationType.DivisionChamber:
                return "Division of chamber";
            case Confirmation.ConfirmationType.CurrencyValue: {
                HashSet<string> currencies = GetCurrencies (in localisation);

                return $"Can spend {Confirm.Value} {string.Join (", ", currencies)}";
            }
            case Confirmation.ConfirmationType.DiceValue:
                return $"Dice roll greater than or equal to {Confirm.Value}";
            case Confirmation.ConfirmationType.DiceCurrency: {
                HashSet<string> currencies = GetCurrencies (in localisation);

                return $"Can spend dice roll {string.Join (", ", currencies)}";
            }
            case Confirmation.ConfirmationType.DiceAdversarial: {
                string dice = "Declarer's dice roll greater than or equal to defender's dice roll";

                if (localisation.Currencies.Count > 0) {
                    HashSet<string> currencies = GetCurrencies (in localisation);

                    return $"Can spend declarer's dice roll {string.Join (", ", currencies)}\n{dice}";
                } else {
                    return dice;
                }
            }
            default:
                throw new UnreachableException ();
        }
    }

    public override EffectBundle? YieldEffects (IDType ballotId) => new (Effects, Confirmation: Confirm);

    public override string ToString (Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];
        string declarer = StringLineFormatter.Indent (DeclarerToString (in localisation), 1);
        string canDeclare = StringLineFormatter.Indent ("Can declare if:", 2);
        string[] confirmation = ConfirmationToString (in localisation).Split ('\n');

        result.Add (declarer);
        result.Add (canDeclare);
        result.AddRange (confirmation.Select (c => StringLineFormatter.Indent (c, 3)));

        foreach (Effect e in Effects) {
            string action = e.ToString (simulation, in localisation);

            /*
             * Currency* is also used with ProcedureTargeted
             * There, it's indented twice/thrice to account for the Ballot Filter (and possibly Faction Target)
             * Here, there's no Ballot Filter, so it's outdented
             */
            if (e.Type is Effect.EffectType.CurrencyAdd or Effect.EffectType.CurrencySubtract) {
                action = StringLineFormatter.Outdent (action);
            }

            result.Add (action);
        }

        return string.Join ('\n', result);
    }
}
