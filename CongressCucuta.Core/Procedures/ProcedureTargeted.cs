namespace CongressCucuta.Core.Procedures;

/*
 * Activates on filtered Ballots
 *
 * effects: at least one
 * action: Vote*, Currency*, ProcedureActivate, Permissions*
 * filter: Ballot
 */
public class ProcedureTargeted : Procedure {
    // Filters Ballots (empty: every, populated: specified)
    public IDType[] BallotIDs { get; }

    public ProcedureTargeted (
        IDType id,
        Effect[] effects,
        IDType[] ballotIds,
        bool isActiveStart = true
    ) : base (id, effects, isActiveStart) {
        foreach (Effect e in effects) {
            switch (e.Type) {
                case Effect.EffectType.VotePassAdd:
                case Effect.EffectType.VoteFailAdd:
                case Effect.EffectType.VotePassTwoThirds:
                case Effect.EffectType.CurrencyAdd:
                case Effect.EffectType.CurrencySubtract:
                case Effect.EffectType.CurrencyInitialise:
                case Effect.EffectType.ProcedureActivate:
                case Effect.EffectType.PermissionsCanVote:
                case Effect.EffectType.PermissionsVotes:
                case Effect.EffectType.PermissionsCanSpeak:
                    break;
                default:
                    throw new ArgumentException ($"ProcedureTargeted ID {id}: Action must be Vote*, Currency*, ProcedureActivate, or Permissions*");
            }
        }

        BallotIDs = ballotIds;
    }

    private string FilterToString (ref readonly Localisation localisation) {
        if (BallotIDs.Length > 0) {
            var ballotsIter = localisation.Ballots.Where (k => BallotIDs.Contains (k.Key))
                .Select (k => k.Value.Item1);

            return $"{string.Join (", ", ballotsIter)}:";
        } else {
            return "Every Ballot:";
        }
    }

    public override EffectBundle? YieldEffects (IDType ballotId) =>
        BallotIDs.Length == 0 || BallotIDs.Contains (ballotId) ? new EffectBundle (Effects) : null;

    public override string ToString (Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];
        string filter = StringLineFormatter.Indent (FilterToString (in localisation), 1);
        bool isFilterAdded = false;

        foreach (Effect e in Effects) {
            result.Add (e.ToString (simulation, in localisation));

            // This adds the Ballot Filter logically behind CurrencyInitialise
            if (e.Type is Effect.EffectType.CurrencyInitialise) {
                if (Effects.Length > 1) {
                    result.Add (filter);
                }

                isFilterAdded = true;
            }
        }

        // This adds the Ballot Filter logically before Effect (except CurrencyInitialise)
        if (!isFilterAdded) {
            result.Insert (1, filter);
        }

        return string.Join ('\n', result);
    }
}
