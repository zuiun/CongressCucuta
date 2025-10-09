using System.Diagnostics;

namespace congress_cucuta.Data;

internal readonly record struct Election : IComparable<Election> {
    internal enum ElectionType {
        ShuffleRemove, // Filters: Parties (out)
        ShuffleAdd, // Filters: Parties (in)
        Region, // Filters: Roles (excluded)
        Party, // Filters: Roles (excluded)
        Nominated, // Elects: Role; Filters: Roles (excluded)
        Appointed, // Elects: Role; Filters: Roles (excluded)
    }

    public IDType? ProcedureID { get; }
    public ElectionType Type { get; }
    public IDType TargetID { get; }
    public IDType[] FilterIDs { get; } = [];
    public bool IsRandom { get; } = false;

    public Election (ElectionType type, IDType[] filterIds) {
        if (filterIds.Length == 0) {
            throw new ArgumentException ("Election must have Filter", nameof (filterIds));
        }

        Type = type;
        FilterIDs = filterIds;
    }

    public Election (IDType procedureId, Procedure.Effect effect, bool isImmediate) {
        ProcedureID = procedureId;

        switch (effect.Action) {
            case Procedure.Effect.ActionType.ElectionRegion:
                Type = ElectionType.Region;
                FilterIDs = effect.TargetIDs;
                break;
            case Procedure.Effect.ActionType.ElectionParty:
                Type = ElectionType.Party;
                FilterIDs = effect.TargetIDs;
                break;
            case Procedure.Effect.ActionType.ElectionNominated:
                Type = ElectionType.Nominated;
                TargetID = effect.TargetIDs[0];

                if (effect.TargetIDs.Length > 1) {
                    FilterIDs = effect.TargetIDs[1 ..];
                }

                break;
            case Procedure.Effect.ActionType.ElectionAppointed:
                Type = ElectionType.Appointed;
                TargetID = effect.TargetIDs[0];

                if (effect.TargetIDs.Length > 1) {
                    FilterIDs = effect.TargetIDs[1 ..];
                }

                if (isImmediate) {
                    IsRandom = true;
                }

                break;
            default:
                throw new NotSupportedException ();
        }
    }

    public int CompareTo (Election other) => Type.CompareTo (other.Type);
}
