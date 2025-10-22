using System.Diagnostics;

namespace CongressCucuta.Core.Procedures;

public readonly record struct Confirmation (Confirmation.ConfirmationType Type, byte Value = 0) {
    /*
     * Always: always succeeds
     * DivisionChamber: simple majority vote, succeeds if vote passes
     * CurrencyValue: succeeds if Currency is higher than Value, Value is subtracted from Currency
     * DiceValue: rolls one dice, succeeds if dice >= Value
     * DiceCurrency: rolls one dice, succeeds if dice <= Currency, dice is subtracted from Currency
     * DiceAdversarial: rolls two die representing declarer and other,
     * succeeds if declarer's dice >= other's dice,
     * declarer's dice is subtracted from declarer's Currency if present
     */
    public enum ConfirmationType {
        Always,
        DivisionChamber,
        CurrencyValue,
        DiceValue,
        DiceCurrency,
        DiceAdversarial,
    }

    public string ToString (IDType[] declarerIds, ref readonly Localisation localisation) {
        HashSet<string> GetCurrencies (ref readonly Localisation localisation) {
            HashSet<string> currencies = [];

            foreach (IDType d in declarerIds) {
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

        switch (Type) {
            case ConfirmationType.Always:
                return "Always";
            case ConfirmationType.DivisionChamber:
                return "Division of chamber";
            case ConfirmationType.CurrencyValue: {
                HashSet<string> currencies = GetCurrencies (in localisation);

                return $"Can spend {Value} {string.Join (", ", currencies)}";
            }
            case ConfirmationType.DiceValue:
                return $"Dice roll greater than or equal to {Value}";
            case ConfirmationType.DiceCurrency: {
                HashSet<string> currencies = GetCurrencies (in localisation);

                return $"Can spend dice roll {string.Join (", ", currencies)}";
            }
            case ConfirmationType.DiceAdversarial: {
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
}
