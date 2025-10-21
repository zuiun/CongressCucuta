using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core.Conditions;

public readonly record struct AndCondition (params ICondition[] Conditions) : ICondition {
    public bool Evaluate (SimulationContext context) {
        foreach (ICondition c in Conditions) {
            if (!c.Evaluate (context)) {
                return false;
            }
        }

        return true;
    }

    public string ToString (ref readonly Localisation localisation) {
        List<string> conditions = [];

        foreach (ICondition c in Conditions) {
            conditions.Add (c.ToString (in localisation));
        }

        return string.Join (" and ", conditions);
    }

    public bool? YieldBallotVote () => null;
}
