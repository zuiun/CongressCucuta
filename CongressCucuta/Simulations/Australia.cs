using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Simulations;

[ExcludeFromCodeCoverage]
internal class Australia : ISimulation {
    public Simulation Simulation { get; }

    public Australia () {
        IDType member = Role.MEMBER;
        IDType primeMinister = Role.HEAD_GOVERNMENT;
        IDType governorGeneral = Role.HEAD_STATE;
        List<IDType> roles = [
            member,
            primeMinister,
            governorGeneral,
            0,
            1,
            Role.LEADER_PARTY,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[member] = ("Member", "Members");
        rolesLocs[primeMinister] = ("Prime Minister", "Prime Ministers");
        rolesLocs[governorGeneral] = ("Governor-General", "Governors-General");
        rolesLocs[0] = ("Leader of LP-CP", "Leaders of LP-CP");
        rolesLocs[1] = ("Leader of ALP", "Leaders of ALP");
        rolesLocs[Role.LEADER_PARTY] = ("Party Leader", "Party Leaders");

        Faction lpcp = new (0);
        Faction alp = new (1);
        List<Faction> parties = [lpcp, alp];
        Dictionary<IDType, (string, string[])> partiesLocs = [];
        partiesLocs[lpcp.ID] = (
            "Liberal-Country Coalition",
            [
                StringLineFormatter.Indent ("Centre-right conservative", 1),
                StringLineFormatter.Indent ("Supported by a broad spectrum of rural and liberal (middle-class) interests", 1),
            ]
        );
        partiesLocs[alp.ID] = (
            "Australian Labor Party",
            [
                StringLineFormatter.Indent ("Left, democratic socialist", 1),
                StringLineFormatter.Indent ("Supported by workers", 1),
                StringLineFormatter.Indent ("Opposed to foreign labour", 2),
                StringLineFormatter.Indent ("Supports welfare and social programmes at any cost", 1),
            ]
        );
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[lpcp.ID] = "LP-CP";
        abbreviations[alp.ID] = "ALP";

        ProcedureImmediate commonwealthRealm = new (
            0,
            [
                new (Procedure.Effect.EffectType.ElectionAppointed, [governorGeneral], 1),
                new (Procedure.Effect.EffectType.PermissionsCanVote, [governorGeneral], 0),
            ]
        );
        ProcedureImmediate generalElection = new (
            1,
            [
                new (Procedure.Effect.EffectType.ElectionParty, [governorGeneral]),
                new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister, governorGeneral]),
            ]
        );
        ProcedureImmediate duumvirate = new (
            2,
            [new (Procedure.Effect.EffectType.PermissionsVotes, [primeMinister], 1)]
        );
        List<ProcedureImmediate> proceduresGovernmental = [commonwealthRealm, generalElection, duumvirate];
        ProcedureTargeted itsTime = new (
            3,
            [new (Procedure.Effect.EffectType.VotePassAdd, [], 2)],
            []
        );
        ProcedureTargeted whiteAustralia = new (
            4,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            [0, 1]
        );
        ProcedureTargeted luckyCountry = new (
            5,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            []
        );
        ProcedureTargeted beJustFearNot = new (
            6,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            [],
            false
        );
        ProcedureTargeted darkYears = new (
            7,
            [
                new (Procedure.Effect.EffectType.VotePassTwoThirds, []),
                new (Procedure.Effect.EffectType.PermissionsVotes, [lpcp.ID], 1),
            ],
            [],
            false
        );
        ProcedureTargeted stagflation = new (
            8,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            itsTime,
            whiteAustralia,
            luckyCountry,
            beJustFearNot,
            darkYears,
            stagflation,
        ];
        ProcedureDeclared voteNoConfidence = new (
            9,
            [new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister, governorGeneral])],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            []
        );
        ProcedureDeclared dismissal = new (
            10,
            [new (Procedure.Effect.EffectType.ElectionAppointed, [primeMinister, governorGeneral])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [governorGeneral]
        );
        ProcedureDeclared viceregalAppointment = new (
            11,
            [new (Procedure.Effect.EffectType.ElectionAppointed, [governorGeneral, primeMinister, Role.LEADER_PARTY])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [primeMinister]
        );
        ProcedureDeclared doubleDissolution = new (
            12,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [primeMinister]
        );
        ProcedureDeclared refusalSupply = new (
            13,
            [new (Procedure.Effect.EffectType.ElectionParty, [governorGeneral])],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            [Role.LEADER_PARTY]
        );
        List<ProcedureDeclared> proceduresDeclared = [voteNoConfidence, dismissal, viceregalAppointment, doubleDissolution, refusalSupply];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[commonwealthRealm.ID] = (
            "Commonwealth Realm",
            "This country's head of state is chosen based on familial lineage, but exercises his prerogatives through a representative."
        );
        procedures[generalElection.ID] = (
            "General Election",
            "This country has a legislature with regular elections, along with an empowered head of government."
        );
        procedures[duumvirate.ID] = (
            "Duumvirate",
            "The ALP has been in the opposition for decades now, and its leaders know that they have to act quickly and decisively to push through reforms."
        );
        procedures[itsTime.ID] = (
            "It's Time",
            "It's time for a new team, a new program, a new drive for equality of opportunities: it's time to create new opportunities for Australians, time for a new vision of what we can achieve in this generation for our nation and the region in which we live."
        );
        procedures[whiteAustralia.ID] = (
            "White Australia",
            "Australia was established as a white country and thus actively restricts access to the government and even into the country for non-whites. This attitude is slowly changing, but it remains government policy for now."
        );
        procedures[luckyCountry.ID] = (
            "The Lucky Country",
            "Australia owes her prosperity to her outstanding luck - and nothing else. Any surprise could devolve into a full-blown economic crisis, should the government not be prepared to meet it."
        );
        procedures[beJustFearNot.ID] = (
            "Be Just and Fear Not",
            Localisation.UNUSED
        );
        procedures[darkYears.ID] = (
            "Dark Years",
            Localisation.UNUSED
        );
        procedures[stagflation.ID] = (
            "Stagflation",
            Localisation.UNUSED
        );
        procedures[voteNoConfidence.ID] = (
            "Vote of No Confidence",
            "This country's head of government is responsible to the legislature, which means that he can be removed if he is seen to be inept."
        );
        procedures[dismissal.ID] = (
            "Dismissal",
            "This country's head of government is responsible to the head of state, which means that he can be removed if he is seen to be inept."
        );
        procedures[viceregalAppointment.ID] = (
            "Viceregal Appointment",
            "By convention, the Governor-General is replaced every five years."
        );
        procedures[doubleDissolution.ID] = (
            "Double Dissolution",
            "This country's legislature can be dissolved in times of political deadlock. Under certain conditions, this can be followed by a joint sitting in order to force through a bill's passage."
        );
        procedures[refusalSupply.ID] = (
            "Refusal of Supply",
            "It is convention in this country to dissolve the legislature upon the rejection of appropriation bills."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [whiteAustralia.ID, beJustFearNot.ID])],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 1)]
            )
        );
        Ballot ballotB = new (
            1,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [whiteAustralia.ID, darkYears.ID])],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 2)]
            )
        );
        Ballot ballotC = new (
            2,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [luckyCountry.ID, stagflation.ID])],
                [
                    new (new BallotsPassedCountCondition (ComparisonType.GreaterThan, 0), 3),
                    new (new BallotsPassedCountCondition (ComparisonType.Equal, 0), Ballot.END),
                ]
            ),
            new Ballot.Result (
                [],
                [
                    new (new BallotsPassedCountCondition (ComparisonType.GreaterThan, 0), 3),
                    new (new BallotsPassedCountCondition (ComparisonType.Equal, 0), Ballot.END),
                ]
            )
        );
        Ballot incidentA = new (
            3,
            new Ballot.Result (
                [],
                []
            ),
            new Ballot.Result (
                [],
                []
            ),
            true
        );
        List<Ballot> ballots = [ballotA, ballotB, ballotC, incidentA];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Establish the Department of Aboriginal Affairs",
            [
                StringLineFormatter.Indent ("Department of Aboriginal Affairs:", 1),
                StringLineFormatter.Indent ("Consultative body elected by aborigines", 2),
                StringLineFormatter.Indent ("Responsible for formulating economic and social policy for the development of aboriginal communities", 2),
                StringLineFormatter.Indent ("Can propose special laws to protect aborigines", 2),
                StringLineFormatter.Indent ("This will not grant aboriginal sovereignty, but will be seen as a step towards it regardless", 1),
                StringLineFormatter.Indent ("There is significant opposition to aboriginal land ownership", 2),
                StringLineFormatter.Indent ("Rural farmers and corporate interests want their land for resource exploitation", 3),
                StringLineFormatter.Indent ("This may lead to further reform in the future", 2),
            ],
            [
                "Government is seen as favouring indigenes",
                StringLineFormatter.Indent ("This may harm resource exploitation", 1),
                StringLineFormatter.Indent ("Some criticise this arguably unnecessary budgetary expenditure", 1),
                StringLineFormatter.Indent ("This is still a relatively cheap endeavour", 2),
                StringLineFormatter.Indent ("Others applaud aboriginal emancipation", 1),
            ],
            [
                "Aborigines remain without much of a voice in the government",
                StringLineFormatter.Indent ("This benefits resource exploitation on aboriginal land", 1),
                StringLineFormatter.Indent ("Perhaps society is too hostile to change for aboriginal emancipation right now", 1),
                "This blocks allocating funds for other purposes",
                StringLineFormatter.Indent ("However, funding a consultative body isn't particularly expensive anyway", 1),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Dismantle the White Australia policy",
            [
                StringLineFormatter.Indent ("Immigrants are currently discriminated against based on skin colour and national origin", 1),
                StringLineFormatter.Indent ("This mainly affects Asians", 2),
                StringLineFormatter.Indent ("Chinese and Muslims especially are considered a security threat", 3),
                StringLineFormatter.Indent ("This protects Australia's cultural heritage, which is supposed to be British Protestant", 2),
                StringLineFormatter.Indent ("Australia has a relatively small population and little natural population growth, which might become an issue", 2),
                StringLineFormatter.Indent ("This policy is opposed across the entire world", 1),
                StringLineFormatter.Indent ("Racism is obviously not very popular", 2),
            ],
            [
                "This may have many effects in the future, especially economic growth",
                StringLineFormatter.Indent ("Right now, however, most immigrants are anti-communist refugees", 1),
                StringLineFormatter.Indent ("This is beginning to lead to a political shift", 2),
                StringLineFormatter.Indent ("Given the current economic situation, these refugees compete with native Australians for vanishingly few jobs", 2),
            ],
            [
                "This is a major setback",
                StringLineFormatter.Indent ("Maintaining the White Australia policy is generally economically harmful", 1),
                StringLineFormatter.Indent ("Australia draws worldwide condemnation for continuing its discriminatory immigration policy", 1),
                "Conservatives fear excessive Asian immigration and are happy",
                StringLineFormatter.Indent ("This will lead to a strengthening of British identity", 1),
                StringLineFormatter.Indent ("This has effects on issues such as constitutional independence, the national flag, and the monarchy", 2),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Establish Medibank",
            [
                StringLineFormatter.Indent ("Medibank:", 1),
                StringLineFormatter.Indent ("Public health insurance", 2),
                StringLineFormatter.Indent ("All are enrolled", 3),
                StringLineFormatter.Indent ("Private health insurance is not an option", 4),
                StringLineFormatter.Indent ("Funded through general taxation", 2),
                StringLineFormatter.Indent ("Does not require levying a separate tax to fund", 3),
                StringLineFormatter.Indent ("This will require cuts in other sectors", 3),
                StringLineFormatter.Indent ("Allows free hospital access for all", 2),
                StringLineFormatter.Indent ("Universal healthcare is popular, but not this proposal", 1),
                StringLineFormatter.Indent ("However, Medibank could be the basis of a reformed healthcare system in the future", 2),
            ],
            [
                "There is immediate opposition to Medibank",
                StringLineFormatter.Indent ("Many want the ability to opt out", 2),
                StringLineFormatter.Indent ("It expands very rapidly, draining the budget", 2),
                StringLineFormatter.Indent ("This is becoming a serious budgetary concern", 3),
                StringLineFormatter.Indent ("Rebates are somewhat expensive", 2),
                StringLineFormatter.Indent ("Medibank is still somewhat successful", 1),
                StringLineFormatter.Indent ("Most of Australia now has free access to hospitals", 2),
            ],
            [
                "This is not a permanent defeat for universal healthcare",
                StringLineFormatter.Indent ("However, there may not be another opportunity for a system as radical as Medibank", 1),
                StringLineFormatter.Indent ("Talks are already beginning for a drastically reduced system", 1),
                StringLineFormatter.Indent ("There are still some major fears that there will never be universal healthcare", 1),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Seek development loans",
            [
                StringLineFormatter.Indent ("Money for development projects has run dry due to social programmes", 1),
                StringLineFormatter.Indent ("Middle Eastern lenders are preferred for their lower interest rates", 1),
                StringLineFormatter.Indent ("However, this is politically very dangerous", 2),
                StringLineFormatter.Indent ("These loans would be based on continued oil revenues, which are currently high due to the Arab-Israeli conflict", 3),
                StringLineFormatter.Indent ("This bypasses traditional American lenders, who tend to ask for joint ownership of profits", 3),
                StringLineFormatter.Indent ("This process may take years to complete due to lack of trust and business ties", 2),
            ],
            [
                "Middle Eastern lenders refuse to offer loans",
                StringLineFormatter.Indent ("This is a major embarrassment", 1),
                StringLineFormatter.Indent ("Government is unable to fund development while spending on apparent vanity projects", 2),
                StringLineFormatter.Indent ("This provokes major outrage", 1),
                StringLineFormatter.Indent ("Middle Eastern oil producers have been benefitting from the oil price crisis at Australia's expense", 2),
                "Money is eventually raised with American aid",
                StringLineFormatter.Indent ("However, the reputational damage has already been dealt", 1),
            ],
            [
                "Government will eventually have to make a choice between raising taxes and making cuts",
                StringLineFormatter.Indent ("It is not sustainable to fund social programmes at the cost of economic development", 1),
                StringLineFormatter.Indent ("Government is criticised for its incoherent economic policy", 1),
                "This is especially controversial because Australia has massive mineral wealth that it could exploit",
                StringLineFormatter.Indent ("This is opposed by aboriginal communities and environmental activists", 1),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 1),
                new (new BallotPassedCondition (incidentA.ID, false), 5),
            ]
        );
        Result incidentAPassed = new (
            1,
            [
                new (new BallotsPassedCountCondition (ComparisonType.Equal, 2), 2),
                new (new BallotsPassedCountCondition (ComparisonType.Equal, 3), 3),
                new (new BallotsPassedCountCondition (ComparisonType.Equal, 4), 4),
            ]
        );
        Result twoPassed = new (
            2,
            []
        );
        Result threePassed = new (
            3,
            []
        );
        Result fourPassed = new (
            4,
            []
        );
        Result incidentAFailed = new (
            5,
            [
                new (new BallotsPassedCountCondition (ComparisonType.FewerThanOrEqual, 1), 6),
                new (new BallotsPassedCountCondition (ComparisonType.GreaterThanOrEqual, 2), 7),
            ]
        );
        Result oneFewerEqualPassed = new (
            6,
            []
        );
        Result twoGreaterEqualPassed = new (
            7,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 8),
                new (new BallotPassedCondition (ballotB.ID, false), 9),
            ]
        );
        Result ballotBPassed = new (
            8,
            []
        );
        Result ballotBFailed = new (
            9,
            []
        );
        List<Result> results = [
            resultStart,
            incidentAPassed,
            twoPassed,
            threePassed,
            fourPassed,
            incidentAFailed,
            oneFewerEqualPassed,
            twoGreaterEqualPassed,
            ballotBPassed,
            ballotBFailed,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[incidentAPassed.ID] = (
            "Incident A Passed",
            [
                "Attempts to raise unconventional loans badly tarnish the government's reputation",
                StringLineFormatter.Indent ("There is little chance for reforms to continue", 1),
            ]
        );
        resultsLocs[twoPassed.ID] = (
            "Two Ballots Passed",
            [
                "Reforms are seen as surface level",
                StringLineFormatter.Indent ("Widespread disillusionment with the government forces it from power in the next election", 1),
                StringLineFormatter.Indent ("There is little hope for more significant reforms, such as constitutional independence or republicanism", 1),
                "Australia must focus on repairing the economy for now",
                StringLineFormatter.Indent ("Stagnation will continue for the time being", 1),
            ]
        );
        resultsLocs[threePassed.ID] = (
            "Three Ballots Passed",
            [
                "Slower pace of reforms limits opposition to the government",
                StringLineFormatter.Indent ("More significant reforms may come in the future", 1),
                StringLineFormatter.Indent ("Current reforms may already have major effects", 1),
                StringLineFormatter.Indent ("Increased acceptance of non-British Australians may eventually create a more expansive national identity", 2),
                "Though the government is in crisis, there is still appetite for reform",
                StringLineFormatter.Indent ("However, it will take some time for the scars of the so-called \"Dark Years\" to heal", 1),
            ]
        );
        resultsLocs[fourPassed.ID] = (
            "Four Ballots Passed",
            [
                "Reforms have changed Australia's landscape forever",
                StringLineFormatter.Indent ("Increased acceptance of non-British Australians will eventually create a more expansive national identity", 1),
                StringLineFormatter.Indent ("This will likely lead to debate over national symbols, such as the national flag and the monarchy", 2),
                StringLineFormatter.Indent ("This era will be remembered as the one which created modern Australia", 1),
                StringLineFormatter.Indent ("It seems likely that Australia will become a republic in the distant future", 1),
            ]
        );
        resultsLocs[incidentAFailed.ID] = (
            "Incident A Failed",
            [
                "Whether due to inaction or financial mismanagement, the government runs out of funds for further reforms and development",
                StringLineFormatter.Indent ("This bodes poorly for economic growth", 1),
            ]
        );
        resultsLocs[oneFewerEqualPassed.ID] = (
            "One or Fewer Ballots Passed",
            [
                "Government chooses to cut spending",
                StringLineFormatter.Indent ("In the short term, this prevents reforms from taking place", 1),
                StringLineFormatter.Indent ("In the long term, this practically guarantees continued stagnation", 1),
                "This comes as a major disappointment",
                StringLineFormatter.Indent ("This was the perfect chance for reform and it was squandered", 1),
                StringLineFormatter.Indent ("Trust in the government collapses", 1),
            ]
        );
        resultsLocs[twoGreaterEqualPassed.ID] = (
            "Two or Greater Ballots Passed",
            [
                "Government chooses to raise taxes",
                StringLineFormatter.Indent ("Effects of these taxes cause significant concern", 1),
                StringLineFormatter.Indent ("Economy is still in a difficult state", 2),
                StringLineFormatter.Indent ("However, this leaves open the possibility of future reforms", 1),
            ]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Reforms have significantly changed Australia's landscape",
                StringLineFormatter.Indent ("Increased acceptance of non-British Australians will eventually create a more expansive national identity", 1),
                StringLineFormatter.Indent ("This will likely lead to debate over national symbols, such as the national flag and the institution of the monarchy", 2),
                StringLineFormatter.Indent ("However, the cost of these reforms is controversial", 1),
                StringLineFormatter.Indent ("Some may be reversed if they remain difficult to fund", 2),
                StringLineFormatter.Indent ("Government has some difficult choices to make regarding the economy", 2),
                StringLineFormatter.Indent ("Luckily, it continues to wield significant political capital", 3),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Australia maintains its traditional ties to the UK",
                StringLineFormatter.Indent ("This has negative effects on economic growth", 1),
                StringLineFormatter.Indent ("There is little hope for more significant reforms, such as constitutional independence or republicanism", 1),
                "More significant reforms may come in the future",
                StringLineFormatter.Indent ("However, Australia does not seem to be changing significantly for the time being", 1),
                StringLineFormatter.Indent ("This is widely seen as a disappointment, but perhaps this was the best option given the prevailing economic situation", 1),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[ballotC.ID] = [doubleDissolution.ID];
        ballotsProceduresDeclared[incidentA.ID] = [dismissal.ID, refusalSupply.ID];
        History history = new (
            [ballotA.ID, ballotB.ID, ballotC.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            [],
            parties,
            [],
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Australia",
                "Constitutional Monarchy",
                [
                    "Australia has been stagnating for 23 years",
                    StringLineFormatter.Indent ("Liberal-Country Coalition (LP-CP) has enjoyed unprecedented support, but this is now ending", 1),
                    StringLineFormatter.Indent ("Interventions in foreign wars, such as in Vietnam, have been very unpopular", 2),
                    StringLineFormatter.Indent ("Consistent policy of economic non-intervention has led to inflation and unemployment", 1),
                    "Australia is desperate for reform",
                    StringLineFormatter.Indent ("Unfortunately, this may not be appreciated by the entrenched supporters of the LP-CP", 1),
                ],
                "5 December 1972",
                "Ascension of Gough Whitlam",
                "The Whitlam Era",
                rolesLocs,
                "Speaker",
                (Localisation.UNUSED, Localisation.UNUSED),
                [],
                ("Party", "Parties"),
                partiesLocs,
                abbreviations,
                [],
                procedures,
                ballotsLocs,
                resultsLocs
            )
        );
    }
}
