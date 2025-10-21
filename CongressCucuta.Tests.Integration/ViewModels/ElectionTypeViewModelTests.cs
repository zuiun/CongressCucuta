//using CongressCucuta.Core.Data;
//using CongressCucuta.Tests.Unit.Fakes;
//using CongressCucuta.ViewModels;

//namespace CongressCucuta.Tests.Integration.ViewModels;

//[TestClass]
//public sealed class ElectionTypeViewModelTests {
//    private static ElectionTypeViewModel Create (Election election) {
//        Dictionary<IDType, SortedSet<IDType>> peopleRoles = [];
//        peopleRoles[0] = [2, Role.MEMBER];
//        peopleRoles[1] = [0, Role.MEMBER];
//        peopleRoles[2] = [3, Role.MEMBER];
//        peopleRoles[3] = [1, Role.MEMBER];
//        Dictionary<IDType, (IDType?, IDType?)> peopleFactions = [];
//        peopleFactions[0] = (2, 0);
//        peopleFactions[1] = (2, 1);
//        peopleFactions[2] = (3, 0);
//        peopleFactions[3] = (3, 1);
//        HashSet<IDType> partiesActive = [3];
//        HashSet<IDType> regionsActive = [0, 1];
//        List<string> people = ["0", "1", "2", "3"];

//        return ElectionTypeViewModel.Create (
//            new (election),
//            peopleRoles,
//            peopleFactions,
//            partiesActive,
//            regionsActive,
//            FakeLocalisation.Create (),
//            people
//        );
//    }

//    [TestMethod]
//    public void Constructor_ShuffleRemove_MutateExpected () {
//        IDType dissolveId = 2;
//        IDType remainingId = 3;
//        Election data = new (Election.ElectionType.ShuffleRemove, [dissolveId]);

//        ElectionTypeViewModel actual = Create (data);

//        Assert.DoesNotContain (dissolveId, actual.PeopleRolesNew[0]);
//        Assert.AreEqual (remainingId, actual.PeopleFactionsNew[0].Item1);
//        Assert.AreEqual (remainingId, actual.PeopleFactionsNew[1].Item1);
//        Assert.AreEqual (remainingId, actual.PeopleFactionsNew[2].Item1);
//        Assert.AreEqual (remainingId, actual.PeopleFactionsNew[3].Item1);
//    }

//    [TestMethod]
//    public void Constructor_ShuffleAdd_MutateExpected () {
//        IDType foundId = 3;
//        IDType originalId = 2;
//        Election data = new (Election.ElectionType.ShuffleAdd, [foundId]);

//        ElectionTypeViewModel actual = Create (data);

//        Assert.Contains (originalId, actual.PeopleRolesNew[0]);
//        Assert.DoesNotContain (foundId, actual.PeopleRolesNew[0]);
//        Assert.AreEqual (originalId, actual.PeopleFactionsNew[0].Item1);
//        Assert.IsTrue (actual.PeopleFactionsNew.Values.Any (f => f.Item2 == foundId));
//    }
//}
