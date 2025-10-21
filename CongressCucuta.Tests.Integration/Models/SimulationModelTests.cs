//using CongressCucuta.Core.Data;
//using CongressCucuta.Models;
//using CongressCucuta.Tests.Unit.Fakes;
//using CongressCucuta.ViewModels;

//namespace CongressCucuta.Tests.Unit.Models;

//[TestClass]
//public sealed class SimulationModelTests {
//    private static bool Contains (SlideModel slide, params string[] description)
//        => description.All (d => slide.Description.Any (l => l.Text.Contains (d)));

//    [TestMethod]
//    public void Constructor_Introduction_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 0;
//        string title = model.Localisation.State;
//        string description = model.Localisation.Government;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual (title, actual.Title);
//        Assert.IsFalse (actual.IsContent);
//        Assert.IsTrue (Contains (actual, description));
//        Assert.IsFalse (actual.Description[0].IsContent);
//        Assert.AreEqual<IDType> (1, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Context_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 1;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Context", actual.Title);
//        Assert.AreEqual<IDType> (0, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (2, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_ProcedureGovernmental_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 2;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Governmental Procedures", actual.Title);
//        Assert.IsTrue (Contains (actual, "Procedure 0"));
//        Assert.IsNotNull (actual.Description[0].Description);
//        Assert.AreEqual<IDType> (1, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (3, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_ProcedureSpecial_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 3;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Special Procedures", actual.Title);
//        Assert.IsTrue (Contains (actual, "Procedure 1"));
//        Assert.IsNotNull (actual.Description[0].Description);
//        Assert.AreEqual<IDType> (2, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (4, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_ProcedureDeclared_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 4;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Declared Procedures", actual.Title);
//        Assert.IsTrue (Contains (actual, "Procedure 3"));
//        Assert.IsNotNull (actual.Description[0].Description);
//        Assert.AreEqual<IDType> (3, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (5, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Roles_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 5;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Roles", actual.Title);
//        Assert.IsTrue (Contains (
//            actual,
//            "Speaker",
//            "Member",
//            "Government Head",
//            "State Head",
//            "Party Leader",
//            "Region Leader",
//            "0 Leader",
//            "1 Leader",
//            "2 Leader",
//            "3 Leader"
//        ));
//        Assert.AreEqual<IDType> (4, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (6, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Regions_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 6;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Regions", actual.Title);
//        Assert.IsTrue (Contains (actual, "Region 0", "Region 1"));
//        Assert.AreEqual<IDType> (5, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (7, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Parties_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 7;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Parties", actual.Title);
//        Assert.IsTrue (Contains (actual, "Party 2 (2)", "Party 3"));
//        Assert.AreEqual<IDType> (6, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (8, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Ballots_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 8;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Ballots", actual.Title);
//        Assert.IsTrue (Contains (actual, "0", "Ballot 0"));
//        Assert.IsFalse (Contains (actual, "1", "Incident 1"));
//        Assert.AreEqual<IDType> (7, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (9, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Title_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 9;
//        string title = model.Localisation.Period;
//        string date = model.Localisation.Date;
//        string situation = model.Localisation.Situation;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual (title, actual.Title);
//        Assert.IsFalse (actual.IsContent);
//        Assert.IsTrue (Contains (actual, date, situation));
//        Assert.IsFalse (actual.Description[0].IsContent);
//        Assert.IsFalse (actual.Description[1].IsContent);
//        Assert.AreEqual<IDType> (10, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Ballot0_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 10;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("0", actual.Title);
//        Assert.IsTrue (Contains (actual, "Ballot 0"));
//        Assert.AreEqual<IDType> (12, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (13, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Ballot1_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 11;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("1", actual.Title);
//        Assert.IsTrue (Contains (actual, "Incident 1"));
//        Assert.AreEqual<IDType> (14, actual.Links[0].TargetID);
//        Assert.AreEqual<IDType> (15, actual.Links[1].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Ballot0Pass_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 12;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("0 Passed", actual.Title);
//        Assert.IsTrue (Contains (actual, "Replace Procedure 1 with Procedure 2:"));
//        Assert.IsTrue (actual.Description[0].IsImportant);
//        Assert.AreEqual<IDType> (11, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Ballot0Fail_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 13;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("0 Failed", actual.Title);
//        Assert.AreEqual<IDType> (16, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Ballot1Pass_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 14;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("1 Passed", actual.Title);
//        Assert.AreEqual<IDType> (16, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Ballot1Fail_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 15;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("1 Failed", actual.Title);
//        Assert.AreEqual<IDType> (16, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_Result_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 16;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Result", actual.Title);
//        Assert.AreEqual<IDType> (17, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_HistoricalResults_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 17;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("Historical Results", actual.Title);
//        Assert.IsTrue (Contains (actual, "Passed", "0", "Failed", "1 (Procedure 3)"));
//        Assert.AreEqual<IDType> (18, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void Constructor_End_ConstructsExpected () {
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        IDType id = 18;

//        SlideModel actual = model.Slides[id];

//        Assert.AreEqual (id, actual.ID);
//        Assert.AreEqual ("End", actual.Title);
//        Assert.AreEqual<IDType> (17, actual.Links[0].TargetID);
//    }

//    [TestMethod]
//    public void SetSlideCurrentIdx_Title_MutatesExpected () {
//        IDType personId = 0;
//        IDType factionId = 2;
//        Dictionary<IDType, sbyte> currenciesValues = [];
//        void Model_CompletingElectionEventHandler (CompletingElectionEventArgs e) {
//            e.PeopleRolesNew[personId] = [factionId];
//            e.PeopleFactionsNew[personId] = (factionId, factionId);
//        }
//        void Context_ModifiedCurrenciesEventHandler (Dictionary<IDType, sbyte> e) {
//            currenciesValues = e;
//        }
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        model.CompletingElection += Model_CompletingElectionEventHandler;
//        model.Context.ModifiedCurrencies += Context_ModifiedCurrenciesEventHandler;
//        IDType title = 9;

//        model.SlideCurrentIdx = title;

//        Assert.Contains (factionId, model.Context.PeopleRoles[personId]);
//        Assert.AreEqual (factionId, model.Context.PeopleFactions[personId].Item1);
//        Assert.AreEqual (factionId, model.Context.PeopleFactions[personId].Item2);
//        Assert.AreEqual (1, currenciesValues[0]);
//        Assert.AreEqual (1, currenciesValues[1]);
//        Assert.AreEqual (1, currenciesValues[2]);
//        Assert.AreEqual (1, currenciesValues[3]);
//        Assert.AreEqual (1, currenciesValues[Currency.STATE]);
//    }

//    [TestMethod]
//    [DataRow ((byte) 10, (byte) 0)]
//    [DataRow ((byte) 11, (byte) 1)]
//    public void SetSlideCurrentIdx_Ballot_MutatesExpected (byte ballotIdx, byte ballotId) {
//        byte votesResultThreshold = 2;
//        byte votesResult = 0;
//        byte votesTotal = 3;
//        byte votesPassThreshold = byte.MaxValue;
//        byte votesFailThreshold = byte.MaxValue;
//        byte votesPass = byte.MaxValue;
//        byte votesFail = byte.MaxValue;
//        byte votesAbstain = byte.MaxValue;
//        List<ElectionModel> elections = [];
//        SortedSet<IDType> roles0 = [0, 2, 255];
//        SortedSet<IDType> roles1 = [1, 255];
//        SortedSet<IDType> roles2 = [3, 255];
//        (IDType, IDType) factions0 = (2, 0);
//        (IDType, IDType) factions1 = (2, 1);
//        (IDType, IDType) factions2 = (3, 0);
//        void Model_CompletingElectionEventHandler (CompletingElectionEventArgs e) {
//            elections = e.Elections;
//            e.PeopleRolesNew[0] = roles0;
//            e.PeopleRolesNew[1] = roles1;
//            e.PeopleRolesNew[2] = roles2;
//            e.PeopleFactionsNew[0] = factions0;
//            e.PeopleFactionsNew[1] = factions1;
//            e.PeopleFactionsNew[2] = factions2;
//        }
//        void Model_StartingBallotEventHandler (StartingBallotEventArgs e) {
//            votesPassThreshold = e.VotesPassThreshold;
//            votesFailThreshold = e.VotesFailThreshold;
//            votesPass = e.VotesPass;
//            votesFail = e.VotesFail;
//            votesAbstain = e.VotesAbstain;
//        }
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        model.Context.InitialisePeople ([new (0, "0"), new (1, "1"), new (2, "2")]);
//        model.CompletingElection += Model_CompletingElectionEventHandler;
//        model.StartingBallot += Model_StartingBallotEventHandler;
//        IDType title = 9;

//        model.SlideCurrentIdx = title;
//        model.SlideCurrentIdx = ballotIdx;

//        Assert.AreEqual<IDType> (ballotId, model.Context.BallotCurrentID);
//        Assert.IsTrue (model.Context.IsBallot);
//        Assert.AreEqual (votesResultThreshold, votesPassThreshold);
//        Assert.AreEqual (votesResultThreshold, votesFailThreshold);
//        Assert.AreEqual (votesResult, votesPass);
//        Assert.AreEqual (votesResult, votesFail);
//        Assert.AreEqual (votesTotal, votesAbstain);
//        CollectionAssert.AreEqual (roles0, model.Context.PeopleRoles[0]);
//        CollectionAssert.AreEqual (roles1, model.Context.PeopleRoles[1]);
//        CollectionAssert.AreEqual (roles2, model.Context.PeopleRoles[2]);
//        Assert.AreEqual (factions0, model.Context.PeopleFactions[0]);
//        Assert.AreEqual (factions1, model.Context.PeopleFactions[1]);
//        Assert.AreEqual (factions2, model.Context.PeopleFactions[2]);
//        Assert.IsNotEmpty (elections);
//        Assert.AreEqual<IDType> (0, (IDType) elections[0].ProcedureID!);
//        Assert.AreEqual (Role.HEAD_STATE, elections[0].TargetID);
//        Assert.Contains (Role.HEAD_GOVERNMENT, elections[0].FilterIDs);
//        Assert.IsTrue (elections[0].IsRandom);
//        Assert.AreEqual (Election.ElectionType.Appointed, elections[0].Type);
//    }

//    [TestMethod]
//    [DataRow ((byte) 10, (byte) 12)]
//    [DataRow ((byte) 11, (byte) 12)]
//    public void SetSlideCurrentIdx_NotBallot_MutatesExpected (byte ballotIdx, byte notBallotIdx) {
//        bool isEndingBallot = false;
//        void Model_EndingBallotEventHandler () {
//            isEndingBallot = true;
//        }
//        FakeSimulation simulation = new ();
//        SimulationModel model = new (simulation);
//        model.EndingBallot += Model_EndingBallotEventHandler;

//        model.SlideCurrentIdx = ballotIdx;
//        model.SlideCurrentIdx = notBallotIdx;

//        Assert.IsFalse (model.Context.IsBallot);
//        Assert.IsTrue (isEndingBallot);
//    }

//    [TestMethod]
//    public void VotingEventHandler_Pass_MutatesExpected () {
//        FakeSimulation simulation = new ();
//        void Model_CompletingElectionEventHandler (CompletingElectionEventArgs e) {
//            e.PeopleRolesNew[0] = [255];
//            e.PeopleRolesNew[1] = [255];
//            e.PeopleRolesNew[2] = [255];
//            e.PeopleFactionsNew[0] = (null, null);
//            e.PeopleFactionsNew[1] = (null, null);
//            e.PeopleFactionsNew[2] = (null, null);
//        }
//        SimulationModel model = new (simulation);
//        model.Context.InitialisePeople ([new (0, "0"), new (1, "1"), new (2, "2")]);
//        model.CompletingElection += Model_CompletingElectionEventHandler;
//        VotingEventArgs args = new (0, true, null, null) {
//            VotesPass = 255,
//            VotesFail = 255,
//            VotesAbstain = 255,
//        };
//        IDType title = 9;
//        model.SlideCurrentIdx = title;

//        model.Context_VotingEventHandler (args);

//        Assert.AreEqual (1, args.VotesPass);
//        Assert.AreEqual (0, args.VotesFail);
//        Assert.AreEqual (2, args.VotesAbstain);
//    }

//    [TestMethod]
//    public void VotingEventHandler_Fail_MutatesExpected () {
//        FakeSimulation simulation = new ();
//        void Model_CompletingElectionEventHandler (CompletingElectionEventArgs e) {
//            e.PeopleRolesNew[0] = [255];
//            e.PeopleRolesNew[1] = [255];
//            e.PeopleRolesNew[2] = [255];
//            e.PeopleFactionsNew[0] = (null, null);
//            e.PeopleFactionsNew[1] = (null, null);
//            e.PeopleFactionsNew[2] = (null, null);
//        }
//        SimulationModel model = new (simulation);
//        model.Context.InitialisePeople ([new (0, "0"), new (1, "1"), new (2, "2")]);
//        model.CompletingElection += Model_CompletingElectionEventHandler;
//        VotingEventArgs args = new (0, null, true, null) {
//            VotesPass = 255,
//            VotesFail = 255,
//            VotesAbstain = 255,
//        };
//        IDType title = 9;
//        model.SlideCurrentIdx = title;

//        model.Context_VotingEventHandler (args);

//        Assert.AreEqual (0, args.VotesPass);
//        Assert.AreEqual (1, args.VotesFail);
//        Assert.AreEqual (2, args.VotesAbstain);
//    }

//    [TestMethod]
//    public void VotingEventHandler_Abstain_MutatesExpected () {
//        FakeSimulation simulation = new ();
//        void Model_CompletingElectionEventHandler (CompletingElectionEventArgs e) {
//            e.PeopleRolesNew[0] = [255];
//            e.PeopleRolesNew[1] = [255];
//            e.PeopleRolesNew[2] = [255];
//            e.PeopleFactionsNew[0] = (null, null);
//            e.PeopleFactionsNew[1] = (null, null);
//            e.PeopleFactionsNew[2] = (null, null);
//        }
//        SimulationModel model = new (simulation);
//        model.Context.InitialisePeople ([new (0, "0"), new (1, "1"), new (2, "2")]);
//        model.CompletingElection += Model_CompletingElectionEventHandler;
//        VotingEventArgs args = new (0, null, null, true) {
//            VotesPass = 255,
//            VotesFail = 255,
//            VotesAbstain = 255,
//        };
//        IDType title = 9;
//        model.SlideCurrentIdx = title;

//        model.Context_VotingEventHandler (args);

//        Assert.AreEqual (0, args.VotesPass);
//        Assert.AreEqual (0, args.VotesFail);
//        Assert.AreEqual (3, args.VotesAbstain);
//    }
//}
