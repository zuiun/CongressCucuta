using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Core.Procedures;
using CongressCucuta.Tests.Fakes;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class SimulationContextTests {
    [TestMethod]
    public void InitialisePeople_Normal_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        Person person0 = new (0, "0");
        Person person1 = new (1, "1");
        List<Person> people = [person0, person1];

        context.InitialisePeople (people);

        Assert.Contains (Role.MEMBER, context.PeopleRoles[0]);
        Assert.Contains (Role.MEMBER, context.PeopleRoles[1]);
        Assert.IsNull (context.PeopleFactions[0].Item1);
        Assert.IsNull (context.PeopleFactions[0].Item2);
        Assert.IsNull (context.PeopleFactions[1].Item1);
        Assert.IsNull (context.PeopleFactions[1].Item2);
    }

    [TestMethod]
    [DataRow ((byte) 0)]
    [DataRow ((byte) 2)]
    public void ChooseCurrencyOwner_Role_ReturnsExpected (byte roleId) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.PeopleFactions[0] = (roleId, roleId);

        IDType expected = roleId;
        IDType actual = context.ChooseCurrencyOwner (0);

        Assert.AreEqual (expected, actual);
    }

    [TestMethod]
    public void ChooseCurrencyOwner_NoRole_ReturnsExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);

        IDType actual = context.ChooseCurrencyOwner (0);

        Assert.AreEqual<IDType> (255, actual);
    }

    [TestMethod]
    public void TryConfirmProcedure_Always_ReturnsExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);

        SimulationContext.ConfirmationResult actual = context.TryConfirmProcedure (0, 3);

        Assert.AreEqual (Confirmation.ConfirmationType.Always, actual.Type);
        Assert.IsTrue (actual.IsConfirmed);
    }

    [TestMethod]
    public void TryConfirmProcedure_DivisionChamber_ReturnsExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            []
        );

        SimulationContext.ConfirmationResult actual = context.TryConfirmProcedure (0, 3);

        Assert.AreEqual (Confirmation.ConfirmationType.DivisionChamber, actual.Type);
        Assert.IsNull (actual.IsConfirmed);
    }

    [TestMethod]
    public void TryConfirmProcedure_CurrencyValue_ReturnsExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.CurrencyValue, 1),
            []
        );

        SimulationContext.ConfirmationResult actual = context.TryConfirmProcedure (0, 3);

        Assert.AreEqual (Confirmation.ConfirmationType.CurrencyValue, actual.Type);
        Assert.IsTrue (actual.IsConfirmed);
        Assert.AreEqual<byte?> (1, actual.Value);
        Assert.AreEqual (255, actual.Currency?.Item1);
        Assert.AreEqual<sbyte?> (0, actual.Currency?.Item2);
    }

    [TestMethod]
    public void TryConfirmProcedure_DiceValue_ReturnsExpected () {
        FakeSimulation simulation = new ();
        int dice = 0;
        FakeGenerator generator = new ([dice]);
        SimulationContext context = new (simulation, generator);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceValue, 1),
            []
        );

        SimulationContext.ConfirmationResult actual = context.TryConfirmProcedure (0, 3);

        Assert.AreEqual (Confirmation.ConfirmationType.DiceValue, actual.Type);
        Assert.IsFalse (actual.IsConfirmed);
        Assert.AreEqual<byte?> (1, actual.Value);
        Assert.AreEqual (dice, actual.DiceDeclarer);
    }

    [TestMethod]
    [DataRow (0, true)]
    [DataRow (6, false)]
    public void TryConfirmProcedure_DiceCurrencySuccess_ReturnsExpected (int dice, bool expected) {
        FakeSimulation simulation = new ();
        FakeGenerator generator = new ([dice]);
        SimulationContext context = new (simulation, generator);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceCurrency, 1),
            []
        );

        SimulationContext.ConfirmationResult actual = context.TryConfirmProcedure (0, 3);

        Assert.AreEqual (Confirmation.ConfirmationType.DiceCurrency, actual.Type);
        Assert.AreEqual (expected, actual.IsConfirmed);
        Assert.AreEqual (255, actual.Currency?.Item1);
        Assert.AreEqual<sbyte?> (1, actual.Currency?.Item2);
        Assert.AreEqual (dice, actual.DiceDeclarer);
    }

    [TestMethod]
    public void TryConfirmProcedure_DiceAdversarialCurrencyFailure_ReturnsExpected () {
        FakeSimulation simulation = new ();
        int[] dice = [6, 6];
        FakeGenerator generator = new (dice);
        SimulationContext context = new (simulation, generator);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceAdversarial),
            []
        );

        SimulationContext.ConfirmationResult actual = context.TryConfirmProcedure (0, 3);

        Assert.AreEqual (Confirmation.ConfirmationType.DiceAdversarial, actual.Type);
        Assert.IsFalse (actual.IsConfirmed);
        Assert.AreEqual (255, actual.Currency?.Item1);
        Assert.AreEqual<sbyte?> (1, actual.Currency?.Item2);
        Assert.AreEqual (dice[0], actual.DiceDeclarer);
    }

    [TestMethod]
    [DataRow (new int[] { 1, 6 }, false)]
    [DataRow (new int[] { 1, 0 }, true)]
    public void TryConfirmProcedure_DiceAdversarial_ReturnsExpected (int[] dice, bool expected) {
        FakeSimulation simulation = new ();
        FakeGenerator generator = new (dice);
        SimulationContext context = new (simulation, generator);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceAdversarial),
            []
        );

        SimulationContext.ConfirmationResult actual = context.TryConfirmProcedure (0, 3);

        Assert.AreEqual (Confirmation.ConfirmationType.DiceAdversarial, actual.Type);
        Assert.AreEqual (expected, actual.IsConfirmed);
        Assert.AreEqual (255, actual.Currency?.Item1);
        Assert.AreEqual<sbyte?> (0, actual.Currency?.Item2);
        Assert.AreEqual (dice[0], actual.DiceDeclarer);
        Assert.AreEqual (dice[1], actual.DiceDefender);
    }

    [TestMethod]
    public void DeclareProcedure_CurrencyAddTarget_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencyAdd, [0], 1)],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );

        context.DeclareProcedure (0, 3);

        Assert.AreEqual (2, context.CurrenciesValues[0]);
    }

    [TestMethod]
    public void DeclareProcedure_CurrencyAddNoTarget_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencyAdd, [], 1)],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );

        context.DeclareProcedure (0, 3);

        Assert.AreEqual (2, context.CurrenciesValues[255]);
    }

    [TestMethod]
    public void DeclareProcedure_CurrencySubtractTarget_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencySubtract, [0], 1)],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );

        context.DeclareProcedure (0, 3);

        Assert.AreEqual (0, context.CurrenciesValues[0]);
    }

    [TestMethod]
    public void DeclareProcedure_CurrencySubtractNoTarget_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencySubtract, [], 1)],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );

        context.DeclareProcedure (0, 3);

        Assert.AreEqual (0, context.CurrenciesValues[255]);
    }

    [TestMethod]
    public void DeclareProcedure_ElectionRegion_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionRegion, [0])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.DeclareProcedure (0, 3);

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Region, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
            Assert.AreEqual<IDType> (251, actual.TargetID);
        }
    }


    [TestMethod]
    public void DeclareProcedure_ElectionParty_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [0])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.DeclareProcedure (0, 3);

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Party, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
            Assert.AreEqual<IDType> (252, actual.TargetID);
        }
    }

    [TestMethod]
    public void DeclareProcedure_ElectionNominated_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionNominated, [0, 1])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.DeclareProcedure (0, 3);

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Nominated, actual.Type);
            Assert.Contains (1, actual.FilterIDs);
            Assert.AreEqual<IDType> (0, actual.TargetID);
        }
    }

    [TestMethod]
    public void DeclareProcedure_ElectionAppointed_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.ElectionAppointed, [0, 1])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.DeclareProcedure (0, 3);

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Appointed, actual.Type);
            Assert.Contains (1, actual.FilterIDs);
            Assert.AreEqual<IDType> (0, actual.TargetID);
        }
    }

    [TestMethod]
    public void DeclareProcedure_BallotLimit_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0"), new (1, "1"), new (2, "2")]);
        context.PeopleRoles[1].Add (1);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotLimit, [1])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );

        context.DeclareProcedure (0, 3);

        Assert.IsTrue (context.Context.PeoplePermissions[0].CanVote);
        Assert.IsTrue (context.Context.PeoplePermissions[1].CanVote);
        Assert.IsFalse (context.Context.PeoplePermissions[2].CanVote);
    }

    [TestMethod]
    public void DeclareProcedure_BallotPass_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        context.VotedBallot += Context_VotedBallotEventHandler;

        context.DeclareProcedure (0, 3);

        static void Context_VotedBallotEventHandler (VotedBallotEventArgs e) {
            Assert.IsTrue (e.IsPassed);
        }
    }

    [TestMethod]
    public void DeclareProcedure_BallotFail_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresDeclared[3] = new (
            3,
            [new Procedure.Effect (Procedure.Effect.EffectType.BallotFail, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            []
        );
        context.VotedBallot += Context_VotedBallotEventHandler;

        context.DeclareProcedure (0, 3);

        static void Context_VotedBallotEventHandler (VotedBallotEventArgs e) {
            Assert.IsFalse (e.IsPassed);
        }
    }

    [TestMethod]
    public void StartSetup_ElectionRegion_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionRegion, [0])]);
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartSetup ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Region, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
            Assert.AreEqual<IDType> (251, actual.TargetID);
        }
    }


    [TestMethod]
    public void StartSetup_ElectionParty_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [0])]);
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartSetup ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Party, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
            Assert.AreEqual<IDType> (252, actual.TargetID);
        }
    }

    [TestMethod]
    public void StartSetup_ElectionNominated_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionNominated, [0, 1])]);
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartSetup ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Nominated, actual.Type);
            Assert.Contains (1, actual.FilterIDs);
            Assert.AreEqual<IDType> (0, actual.TargetID);
        }
    }

    [TestMethod]
    public void StartSetup_ElectionAppointed_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionAppointed, [0, 1])]);
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartSetup ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Appointed, actual.Type);
            Assert.Contains (1, actual.FilterIDs);
            Assert.AreEqual<IDType> (0, actual.TargetID);
        }
    }

    [TestMethod]
    [DataRow ((byte) 1, true)]
    [DataRow ((byte) 0, false)]
    public void StartSetup_PermissionsCanVote_MutatesExpected (byte value, bool expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsCanVote, [255], value)]);

        context.StartSetup ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[0].CanVote);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 1)]
    [DataRow ((byte) 1, (byte) 2)]
    public void StartSetup_PermissionsVotes_MutatesExpected (byte value, byte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsVotes, [255], value)]);

        context.StartSetup ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[0].Votes);
    }

    [TestMethod]
    [DataRow ((byte) 1, true)]
    [DataRow ((byte) 0, false)]
    public void StartSetup_PermissionsCanSpeak_MutatesExpected (byte value, bool expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsCanSpeak, [255], value)]);

        context.StartSetup ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[0].CanSpeak);
    }

    [TestMethod]
    public void StartBallot_VotePassAdd_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.VotePassAdd, [], 1)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (1, context.Context.VotesPassBonus);
    }

    [TestMethod]
    public void StartBallot_VoteFailAdd_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (1, context.Context.VotesFailBonus);
    }

    [TestMethod]
    public void StartBallot_VotePassTwoThirds_MutatesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.VotePassTwoThirds, [])],
            []
        );

        context.StartBallot ();

        Assert.IsFalse (context.Context.IsSimpleMajority);
    }

    [TestMethod]
    [DataRow ((byte) 0, (sbyte) 1)]
    [DataRow ((byte) 1, (sbyte) 2)]
    public void StartBallot_CurrencyAddTarget_MutatesExpected (byte ballotId, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = ballotId
        };
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencyAdd, [0], 1)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.CurrenciesValues[0]);
    }

    [TestMethod]
    [DataRow ((byte) 0, (sbyte) 1)]
    [DataRow ((byte) 1, (sbyte) 2)]
    public void StartBallot_CurrencyAddNoTarget_MutatesExpected (byte ballotId, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = ballotId
        };
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencyAdd, [], 1)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.CurrenciesValues[255]);
    }

    [TestMethod]
    [DataRow ((byte) 0, (sbyte) 1)]
    [DataRow ((byte) 1, (sbyte) 0)]
    public void StartBallot_CurrencySubtractTarget_MutatesExpected (byte ballotId, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = ballotId
        };
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencySubtract, [0], 1)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.CurrenciesValues[0]);
    }

    [TestMethod]
    [DataRow ((byte) 0, (sbyte) 1)]
    [DataRow ((byte) 1, (sbyte) 0)]
    public void StartBallot_CurrencySubtractNoTarget_MutatesExpected (byte ballotId, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = ballotId
        };
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.CurrencySubtract, [], 1)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.CurrenciesValues[255]);
    }

    [TestMethod]
    public void StartBallot_ProcedureActivateBallot0_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.ProcedureActivate, [0])],
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartBallot ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            Assert.Fail ();
        }
    }

    [TestMethod]
    public void StartBallot_ProcedureActivateRegion_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = 1,
        };
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionRegion, [0])]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.ProcedureActivate, [0])],
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartBallot ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Region, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
            Assert.AreEqual<IDType> (251, actual.TargetID);
        }
    }

    [TestMethod]
    public void StartBallot_ProcedureActivateParty_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = 1,
        };
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionParty, [0])]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.ProcedureActivate, [0])],
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartBallot ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Party, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
            Assert.AreEqual<IDType> (252, actual.TargetID);
        }
    }

    [TestMethod]
    public void StartBallot_ProcedureActivateNominated_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = 1,
        };
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionNominated, [0, 1])]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.ProcedureActivate, [0])],
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartBallot ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Nominated, actual.Type);
            Assert.Contains (1, actual.FilterIDs);
            Assert.AreEqual<IDType> (0, actual.TargetID);
        }
    }

    [TestMethod]
    public void StartBallot_ProcedureActivateAppointed_InvokesExpected () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = 1,
        };
        context.ProceduresGovernmental[0] = new (0, [new Procedure.Effect (Procedure.Effect.EffectType.ElectionAppointed, [0, 1])]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.ProcedureActivate, [0])],
            []
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.StartBallot ();

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.Appointed, actual.Type);
            Assert.Contains (1, actual.FilterIDs);
            Assert.AreEqual<IDType> (0, actual.TargetID);
        }
    }

    [TestMethod]
    [DataRow ((byte) 1, true)]
    [DataRow ((byte) 0, false)]
    public void StartBallot_PermissionsCanVoteTarget_MutatesExpected (byte value, bool expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsCanVote, [255], value)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[0].CanVote);
    }

    [TestMethod]
    [DataRow ((byte) 1, true)]
    [DataRow ((byte) 0, false)]
    public void StartBallot_PermissionsCanVoteNoTarget_MutatesExpected (byte value, bool expected) {
        FakeSimulation simulation = new ();
        FakeGenerator generator = new ([1]);
        SimulationContext context = new (simulation, generator);
        context.InitialisePeople ([new (0, "0"), new (1, "1")]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsCanVote, [], value)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[1].CanVote);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 1)]
    [DataRow ((byte) 1, (byte) 2)]
    public void StartBallot_PermissionsVotesTarget_MutatesExpected (byte value, byte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsVotes, [255], value)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[0].Votes);
    }

    [TestMethod]
    [DataRow ((byte) 0, (byte) 1)]
    [DataRow ((byte) 1, (byte) 2)]
    public void StartBallot_PermissionsVotesNoTarget_MutatesExpected (byte value, byte expected) {
        FakeSimulation simulation = new ();
        FakeGenerator generator = new ([1]);
        SimulationContext context = new (simulation, generator);
        context.InitialisePeople ([new (0, "0"), new (1, "1")]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsVotes, [], value)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[1].Votes);
    }

    [TestMethod]
    [DataRow ((byte) 1, true)]
    [DataRow ((byte) 0, false)]
    public void StartBallot_PermissionsCanSpeakTarget_MutatesExpected (byte value, bool expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.InitialisePeople ([new (0, "0")]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsCanSpeak, [255], value)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[0].CanSpeak);
    }

    [TestMethod]
    [DataRow ((byte) 1, true)]
    [DataRow ((byte) 0, false)]
    public void StartBallot_PermissionsCanSpeakNoTarget_MutatesExpected (byte value, bool expected) {
        FakeSimulation simulation = new ();
        FakeGenerator generator = new ([1]);
        SimulationContext context = new (simulation, generator);
        context.InitialisePeople ([new (0, "0"), new (1, "1")]);
        context.ProceduresSpecial[1] = new (
            1,
            [new Procedure.Effect (Procedure.Effect.EffectType.PermissionsCanSpeak, [], value)],
            []
        );

        context.StartBallot ();

        Assert.AreEqual (expected, context.Context.PeoplePermissions[1].CanSpeak);
    }

    [TestMethod]
    [DataRow ((byte) 0, true)]
    [DataRow ((byte) 0, false)]
    [DataRow ((byte) 1, true)]
    [DataRow ((byte) 1, false)]
    public void EndBallot_Normal_InvokesExpected (byte ballotId, bool isPass) {
        IDType expected = ballotId;
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation) {
            BallotCurrentID = ballotId,
        };
        context.VotedBallot += Context_VotedBallotEventHandler;

        context.EndBallot (isPass);

        void Context_VotedBallotEventHandler (VotedBallotEventArgs e) {
            Assert.AreEqual (expected, e.ID);
            Assert.AreEqual (isPass, e.IsPassed);
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void EndBallot_FoundParty_InvokesExpected (bool isPass) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.FoundParty, [0])], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.FoundParty, [0])], [])
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.EndBallot (isPass);

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.ShuffleAdd, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
            Assert.AreEqual<IDType> (252, actual.TargetID);
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void EndBallot_DissolveParty_InvokesExpected (bool isPass) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.DissolveParty, [0])], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.DissolveParty, [0])], [])
        );
        context.PreparingElection += Context_PreparingElectionEventHandler;

        context.EndBallot (isPass);

        static void Context_PreparingElectionEventHandler (PreparingElectionEventArgs e) {
            ElectionContext actual = e.Elections[0];

            Assert.AreEqual (ElectionContext.ElectionType.ShuffleRemove, actual.Type);
            Assert.Contains (0, actual.FilterIDs);
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void EndBallot_RemoveProcedure_InvokesExpected (bool isPass) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.RemoveProcedure, [1])], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.RemoveProcedure, [1])], [])
        );
        context.ModifiedProcedures += Context_ModifiedProceduresEventHandler;

        context.EndBallot (isPass);

        static void Context_ModifiedProceduresEventHandler (HashSet<ProcedureTargeted> e) {
            Assert.IsEmpty (e);
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void EndBallot_ReplaceProcedure_InvokesExpected (bool isPass) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [1, 2])], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [1, 2])], [])
        );
        context.ModifiedProcedures += Context_ModifiedProceduresEventHandler;

        context.EndBallot (isPass);

        static void Context_ModifiedProceduresEventHandler (HashSet<ProcedureTargeted> e) {
            Assert.HasCount (1, e);
            Assert.AreEqual<IDType> (2, e.First ().ID);
        }
    }

    [TestMethod]
    [DataRow (true, (sbyte) 1, (sbyte) 2)]
    [DataRow (false, (sbyte) -1, (sbyte) 0)]
    public void EndBallot_ModifyCurrencyState_MutatesExpected (bool isPass, sbyte value, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [255], value)], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [255], value)], [])
        );

        context.EndBallot (isPass);

        Assert.AreEqual (expected, context.CurrenciesValues[255]);
    }

    [TestMethod]
    [DataRow (true, (sbyte) 1, (sbyte) 2)]
    [DataRow (false, (sbyte) -1, (sbyte) 0)]
    public void EndBallot_ModifyCurrencyParty_MutatesExpected (bool isPass, sbyte value, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [254], value)], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [254], value)], [])
        );

        context.EndBallot (isPass);

        Assert.AreEqual (expected, context.CurrenciesValues[2]);
        Assert.AreEqual (expected, context.CurrenciesValues[3]);
    }

    [TestMethod]
    [DataRow (true, (sbyte) 1, (sbyte) 2)]
    [DataRow (false, (sbyte) -1, (sbyte) 0)]
    public void EndBallot_ModifyCurrencyRegion_MutatesExpected (bool isPass, sbyte value, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [253], value)], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [253], value)], [])
        );

        context.EndBallot (isPass);

        Assert.AreEqual (expected, context.CurrenciesValues[0]);
        Assert.AreEqual (expected, context.CurrenciesValues[1]);
    }

    [TestMethod]
    [DataRow (true, (sbyte) 1, (sbyte) 2)]
    [DataRow (false, (sbyte) -1, (sbyte) 0)]
    public void EndBallot_ModifyCurrencyNormal_MutatesExpected (bool isPass, sbyte value, sbyte expected) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.Ballots[0] = new (
            0,
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [0, 2], value)], []),
            new Ballot.Result ([new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [0, 2], value)], [])
        );

        context.EndBallot (isPass);

        Assert.AreEqual (expected, context.CurrenciesValues[0]);
        Assert.AreEqual (expected, context.CurrenciesValues[2]);
    }

    [TestMethod]
    public void IsBallotPassed_Pass_ReturnsTrue () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.EndBallot (true);

        bool actual = context.IsBallotPassed (0);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void IsBallotPassed_Fail_ReturnsFalse () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.EndBallot (false);

        bool actual = context.IsBallotPassed (0);

        Assert.IsFalse (actual);
    }

    [TestMethod]
    public void IsBallotPassed_Current_ReturnsFalse () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);

        bool actual = context.IsBallotPassed (0);

        Assert.IsFalse (actual);
    }

    [TestMethod]
    public void GetBallotsPassedCount_Pass_Returns1 () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.EndBallot (true);

        byte actual = context.GetBallotsPassedCount ();

        Assert.AreEqual (1, actual);
    }

    [TestMethod]
    public void GetBallotsPassedCount_Fail_Returns0 () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);
        context.EndBallot (false);

        byte actual = context.GetBallotsPassedCount ();

        Assert.AreEqual (0, actual);
    }

    [TestMethod]
    public void GetBallotsPassedCount_Current_Returns0 () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);

        byte actual = context.GetBallotsPassedCount ();

        Assert.AreEqual (0, actual);
    }

    [TestMethod]
    [DataRow ((byte) 0)]
    [DataRow ((byte) 1)]
    [DataRow ((byte) 2)]
    [DataRow ((byte) 3)]
    [DataRow ((byte) 255)]
    public void GetCurrencyValue_Normal_ReturnsExpected (byte currencyId) {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);

        sbyte actual = context.GetCurrencyValue (currencyId);

        Assert.AreEqual (1, actual);
    }

    [TestMethod]
    public void IsProcedureActive_ActiveStart_ReturnsTrue () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);

        bool actual = context.IsProcedureActive (1);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void IsProcedureActive_NotActiveStart_ReturnsFalse () {
        FakeSimulation simulation = new ();
        SimulationContext context = new (simulation);

        bool actual = context.IsProcedureActive (2);

        Assert.IsFalse (actual);
    }
}
