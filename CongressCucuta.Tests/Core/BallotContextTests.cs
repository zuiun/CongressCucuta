using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class BallotContextTests {
    private static BallotContext Create () {
        BallotContext context = new ();

        context.PeoplePermissions[0] = new (true);
        context.PeoplePermissions[1] = new (true);
        context.PeoplePermissions[2] = new (true);
        return context;
    }

    [TestMethod]
    public void Reset_Normal_MutatesExpected () {
        BallotContext context = new ();
        context.VotesPass.Add (0);
        context.VotesFail.Add (0);
        context.ProceduresDeclared.Add (0);
        context.VotesPassBonus = 1;
        context.VotesFailBonus = 1;

        context.Reset ();

        Assert.IsEmpty (context.VotesPass);
        Assert.IsEmpty (context.VotesFail);
        Assert.IsEmpty (context.ProceduresDeclared);
        Assert.AreEqual (0, context.VotesPassBonus);
        Assert.AreEqual (0, context.VotesFailBonus);
    }

    [TestMethod]
    public void ResetVotes_Normal_MutatesExpected () {
        byte votesResult = 1;
        byte votesTotal = 3;
        byte votesResultThreshold = 3;
        byte votesPass = byte.MaxValue;
        byte votesFail = byte.MaxValue;
        byte votesAbstain = byte.MaxValue;
        byte votesPassThreshold = byte.MaxValue;
        byte votesFailThreshold = byte.MaxValue;
        void Context_UpdateVotesEventHandler (UpdatedVotesEventArgs e) {
            votesPass = e.VotesPass;
            votesFail = e.VotesFail;
            votesAbstain = e.VotesAbstain;
            votesPassThreshold = e.VotesPassThreshold;
            votesFailThreshold = e.VotesFailThreshold;
        }
        BallotContext context = Create ();
        IDType procedure = 0;
        context.VotesPass.Add (0);
        context.VotesFail.Add (0);
        context.ProceduresDeclared.Add (procedure);
        context.VotesPassBonus = 1;
        context.VotesFailBonus = 1;
        context.UpdatedVotes += Context_UpdateVotesEventHandler;

        context.ResetVotes ();

        Assert.IsEmpty (context.VotesPass);
        Assert.IsEmpty (context.VotesFail);
        CollectionAssert.Contains (context.ProceduresDeclared, procedure);
        Assert.AreEqual (1, context.VotesPassBonus);
        Assert.AreEqual (1, context.VotesFailBonus);
        Assert.AreEqual (votesResult, votesPass);
        Assert.AreEqual (votesResult, votesFail);
        Assert.AreEqual (votesTotal, votesAbstain);
        Assert.AreEqual (votesResultThreshold, votesPassThreshold);
        Assert.AreEqual (votesResultThreshold, votesFailThreshold);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, (byte) 3)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, (byte) 4)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, (byte) 4)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, (byte) 4)]
    [DataRow ((byte) 1, (byte) 1, (byte) 0, (byte) 5)]
    [DataRow ((byte) 1, (byte) 0, (byte) 1, (byte) 5)]
    [DataRow ((byte) 0, (byte) 1, (byte) 1, (byte) 5)]
    [DataRow ((byte) 1, (byte) 1, (byte) 1, (byte) 6)]
    public void CalculateVotesTotal_Normal_ReturnsExpected (byte passBonus, byte failBonus, byte votesBonus, byte expected) {
        BallotContext context = Create ();
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);

        byte actual = context.CalculateVotesTotal ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, new byte[] { 0 }, (byte) 1)]
    [DataRow ((byte) 0, (byte) 0, new byte[] { 0, 1 }, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, new byte[] { 0 }, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, new byte[] { 0, 1 }, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, new byte[] { 0 }, (byte) 2)]
    [DataRow ((byte) 0, (byte) 1, new byte[] { 0, 1 }, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, new byte[] { 0 }, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, new byte[] { 0, 1 }, (byte) 4)]
    public void CalculateVotesPass_Normal_ReturnsExpected (byte passBonus, byte votesBonus, byte[] passIds, byte expected) {
        BallotContext context = Create ();
        context.VotesPassBonus = passBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);
        context.VotesPass.AddRange ([.. passIds]);

        byte actual = context.CalculateVotesPass ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, new byte[] { 0 }, (byte) 1)]
    [DataRow ((byte) 0, (byte) 0, new byte[] { 0, 1 }, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, new byte[] { 0 }, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, new byte[] { 0, 1 }, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, new byte[] { 0 }, (byte) 2)]
    [DataRow ((byte) 0, (byte) 1, new byte[] { 0, 1 }, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, new byte[] { 0 }, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, new byte[] { 0, 1 }, (byte) 4)]
    public void CalculateVotesFail_Normal_ReturnsExpected (byte failBonus, byte votesBonus, byte[] failIds, byte expected) {
        BallotContext context = Create ();
        context.VotesFailBonus = failBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);
        context.VotesFail.AddRange ([.. failIds]);

        byte actual = context.CalculateVotesFail ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { }, new byte[] { }, (byte) 3)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, new byte[] { }, new byte[] { }, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, new byte[] { }, new byte[] { }, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, (byte) 0, new byte[] { }, new byte[] { }, (byte) 3)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, new byte[] { }, new byte[] { }, (byte) 4)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { }, (byte) 2)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, new byte[] { 0 }, new byte[] { }, (byte) 2)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { 1 }, (byte) 1)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0, 2 }, new byte[] { 1 }, (byte) 0)]
    public void CalculateVotesAbstain_Normal_ReturnsExpected (
        byte passBonus,
        byte failBonus,
        byte votesBonus,
        byte[] passIds,
        byte[] failIds,
        byte expected
    ) {
        BallotContext context = Create ();
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);
        context.VotesPass.AddRange ([.. passIds]);
        context.VotesFail.AddRange ([.. failIds]);

        byte actual = context.CalculateVotesAbstain ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, (byte) 3)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, (byte) 0, (byte) 3)]
    [DataRow ((byte) 1, (byte) 0, (byte) 1, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, (byte) 1, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, (byte) 1, (byte) 4)]
    public void CalculateVotesPassThreshold_SimpleMajority_ReturnsExpected (byte passBonus, byte failBonus, byte votesBonus, byte expected) {
        BallotContext context = Create ();
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);

        byte actual = context.CalculateVotesPassThreshold ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, (byte) 3)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, (byte) 0, (byte) 4)]
    [DataRow ((byte) 1, (byte) 0, (byte) 1, (byte) 4)]
    [DataRow ((byte) 0, (byte) 1, (byte) 1, (byte) 4)]
    [DataRow ((byte) 1, (byte) 1, (byte) 1, (byte) 4)]
    public void CalculateVotesPassThreshold_SuperMajority_ReturnsExpected (byte passBonus, byte failBonus, byte votesBonus, byte expected) {
        BallotContext context = Create ();
        context.IsSimpleMajority = false;
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);

        byte actual = context.CalculateVotesPassThreshold ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, (byte) 3)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, (byte) 0, (byte) 3)]
    [DataRow ((byte) 1, (byte) 0, (byte) 1, (byte) 3)]
    [DataRow ((byte) 0, (byte) 1, (byte) 1, (byte) 3)]
    [DataRow ((byte) 1, (byte) 1, (byte) 1, (byte) 4)]
    public void CalculateVotesFailThreshold_SimpleMajority_ReturnsExpected (byte passBonus, byte failBonus, byte votesBonus, byte expected) {
        BallotContext context = Create ();
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);

        byte actual = context.CalculateVotesFailThreshold ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, (byte) 2)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, (byte) 2)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, (byte) 2)]
    [DataRow ((byte) 1, (byte) 1, (byte) 0, (byte) 2)]
    [DataRow ((byte) 1, (byte) 0, (byte) 1, (byte) 2)]
    [DataRow ((byte) 0, (byte) 1, (byte) 1, (byte) 2)]
    [DataRow ((byte) 1, (byte) 1, (byte) 1, (byte) 3)]
    public void CalculateVotesFailThreshold_SuperMajority_ReturnsExpected (byte passBonus, byte failBonus, byte votesBonus, byte expected) {
        BallotContext context = Create ();
        context.IsSimpleMajority = false;
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);

        byte actual = context.CalculateVotesFailThreshold ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { }, new byte[] { }, null)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { }, null)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { 1 }, null)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0, 2 }, new byte[] { 1 }, true)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, new byte[] { 0, 2 }, new byte[] { 1 }, true)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, new byte[] { 0, 2 }, new byte[] { 1 }, null)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, new byte[] { 0, 2 }, new byte[] { 1 }, true)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { 1, 2 }, false)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { 1, 2 }, null)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, new byte[] { 0 }, new byte[] { 1, 2 }, false)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, new byte[] { 0 }, new byte[] { 1, 2 }, null)]
    public void IsBallotVoted_SimpleMajority_ReturnsExpected (
        byte passBonus,
        byte failBonus,
        byte votesBonus,
        byte[] passIds,
        byte[] failIds,
        bool? expected
    ) {
        BallotContext context = Create ();
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.VotesPass.AddRange ([.. passIds]);
        context.VotesFail.AddRange ([.. failIds]);
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);

        bool? actual = context.IsBallotVoted ();

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { }, new byte[] { }, null)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { }, null)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { 1 }, null)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0, 2 }, new byte[] { 1 }, true)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, new byte[] { 0, 2 }, new byte[] { 1 }, true)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, new byte[] { 0, 2 }, new byte[] { 1 }, false)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, new byte[] { 0, 2 }, new byte[] { 1 }, true)]
    [DataRow ((byte) 0, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { 1, 2 }, false)]
    [DataRow ((byte) 1, (byte) 0, (byte) 0, new byte[] { 0 }, new byte[] { 1, 2 }, false)]
    [DataRow ((byte) 0, (byte) 1, (byte) 0, new byte[] { 0 }, new byte[] { 1, 2 }, false)]
    [DataRow ((byte) 0, (byte) 0, (byte) 1, new byte[] { 0 }, new byte[] { 1, 2 }, false)]
    public void IsBallotVoted_SuperMajority_ReturnsExpected (
        byte passBonus,
        byte failBonus,
        byte votesBonus,
        byte[] passIds,
        byte[] failIds,
        bool? expected
    ) {
        BallotContext context = Create ();
        context.IsSimpleMajority = false;
        context.VotesPassBonus = passBonus;
        context.VotesFailBonus = failBonus;
        context.VotesPass.AddRange ([.. passIds]);
        context.VotesFail.AddRange ([.. failIds]);
        context.PeoplePermissions[0] += new Permissions.Composition (Votes: votesBonus);

        bool? actual = context.IsBallotVoted ();

        Assert.AreEqual (expected, actual);
    }
}
