using CongressCucuta.Core;
using CongressCucuta.ViewModels;

namespace CongressCucuta.Tests.Unit.ViewModels;

[TestClass]
public sealed class PersonViewModelTests {
    [TestMethod]
    [DataRow ("Role 1", "R1")]
    [DataRow ("Role-2", "R2")]
    [DataRow ("Role of 3", "Ro3")]
    public void GetRoles_Normal_ReturnsExpected (string role, string expected) {
        PersonViewModel person = new (0, "Person", true);

        person.Roles.Add (new (role));

        Assert.AreEqual (role, person.Roles[0].Name);
        Assert.Contains (expected, person.Roles[0].Abbreviation);
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void SetPass_Same_InvokesNothing (bool isPass) {
        PersonViewModel person = new (0, "Person", true) {
            IsPass = isPass,
        };
        person.VotingPass += Person_VotingPassEventHandler;

        person.IsPass = isPass;

        static void Person_VotingPassEventHandler (object? sender, bool e) {
            Assert.Fail ();
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void SetPass_Different_InvokesExpected (bool isPass) {
        PersonViewModel person = new (0, "Person", true) {
            IsPass = ! isPass,
        };
        person.VotingPass += Person_VotingPassEventHandler;

        person.IsPass = isPass;

        void Person_VotingPassEventHandler (object? sender, bool e) {
            PersonViewModel person = (PersonViewModel) sender!;

            Assert.AreEqual (isPass, person.IsPass);
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void SetFail_Same_InvokesNothing (bool isFail) {
        PersonViewModel person = new (0, "Person", true) {
            IsFail = isFail,
        };
        person.VotingFail += Person_VotingFailEventHandler;

        person.IsFail = isFail;

        static void Person_VotingFailEventHandler (object? sender, bool e) {
            Assert.Fail ();
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void SetFail_Different_InvokesExpected (bool isFail) {
        PersonViewModel person = new (0, "Person", true) {
            IsFail = ! isFail,
        };
        person.VotingFail += Person_VotingFailEventHandler;

        person.IsFail = isFail;

        void Person_VotingFailEventHandler (object? sender, bool e) {
            PersonViewModel person = (PersonViewModel) sender!;

            Assert.AreEqual (isFail, person.IsFail);
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void SetAbstain_Same_InvokesNothing (bool isAbstain) {
        PersonViewModel person = new (0, "Person", true) {
            IsAbstain = isAbstain,
        };
        person.VotingAbstain += Person_VotingAbstainEventHandler;

        person.IsAbstain = isAbstain;

        static void Person_VotingAbstainEventHandler (object? sender, bool e) {
            Assert.Fail ();
        }
    }

    [TestMethod]
    [DataRow (true)]
    [DataRow (false)]
    public void SetAbstain_Different_InvokesExpected (bool isAbstain) {
        PersonViewModel person = new (0, "Person", true) {
            IsAbstain = ! isAbstain,
        };
        person.VotingAbstain += Person_VotingAbstainEventHandler;

        person.IsAbstain = isAbstain;

        void Person_VotingAbstainEventHandler (object? sender, bool e) {
            PersonViewModel person = (PersonViewModel) sender!;

            Assert.AreEqual (isAbstain, person.IsAbstain);
        }
    }

    [TestMethod]
    public void SetInteractable_False_MutatesExpected () {
        PersonViewModel person = new (0, "Name", true);

        person.IsInteractable = false;

        Assert.IsFalse (person.IsPass);
        Assert.IsFalse (person.IsFail);
        Assert.IsTrue (person.IsAbstain);
    }

    [TestMethod]
    public void Reset_Normal_MutatesExpected () {
        PersonViewModel person = new (0, "Name", true) {
            IsPass = true,
            IsFail = true,
            IsAbstain = false,
        };

        person.Reset ();

        Assert.IsFalse (person.IsPass);
        Assert.IsFalse (person.IsFail);
        Assert.IsTrue (person.IsAbstain);
    }

    [TestMethod]
    public void UpdatePermissions_Normal_MutatesExpected () {
        Permissions permissions = new (false, 0, false);
        PersonViewModel person = new (0, "Name", true);

        person.UpdatePermissions (permissions);

        Assert.IsFalse (person.CanVote);
        Assert.AreEqual (0, person.Votes);
        Assert.IsFalse (person.CanSpeak);
    }
}
