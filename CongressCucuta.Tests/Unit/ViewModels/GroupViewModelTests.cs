using CongressCucuta.Core;
using CongressCucuta.Core.Contexts;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class GroupViewModelTests {
    [TestMethod]
    public void IsSelected_Selected_ReturnsTrue () {
        ElectionContext.Group group = new (0, [0, 255], []);
        group.PeopleAreCandidates[0] = true;
        group.PeopleAreCandidates[1] = false;
        Dictionary<IDType, string> peopleNames = [];
        peopleNames[0] = "Person 0";
        peopleNames[1] = "Person 1";
        GroupViewModel vm = new (group, "0", peopleNames);
        vm.People[0].IsSelected = true;

        bool actual = vm.IsSelected ();

        Assert.IsTrue (actual);
    }

    [TestMethod]
    public void IsSelected_NotSelected_ReturnsFalse () {
        ElectionContext.Group group = new (0, [0, 255], []);
        group.PeopleAreCandidates[0] = true;
        group.PeopleAreCandidates[1] = false;
        Dictionary<IDType, string> peopleNames = [];
        peopleNames[0] = "Person 0";
        peopleNames[1] = "Person 1";
        GroupViewModel vm = new (group, "0", peopleNames);

        bool actual = vm.IsSelected ();

        Assert.IsFalse (actual);
    }

    [TestMethod]
    public void IsSelected_NoCandidates_ReturnsTrue () {
        ElectionContext.Group group = new (0, [0, 255], []);
        group.PeopleAreCandidates[0] = false;
        group.PeopleAreCandidates[1] = false;
        Dictionary<IDType, string> peopleNames = [];
        peopleNames[0] = "Person 0";
        peopleNames[1] = "Person 1";
        GroupViewModel vm = new (group, "0", peopleNames);

        bool actual = vm.IsSelected ();

        Assert.IsTrue (actual);
    }
}
