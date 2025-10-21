namespace CongressCucuta.Core.Procedures;

/*
 * Activated once at the beginning of a simulation
 * It can only be activated again through a Procedure
 *
 * effects: populated
 * action: Election*, Permissions*
 */
public class ProcedureImmediate : Procedure {
    public ProcedureImmediate (IDType id, Effect[] effects) : base (id, effects) {
        foreach (Effect e in effects) {
            switch (e.Type) {
                case Effect.EffectType.ElectionRegion:
                case Effect.EffectType.ElectionParty:
                case Effect.EffectType.ElectionNominated:
                case Effect.EffectType.ElectionAppointed:
                    break;
                case Effect.EffectType.PermissionsCanVote:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureImmediate ID {id}: PermissionsCanSpeak Target must be populated");
                    }

                    break;
                case Effect.EffectType.PermissionsVotes:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureImmediate ID {id}: PermissionsCanSpeak Target must be populated");
                    }

                    break;
                case Effect.EffectType.PermissionsCanSpeak:
                    if (e.TargetIDs.Length == 0) {
                        throw new ArgumentException ($"ProcedureImmediate ID {id}: PermissionsCanSpeak Target must be populated");
                    }

                    break;
                default:
                    throw new ArgumentException ($"ProcedureImmediate ID {id}: Action must be Election* or Permissions*");
            }
        }
    }

    public override EffectBundle? YieldEffects (IDType ballotId) => new (Effects);

    public override string ToString (Simulation simulation, ref readonly Localisation localisation) {
        List<string> result = [localisation.Procedures[ID].Item1];

        foreach (Effect e in Effects) {
            string effect = e.ToString (simulation, in localisation);

            /*
             * Permissions* is also used with ProcedureTargeted
             * There, it's indented thrice to account for the Ballot Filter (and Role Target)
             * Here, there's no Ballot Filter, so it's outdented
             */
            if (
                e.Type is Effect.EffectType.PermissionsCanVote
                or Effect.EffectType.PermissionsVotes
                or Effect.EffectType.PermissionsCanSpeak
            ) {
                effect = StringLineFormatter.Outdent (effect);
            }

            result.Add (effect);
        }

        return string.Join ('\n', result);
    }
}
