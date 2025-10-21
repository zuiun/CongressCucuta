using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Tests.Unit.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class ConditionTests {
    [TestMethod]
    public void Evaluate_AlwaysCondition_ReturnsTrue () {
        FakeSimulationContext context = new ();
        AlwaysCondition condition = new ();

        bool actual = condition.Evaluate (context);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void ToString_AlwaysCondition_ReturnsNext () {
        Localisation localisation = FakeLocalisation.Create ();
        AlwaysCondition condition = new ();

        string actual = condition.ToString (in localisation);

        Assert.AreEqual ("Next", actual);
    }

    [TestMethod]
    public void YieldBallotVote_AlwaysCondition_ReturnsNull () {
        AlwaysCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }

    [TestMethod]
    public void Evaluate_NeverCondition_ReturnsFalse () {
        FakeSimulationContext context = new ();
        NeverCondition condition = new ();

        bool actual = condition.Evaluate (context);

        Assert.IsFalse (actual);
    }

    [TestMethod]
    public void ToString_NeverCondition_ReturnsEnd () {
        Localisation localisation = FakeLocalisation.Create ();
        NeverCondition condition = new ();

        string actual = condition.ToString (in localisation);

        Assert.AreEqual ("End", actual);
    }

    [TestMethod]
    public void YieldBallotVote_NeverCondition_ReturnsNull () {
        NeverCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }

    [TestMethod]
    public void Evaluate_AndConditionAlwaysAlways_ReturnsTrue () {
        FakeSimulationContext context = new ();
        AndCondition condition = new (
            new AlwaysCondition (),
            new AlwaysCondition ()
        );

        bool actual = condition.Evaluate (context);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void Evaluate_AndConditionAlwaysNever_ReturnsFalse () {
        FakeSimulationContext context = new ();
        AndCondition condition = new (
            new AlwaysCondition (),
            new NeverCondition ()
        );

        bool actual = condition.Evaluate (context);

        Assert.IsFalse (actual);
    }

    [TestMethod]
    public void ToString_AndConditionAlwaysAlways_ReturnsNextEnd () {
        Localisation localisation = FakeLocalisation.Create ();
        AndCondition condition = new (
            new AlwaysCondition (),
            new NeverCondition ()
        );

        string actual = condition.ToString (in localisation);

        Assert.AreEqual ("Next and End", actual);
    }

    [TestMethod]
    public void YieldBallotVote_AndCondition_ReturnsNull () {
        AndCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }

    [TestMethod]
    public void Evaluate_OrConditionAlwaysAlways_ReturnsTrue () {
        FakeSimulationContext context = new ();
        OrCondition condition = new (
            new AlwaysCondition (),
            new AlwaysCondition ()
        );

        bool actual = condition.Evaluate (context);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void Evaluate_OrConditionAlwaysNever_ReturnsTrue () {
        FakeSimulationContext context = new ();
        OrCondition condition = new (
            new AlwaysCondition (),
            new NeverCondition ()
        );

        bool actual = condition.Evaluate (context);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void Evaluate_OrConditionNeverNever_ReturnsFalse () {
        FakeSimulationContext context = new ();
        OrCondition condition = new (
            new NeverCondition (),
            new NeverCondition ()
        );

        bool actual = condition.Evaluate (context);

        Assert.IsFalse (actual);
    }

    [TestMethod]
    public void ToString_OrConditionAlwaysAlways_ReturnsNextEnd () {
        Localisation localisation = FakeLocalisation.Create ();
        OrCondition condition = new (
            new AlwaysCondition (),
            new NeverCondition ()
        );

        string actual = condition.ToString (in localisation);

        Assert.AreEqual ("Next or End", actual);
    }

    [TestMethod]
    public void YieldBallotVote_OrCondition_ReturnsNull () {
        OrCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }

    [TestMethod]
    [DataRow (true, true)]
    [DataRow (false, true)]
    [DataRow (true, false)]
    [DataRow (false, false)]
    public void Evaluate_BallotVoteConditionPassed_ReturnsExpected (bool shouldBePassed, bool isPassed) {
        FakeSimulationContext context = new () {
            IsBallotVotedResult = isPassed,
        };
        BallotVoteCondition condition = new (shouldBePassed);

        bool expected = shouldBePassed == isPassed;
        bool actual = condition.Evaluate (context);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void Evaluate_BallotVoteConditionFailed_ReturnsExpected () {

    }

    [TestMethod]
    public void Evaluate_BallotVoteConditionNone_ReturnsExpected () {

    }

    [TestMethod]
    [DataRow (true, "Pass")]
    [DataRow (false, "Fail")]
    public void ToString_BallotVoteCondition_ReturnsExpected (bool shouldBePassed, string expected) {
        Localisation localisation = FakeLocalisation.Create ();
        BallotVoteCondition condition = new (shouldBePassed);

        string actual = condition.ToString (in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void YieldBallotVote_BallotVoteCondition_ReturnsExpected (bool shouldBePassed) {
        BallotVoteCondition condition = new (shouldBePassed);

        bool? actual = condition.YieldBallotVote ();

        Assert.AreEqual (shouldBePassed, actual);
    }

    [TestMethod]
    [DataRow (true, true)]
    [DataRow (false, true)]
    [DataRow (true, false)]
    [DataRow (false, false)]
    public void Evaluate_BallotPassedCondition_ReturnsExpected (bool shouldBePassed, bool isPassed) {
        FakeSimulationContext context = new () {
            IsBallotPassedResult = isPassed,
        };
        context.VoteBallot (isPassed);
        BallotPassedCondition condition = new (0, shouldBePassed);

        bool expected = shouldBePassed == isPassed;
        bool actual = condition.Evaluate (context);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (true, "0 Passed")]
    [DataRow (false, "0 Failed")]
    public void ToString_BallotPassedCondition_ReturnsExpected (bool shouldBePassed, string expected) {
        Localisation localisation = FakeLocalisation.Create ();
        BallotPassedCondition condition = new (0, shouldBePassed);

        string actual = condition.ToString (in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void YieldBallotVote_BallotPassedCondition_ReturnsExpected () {
        BallotPassedCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }

    [TestMethod]
    [DataRow (0, (byte) 1, true)]
    [DataRow (0, (byte) 0, false)]
    [DataRow (1, (byte) 0, true)]
    [DataRow (1, (byte) 1, false)]
    [DataRow (2, (byte) 2, true)]
    [DataRow (2, (byte) 1, false)]
    [DataRow (3, (byte) 1, true)]
    [DataRow (3, (byte) 2, false)]
    [DataRow (4, (byte) 1, true)]
    [DataRow (4, (byte) 0, false)]
    public void Evaluate_BallotsPassedCountCondition_ReturnsExpected (int comparison, byte count, bool expected) {
        FakeSimulationContext context = new () {
            GetBallotsPassedCountResult = 1,
        };
        BallotsPassedCountCondition condition = new ((ComparisonType) comparison, count);

        bool actual = condition.Evaluate (context);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (0, "1 Ballot(s) Passed")]
    [DataRow (1, "Greater than 1 Ballot(s) Passed")]
    [DataRow (2, "Fewer than 1 Ballot(s) Passed")]
    [DataRow (3, "1 or Greater Ballot(s) Passed")]
    [DataRow (4, "1 or Fewer Ballot(s) Passed")]
    public void ToString_BallotsPassedCountCondition_ReturnsExpected (int comparison, string expected) {
        Localisation localisation = FakeLocalisation.Create ();
        BallotsPassedCountCondition condition = new ((ComparisonType) comparison, 1);

        string actual = condition.ToString (in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void YieldBallotVote_BallotsPassedCountCondition_ReturnsNull () {
        BallotsPassedCountCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }

    [TestMethod]
    [DataRow (0, (sbyte) 1, true)]
    [DataRow (0, (sbyte) 0, false)]
    [DataRow (1, (sbyte) 0, true)]
    [DataRow (1, (sbyte) 1, false)]
    [DataRow (2, (sbyte) 2, true)]
    [DataRow (2, (sbyte) 1, false)]
    [DataRow (3, (sbyte) 1, true)]
    [DataRow (3, (sbyte) 2, false)]
    [DataRow (4, (sbyte) 1, true)]
    [DataRow (4, (sbyte) 0, false)]
    public void Evaluate_CurrencyValueCondition_ReturnsExpected (int comparison, sbyte value, bool expected) {
        FakeSimulationContext context = new () {
            GetCurrencyValueResult = 1,
        };
        CurrencyValueCondition condition = new (Currency.STATE, (ComparisonType) comparison, value);

        bool actual = condition.Evaluate (context);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (0, "1 Currency State")]
    [DataRow (1, "Greater than 1 Currency State")]
    [DataRow (2, "Fewer than 1 Currency State")]
    [DataRow (3, "1 or Greater Currency State")]
    [DataRow (4, "1 or Fewer Currency State")]
    public void ToString_CurrencyValueCondition_ReturnsExpected (int comparison, string expected) {
        Localisation localisation = FakeLocalisation.Create ();
        CurrencyValueCondition condition = new (Currency.STATE, (ComparisonType) comparison, 1);

        string actual = condition.ToString (in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void YieldBallotVote_CurrencyValueCondition_ReturnsNull () {
        CurrencyValueCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }

    [TestMethod]
    [DataRow (true, true)]
    [DataRow (false, true)]
    [DataRow (true, false)]
    [DataRow (false, false)]
    public void Evaluate_ProcedureActiveCondition_ReturnsExpected (bool shouldBeActive, bool isActive) {
        FakeSimulationContext context = new () {
            IsProcedureActiveResult = isActive,
        };
        ProcedureActiveCondition condition = new (0, shouldBeActive);

        bool expected = shouldBeActive == isActive;
        bool actual = condition.Evaluate (context);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    [DataRow (true, "Procedure 0 is Active")]
    [DataRow (false, "Procedure 0 is not Active")]
    public void ToString_ProcedureActiveCondition_ReturnsExpected (bool shouldBeActive, string expected) {
        Localisation localisation = FakeLocalisation.Create ();
        ProcedureActiveCondition condition = new (0, shouldBeActive);

        string actual = condition.ToString (in localisation);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void YieldBallotVote_ProcedureActiveCondition_ReturnsNull () {
        ProcedureActiveCondition condition = new ();

        bool? actual = condition.YieldBallotVote ();

        Assert.IsNull (actual);
    }
}
