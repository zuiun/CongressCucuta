using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Models;

internal class ElectionModel (ElectionContext election) {
    public IDType? ProcedureID = election.ProcedureID;
    public ElectionContext.ElectionType Type => election.Type;
    public IDType TargetID => election.TargetID;
    public IDType[] FilterIDs => election.FilterIDs;
    public bool IsRandom => election.IsRandom;
}
