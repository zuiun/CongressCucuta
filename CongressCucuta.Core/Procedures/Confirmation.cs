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
}
