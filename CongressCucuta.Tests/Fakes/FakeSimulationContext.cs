using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Tests.Unit.Fakes;

public class FakeSimulationContext () : SimulationContext (new FakeSimulation ()) {
    //public ConfirmationResult TryConfirmProcedureResult { get; set; }
    //public bool? DeclareProcedureResult { get; set; }
    //public IDType ChooseCurrencyOwnerResult { get; set; }
    //public (byte, byte, byte) VotePassResult { get; set; }
    //public (byte, byte, byte) VoteFailResult { get; set; }
    //public (byte, byte, byte) VoteAbstainResult { get; set; }
    public bool? IsBallotVotedResult { get; set; }
    public bool IsBallotPassedResult { get; set; }
    public byte GetBallotsPassedCountResult { get; set; }
    public sbyte GetCurrencyValueResult { get; set; }
    public bool IsProcedureActiveResult { get; set; }

    //public ConfirmationResult TryConfirmProcedure (IDType personId, IDType procedureId) => TryConfirmProcedureResult;

    //public bool? DeclareProcedure (IDType personId, IDType procedureId) => DeclareProcedureResult;

    //public void StartSetup () { }

    //public IDType ChooseCurrencyOwner (IDType personId) => ChooseCurrencyOwnerResult;

    //public (byte, byte, byte) VotePass (IDType personId, bool isPass) => VotePassResult;

    //public (byte, byte, byte) VoteFail (IDType personId, bool isFail) => VoteFailResult;

    //public (byte, byte, byte) VoteAbstain (IDType personId, bool isAbstain) => VoteAbstainResult;

    //public void InitialisePeople (List<Person> people) { }

    //public void VoteBallot (bool isPass) { }

    public override bool? IsBallotVoted () => IsBallotVotedResult;

    public override bool IsBallotPassed (IDType ballotId) => IsBallotPassedResult;

    public override byte GetBallotsPassedCount () => GetBallotsPassedCountResult;

    public override sbyte GetCurrencyValue (IDType currencyId) => GetCurrencyValueResult;

    public override bool IsProcedureActive (IDType procedureId) => IsProcedureActiveResult;
}
