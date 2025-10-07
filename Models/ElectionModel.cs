using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal class ElectionModel (Election election) {
    public IDType? ProcedureID = election.ProcedureID;
    public Election.ElectionType Type => election.Type;
    public IDType TargetID => election.TargetID;
    public IDType[] FilterIDs => election.FilterIDs;
    public bool IsRandom => election.IsRandom;
}
