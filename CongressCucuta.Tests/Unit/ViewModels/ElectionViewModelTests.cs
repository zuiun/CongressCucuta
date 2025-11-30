using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.Tests.Fakes;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class ElectionViewModelTests {
    private static ElectionViewModel Create (params List<ElectionContext> contexts) {
        Dictionary<IDType, SortedSet<IDType>> peopleRoles = [];
        peopleRoles[0] = [];
        Dictionary<IDType, (IDType?, IDType?)> peopleFactions = [];
        peopleFactions[0] = (3, null);
        HashSet<IDType> partiesActive = [2];
        HashSet<IDType> regionsActive = [];
        Dictionary<IDType, Person> people = [];
        people[0] = new (0, "0");
        PreparingElectionEventArgs args = new (contexts, peopleRoles, peopleFactions, partiesActive, regionsActive, people);
        Localisation localisation = FakeLocalisation.Create ();

        return new (args, in localisation);
    }

    [TestMethod]
    public void RunNextElectionCommand_ExecuteNormal_MutatesExpected () {
        ElectionContext context1 = new (ElectionContext.ElectionType.ShuffleRemove, [2]);
        ElectionContext context2 = new (ElectionContext.ElectionType.ShuffleRemove, [3]);
        ElectionViewModel election = Create (context1, context2);

        election.RunNextElectionCommand.Execute (null);

        Assert.AreEqual ("Dissolution of Party 3", election.Title);
    }

    [TestMethod]
    public void RunNextElectionCommand_ExecuteEnd_InvokesExpected () {
        bool isCompleted = false;
        ElectionContext context = new (ElectionContext.ElectionType.ShuffleRemove, [2]);
        ElectionViewModel election = Create (context);
        election.CompletingElection += () => isCompleted = true;

        election.RunNextElectionCommand.Execute (null);

        Assert.IsTrue (isCompleted);
    }

    [TestMethod]
    public void RunNextElectionCommand_CanExecute_ReturnsTrue () {
        ElectionContext context = new (ElectionContext.ElectionType.ShuffleRemove, [2]);
        ElectionViewModel election = Create (context);

        bool actual = election.RunNextElectionCommand.CanExecute (null);

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void RunNextElectionCommand_CanExecute_ReturnsFalse () {
        ElectionContext context = new (ElectionContext.ElectionType.ShuffleAdd, [2], isLeaderNeeded: true);
        ElectionViewModel election = Create (context);
        election.RunElection ();

        bool actual = election.RunNextElectionCommand.CanExecute (null);

        Assert.IsFalse (actual);
    }

    [TestMethod]
    public void TryRunNextElectionCommand_ExecuteSuccess_InvokesExpected () {
        bool isCompleted = false;
        ElectionContext context = new (ElectionContext.ElectionType.ShuffleRemove, [2]);
        ElectionViewModel election = Create (context);
        election.CompletingElection += () => isCompleted = true;

        election.TryRunNextElectionCommand.Execute (null);

        Assert.IsTrue (isCompleted);
    }

    [TestMethod]
    public void TryRunNextElectionCommand_ExecuteFailure_DoesNothing () {
        ElectionContext context = new (ElectionContext.ElectionType.ShuffleAdd, [2], isLeaderNeeded: true);
        ElectionViewModel election = Create (context);
        election.CompletingElection += () => Assert.Fail ();
        election.RunElection ();

        election.TryRunNextElectionCommand.Execute (null);
    }
}
