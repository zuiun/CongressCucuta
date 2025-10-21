using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Core;

/*
 * T indicates which object's ID is referenced
 * It serves no functional purpose and exists only to prevent mixing of different Link<T>
 */
public readonly record struct Link<T> (ICondition Condition, IDType TargetID)
where T : IID {
    public IDType? Resolve (SimulationContext context) => Condition.Evaluate (context) ? TargetID : null;
}
