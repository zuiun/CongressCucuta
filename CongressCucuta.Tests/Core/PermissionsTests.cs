using CongressCucuta.Core;

namespace CongressCucuta.Tests.Unit.Core;

[TestClass]
public sealed class PermissionsTests {
    [TestMethod]
    [DataRow (true, null, true)]
    [DataRow (false, null, false)]
    [DataRow (true, true, true)]
    [DataRow (true, false, false)]
    [DataRow (false, true, true)]
    [DataRow (false, false, false)]
    public void Add_PermissionsCompositionCanVote_ReturnsExpected (bool canVoteLeft, bool? canVoteRight, bool expected) {
        Permissions left = new (canVoteLeft);
        Permissions.Composition right = new (CanVote: canVoteRight);

        Permissions actual = left + right;

        Assert.AreEqual (expected, actual.CanVote);
    }

    [TestMethod]
    [DataRow ((byte) 1, null, (byte) 1)]
    [DataRow ((byte) 1, (byte) 0, (byte) 1)]
    [DataRow ((byte) 1, (byte) 1, (byte) 2)]
    public void Add_PermissionsCompositionVotes_ReturnsExpected (byte votesLeft, byte? votesRight, byte expected) {
        Permissions left = new (true, Votes: votesLeft);
        Permissions.Composition right = new (Votes: votesRight);

        Permissions actual = left + right;

        Assert.AreEqual (expected, actual.Votes);
    }

    [TestMethod]
    [DataRow (true, null, true)]
    [DataRow (false, null, false)]
    [DataRow (true, true, true)]
    [DataRow (true, false, false)]
    [DataRow (false, true, true)]
    [DataRow (false, false, false)]
    public void Add_PermissionsCompositionCanSpeak_ReturnsExpected (bool canSpeakLeft, bool? canSpeakRight, bool expected) {
        Permissions left = new (true, CanSpeak: canSpeakLeft);
        Permissions.Composition right = new (CanSpeak: canSpeakRight);

        Permissions actual = left + right;

        Assert.AreEqual (expected, actual.CanSpeak);
    }

    [TestMethod]
    [DataRow (true, true, true)]
    [DataRow (true, false, false)]
    [DataRow (false, true, false)]
    [DataRow (false, false, false)]
    public void Add_PermissionsPermissionsCanVote_ReturnsExpected (bool canVoteLeft, bool canVoteRight, bool expected) {
        Permissions left = new (canVoteLeft);
        Permissions right = new (canVoteRight);

        Permissions actual = left + right;

        Assert.AreEqual (expected, actual.CanVote);
    }

    [TestMethod]
    [DataRow ((byte) 1, (byte) 0, (byte) 1)]
    [DataRow ((byte) 1, (byte) 1, (byte) 1)]
    [DataRow ((byte) 1, (byte) 2, (byte) 2)]
    public void Add_PermissionsPermissionsVotes_ReturnsExpected (byte votesLeft, byte votesRight, byte expected) {
        Permissions left = new (true, Votes: votesLeft);
        Permissions right = new (true, votesRight);

        Permissions actual = left + right;

        Assert.AreEqual (expected, actual.Votes);
    }

    [TestMethod]
    [DataRow (true, true, true)]
    [DataRow (true, false, false)]
    [DataRow (false, true, false)]
    [DataRow (false, false, false)]
    public void Add_PermissionsPermissionsCanSpeak_ReturnsExpected (bool canSpeakLeft, bool canSpeakRight, bool expected) {
        Permissions left = new (true, CanSpeak: canSpeakLeft);
        Permissions right = new (true, CanSpeak: canSpeakRight);

        Permissions actual = left + right;

        Assert.AreEqual (expected, actual.CanSpeak);
    }

    [TestMethod]
    [DataRow (true, "Can vote")]
    [DataRow (false, "Cannot vote")]
    public void ToString_PermissionsCanVote_ReturnsExpected (bool canVote, string expected) {
        Permissions permissions = new (canVote);

        string actual = permissions.ToString ();

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    [DataRow ((byte) 1, "Has 1 vote(s)")]
    [DataRow ((byte) 2, "Has 2 vote(s)")]
    public void ToString_PermissionsVotes_ReturnsExpected (byte votes, string expected) {
        Permissions permissions = new (true, Votes: votes);

        string actual = permissions.ToString ();

        Assert.Contains (expected, actual);
    }

    [TestMethod]
    public void ToString_PermissionsCanSpeakTrue_ReturnsExpected () {
        Permissions permissions = new (true, CanSpeak: true);

        string expected = "Cannot speak";
        string actual = permissions.ToString ();

        Assert.DoesNotContain (expected, actual);
    }

    [TestMethod]
    public void ToString_PermissionsCanSpeakFalse_ReturnsExpected () {
        Permissions permissions = new (true, CanSpeak: false);

        string expected = "Cannot speak";
        string actual = permissions.ToString ();

        Assert.Contains (expected, actual);
    }
}
