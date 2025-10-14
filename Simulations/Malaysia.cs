using congress_cucuta.Converters;
using congress_cucuta.Data;
using System.Diagnostics.Metrics;

namespace congress_cucuta.Simulations;

internal class Malaysia : ISimulation {
    public Simulation Simulation { get; }

    public Malaysia () {
        IDType member = Role.MEMBER;
        IDType primeMinister = Role.HEAD_GOVERNMENT;
        IDType king = Role.HEAD_STATE;
        List<IDType> roles = [
            member,
            primeMinister,
            king,
            Role.LEADER_PARTY,
            0,
            1,
            2,
            3,
            4,
            5,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[member] = ("Member", "Members");
        rolesLocs[primeMinister] = ("Prime Minister", "Prime Ministers");
        rolesLocs[king] = ("King", "Kings");
        rolesLocs[0] = (Localisation.UNUSED, Localisation.UNUSED);
        rolesLocs[1] = (Localisation.UNUSED, Localisation.UNUSED);
        rolesLocs[2] = (Localisation.UNUSED, Localisation.UNUSED);
        rolesLocs[3] = ("Party Leader of Alliance", "Party Leaders of Alliance");
        rolesLocs[4] = ("Party Leader of DAP", "Party Leaders of DAP");
        rolesLocs[5] = ("Party Leader of PAS", "Party Leaders of PAS");
        rolesLocs[Role.LEADER_PARTY] = ("Party Leader", "Party Leaders");

        Faction westCoast = new (0);
        Faction eastCoast = new (1);
        Faction eastMalaysia = new (2);
        List<Faction> states = [westCoast, eastCoast, eastMalaysia];
        Dictionary<IDType, (string, string[])> statesLocs = [];
        statesLocs[westCoast.ID] = (
            "West Coast",
            [
                StringLineFormatter.Indent ("Very populous and wealthy states", 1),
                StringLineFormatter.Indent ("Significant Chinese and Indian population", 1),
            ]
        );
        statesLocs[eastCoast.ID] = (
            "East Coast",
            [
                StringLineFormatter.Indent ("Less powerful states", 1),
                StringLineFormatter.Indent ("Malay-dominated", 1),
                StringLineFormatter.Indent ("Rural, conservative, and generally Islamist", 1),
            ]
        );
        statesLocs[eastMalaysia.ID] = (
            "East Malaysia",
            [
                StringLineFormatter.Indent ("Relatively poor, but populous and more autonomous", 1),
                StringLineFormatter.Indent ("Very ethnically diverse with a large Christian minority", 1),
            ]
        );

        Faction alliance = new (3);
        Faction dap = new (4);
        Faction pas = new (5);
        List<Faction> parties = [alliance, dap, pas];
        Dictionary<IDType, (string, string[])> partiesLocs = [];
        partiesLocs[alliance.ID] = (
            "Alliance Party",
            [
                StringLineFormatter.Indent ("Malay-dominated party", 1),
                StringLineFormatter.Indent ("Extremely Malay nationalist with no clear agenda", 1),
                StringLineFormatter.Indent ("This stance has been slowly softening", 2),
            ]
        );
        partiesLocs[dap.ID] = (
            "Democratic Action Party",
            [
                StringLineFormatter.Indent ("Pan-Malaysian party with significant Chinese support", 1),
                StringLineFormatter.Indent ("Democratic socialist", 1),
            ]
        );
        partiesLocs[pas.ID] = (
            "Pan-Malayan Islamic Party",
            [
                StringLineFormatter.Indent ("Malay-dominated party", 1),
                StringLineFormatter.Indent ("Left Islamist, but increasingly Malay nationalist", 1),
            ]
        );
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[alliance.ID] = "Alliance";
        abbreviations[dap.ID] = "DAP";
        abbreviations[pas.ID] = "PAS";

        ProcedureImmediate royalElection = new (
            0,
            [
                new (Procedure.Effect.ActionType.ElectionNominated, [king]),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [king], 0),
            ]
        );
        ProcedureImmediate federation = new (
            1,
            [new (Procedure.Effect.ActionType.ElectionRegion, [])]
        );
        ProcedureImmediate generalElection = new (
            2,
            [
                new (Procedure.Effect.ActionType.ElectionParty, [king]),
                new (Procedure.Effect.ActionType.ElectionNominated, [primeMinister, king]),
            ]
        );
        ProcedureImmediate malaySupremacy = new (
            3,
            [new (Procedure.Effect.ActionType.PermissionsVotes, [alliance.ID], 2)]
        );
        List<ProcedureImmediate> proceduresGovernmental = [royalElection, federation, generalElection, malaySupremacy];
        ProcedureTargeted chongEuLim = new (
            4,
            [new (Procedure.Effect.ActionType.ProcedureActivate, [generalElection.ID])],
            [1]
        );
        ProcedureTargeted conferenceRulers = new (
            5,
            [new (Procedure.Effect.ActionType.ProcedureActivate, [royalElection.ID])],
            [1]
        );
        ProcedureTargeted thirteenMayIncident = new (
            6,
            [new (Procedure.Effect.ActionType.PermissionsCanVote, [alliance.ID], 0)],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [chongEuLim, conferenceRulers, thirteenMayIncident];
        ProcedureDeclared royalVeto = new (
            7,
            [
                new (Procedure.Effect.ActionType.BallotFail, [])
            ],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [king]
        );
        ProcedureDeclared stateEmergency = new (
            8,
            [
                new (Procedure.Effect.ActionType.ElectionAppointed, [primeMinister, king]),
                new (Procedure.Effect.ActionType.BallotLimit, [primeMinister, king]),
            ],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [king]
        );
        List<ProcedureDeclared> proceduresDeclared = [royalVeto, stateEmergency];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[royalElection.ID] = (
            "Royal Election",
            "This country has a tradition of electing its sovereigns from distinguished families rather than allowing rulership to be inherited."
        );
        procedures[federation.ID] = (
            "Federation",
            "This country's government is organised as a federation, divided into powerful administrative divisions with individual representation in the national government."
        );
        procedures[generalElection.ID] = (
            "General Election",
            "This country has a legislature with regular elections, along with an empowered head of government."
        );
        procedures[malaySupremacy.ID] = (
            "Malay Supremacy",
            "The Malaysian government officially follows Malay Supremacy, guaranteeing special rights for the Malays, mainly to the detriment of the Chinese. This congenital fear of a Chinese takeover is so entrenched that it will take generations to overcome."
        );
        procedures[chongEuLim.ID] = (
            "Dr. Chong Eu Lim",
            "Dr. Lim is the powerful leader of the Malaysian Chinese Association, a component of the Alliance. However, with the racial situation getting worse and worse every day, Dr. Lim might form his own party to contest the upcoming elections in a bid to better serve the needs of all Malaysians, not just those of the Malays."
        );
        procedures[conferenceRulers.ID] = (
            "Conference of Rulers",
            "Every five years, the Conference of Rulers elects a new king from the royal states of Malaysia. The current king's term is set to expire in 1970."
        );
        procedures[thirteenMayIncident.ID] = (
            "13 May Incident",
            Localisation.UNUSED
        );
        procedures[royalVeto.ID] = (
            "Royal Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );
        procedures[stateEmergency.ID] = (
            "State of Emergency",
            "The king can dissolve parliament and form an unelected government in times of major instability."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [],
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
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ReplaceProcedure,
                        [chongEuLim.ID, thirteenMayIncident.ID]
                    ),
                ],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 2)]
            )
        );
        Ballot incidentA = new (
            2,
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 3)]
            ),
            true
        );
        Ballot ballotC = new (
            3,
            new Ballot.Result (
                [],
                []
            ),
            new Ballot.Result (
                [],
                []
            )
        );
        List<Ballot> ballots = [ballotA, ballotB, incidentA, ballotC];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Adopt the National Language Bill",
            [
                StringLineFormatter.Indent ("National Language Bill:", 1),
                StringLineFormatter.Indent ("Malay remains the national language", 2),
                StringLineFormatter.Indent ("English retains limited official recognition", 2),
                StringLineFormatter.Indent ("Sarawak and Sabah can continue to use English for any purpose", 3),
                StringLineFormatter.Indent ("Chinese, Tamil, and other regional languages may be used in unofficial contexts", 2),
                StringLineFormatter.Indent ("This bill does very little", 1),
                StringLineFormatter.Indent ("English is already official, but is being phased out in a year", 2),
                StringLineFormatter.Indent ("This may be an opportunity to retain English as an official language", 3),
                StringLineFormatter.Indent ("This is a token concession to the Chinese", 2),
            ],
            [
                "Very little has changed",
                StringLineFormatter.Indent ("However, the passage of this bill at least proves that there is some will towards true equality", 1),
                StringLineFormatter.Indent ("Some Malays virulently opposed this bill, so hardline Malay nationalists are still very much in the mainstream", 2),
            ],
            [
                "Malays celebrate their continued dominance",
                StringLineFormatter.Indent ("Chinese and Indians expand their rhetoric questioning Malaysia's legitimacy", 1),
                StringLineFormatter.Indent ("They would have preferred to remain a British colony rather than gain independence under a Malay-dominated state", 2),
                StringLineFormatter.Indent ("English is phased out as intended, to the detriment of national unity", 1),
                StringLineFormatter.Indent ("Race riots become relatively commonplace", 2),
                StringLineFormatter.Indent ("Opposition is especially strong in Sarawak and Sabah, which were promised special rights in exchange for union", 2),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Sunset Article 153",
            [
                StringLineFormatter.Indent ("Article 153 is the legal basis of Malay Supremacy", 1),
                StringLineFormatter.Indent ("It empowers the government to establish quotas for Malays in the civil service, education, and business", 2),
                StringLineFormatter.Indent ("Sunsetting this will cause it to expire in fifteen years if nothing is done to extend it", 2),
                StringLineFormatter.Indent ("Opposition to this article by Chinese-majority Singapore led to its expulsion from the union", 1),
                StringLineFormatter.Indent ("Even discussing Article 153 has the potential to destroy Malaysia's very tenuous unity", 2),
                StringLineFormatter.Indent ("Although some Malays are offended by affirmative action, most want to protect their special rights", 2),
            ],
            [
                "Malaysia appears to be on the verge of collapse",
                StringLineFormatter.Indent ("Malay faith in the Malaysian government has completely eroded", 1),
                StringLineFormatter.Indent ("Malays start race riots, often with open support from the police", 1),
                StringLineFormatter.Indent ("Chinese and Indians start counter-riots", 2),
                StringLineFormatter.Indent ("Many Chinese ironically begin fleeing to China out of fear", 3),
            ],
            [
                "Chinese and Indians are extremely agitated",
                StringLineFormatter.Indent ("Kuala Lumpur descends into racial violence and chaos", 1),
                StringLineFormatter.Indent ("This leads to hundreds of deaths", 2),
                StringLineFormatter.Indent ("Situation eventually calms down, as nobody sees violence as significantly beneficial in the long-term", 2),
                "Malaysia appears to be stable for now",
                StringLineFormatter.Indent ("Chinese feel marginalised, but are not in any imminent danger after the end of rioting", 1),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Promulgate the National Principles",
            [
                StringLineFormatter.Indent ("After the disastrous riots in Kuala Lumpur, the government must reunite the country", 1),
                StringLineFormatter.Indent ("Their solution is a new pledge of intent, which notably does not mention race", 2),
                StringLineFormatter.Indent ("National Principles:", 1),
                StringLineFormatter.Indent ("Belief in God (Islam)", 2),
                StringLineFormatter.Indent ("Loyalty to King and country", 2),
                StringLineFormatter.Indent ("Supremacy of the constitution", 2),
                StringLineFormatter.Indent ("Rule of law", 2),
                StringLineFormatter.Indent ("Courtesy and morality", 2),
            ],
            [
                "Introduction of National Principles is successful",
                StringLineFormatter.Indent ("Students and government officials are required to recite the pledge, which helps to instil genuine belief in the principles", 1),
                StringLineFormatter.Indent ("This has little effect right now, but may lead to societal change in future generations", 1),
            ],
            [
                "Malaysia remains deeply divided",
                StringLineFormatter.Indent ("Without any official government acknowledgement, Chinese and Indians again question their place in Malaysia", 1),
                StringLineFormatter.Indent ("This last happened when Singapore was expelled from Malaysia", 2),
                StringLineFormatter.Indent ("This could lead to further conflict in the future", 1),
                StringLineFormatter.Indent ("Notably, opposition to the government is rising in Sarawak and Sabah", 2),
                StringLineFormatter.Indent ("Apparent ignorance by the federal government leads many to believe that they should not be involved in Malaya's problems", 3),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Adopt the New Economic Policy",
            [
                StringLineFormatter.Indent ("New Economic Policy (NEP) is a twenty-year economic programme that is intended to reduce inequality", 1),
                StringLineFormatter.Indent ("Goal is for Malays to own 30% of all economic assets, non-Malays to own 40%, and foreigners to own 30%", 2),
                StringLineFormatter.Indent ("Malays currently only own 3%", 3),
                StringLineFormatter.Indent ("This will be achieved by giving Malays preferential access to and ownership of business, especially in the public sector", 2),
                StringLineFormatter.Indent ("Ideally, Malays would gain assets without affecting assets owned by non-Malays", 3),
                StringLineFormatter.Indent ("This is also intended to lower regional inequality", 1),
                StringLineFormatter.Indent ("However, there are no concrete plans for implementation", 2),
            ],
            [
                "Concrete plans are being drafted",
                StringLineFormatter.Indent ("Results will probably not be seen for decades, if at all", 1),
                "Chinese are not particularly opposed",
                StringLineFormatter.Indent ("They don't directly lose any assets", 1),
                StringLineFormatter.Indent ("Some see opportunities for enrichment", 1),
                StringLineFormatter.Indent ("This would be by setting up companies that are largely owned by Malays, but managed by Chinese", 2),
                "Malays are generally happy",
                StringLineFormatter.Indent ("However, there is significant concern about concentration of wealth", 1),
            ],
            [
                "Most Malaysians are very confused",
                StringLineFormatter.Indent ("This was the perfect opportunity to support pan-Malaysian unity and equality", 1),
                StringLineFormatter.Indent ("This would even have benefitted non-Malays, although many recognise that this plan was hastily and poorly conceived", 2),
                StringLineFormatter.Indent ("Chinese are worried that they may now lose their assets to a more drastic redistribution programme", 2),
                "For the moment, the government seems to be avoiding any dramatic actions",
                StringLineFormatter.Indent ("After all, Malaysian society is extremely divided", 1),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 1),
                new (new BallotPassedCondition (ballotB.ID, false), 6),
            ]
        );
        Result ballotBPassed = new (
            1,
            [
                new (new BallotPassedCondition (ballotA.ID, true), 2),
                new (new BallotPassedCondition (ballotA.ID, false), 3),
            ]
        );
        Result ballotAPassed = new (
            2,
            []
        );
        Result ballotAFailed = new (
            3,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 4),
                new (new BallotPassedCondition (ballotC.ID, false), 5),
            ]
        );
        Result ballotCPassed = new (
            4,
            []
        );
        Result ballotCFailed = new (
            5,
            []
        );
        Result ballotBFailed = new (
            6,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 7),
                new (new BallotPassedCondition (incidentA.ID, false), 12),
            ]
        );
        Result incidentAPassed = new (
            7,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 8),
                new (new BallotPassedCondition (ballotC.ID, false), 9),
            ]
        );
        Result ballotCPassed2 = new (
            8,
            []
        );
        Result ballotCFailed2 = new (
            9,
            [
                new (new BallotPassedCondition (ballotA.ID, true), 10),
                new (new BallotPassedCondition (ballotA.ID, false), 11),
            ]
        );
        Result ballotAPassed2 = new (
            10,
            []
        );
        Result ballotAFailed2 = new (
            11,
            []
        );
        Result incidentAFailed = new (
            12,
            [
                new (new BallotPassedCondition (ballotA.ID, true), 13),
                new (new BallotPassedCondition (ballotA.ID, false), 14),
            ]
        );
        Result ballotCPassed3 = new (
            13,
            []
        );
        Result ballotCFailed3 = new (
            14,
            []
        );
        List<Result> results = [
            resultStart,
            ballotBPassed,
            ballotAPassed,
            ballotAFailed,
            ballotCPassed,
            ballotCFailed,
            ballotBFailed,
            incidentAPassed,
            ballotCPassed2,
            ballotCFailed2,
            ballotAPassed2,
            ballotAFailed2,
            incidentAFailed,
            ballotCPassed3,
            ballotCFailed3,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Malaysia is crippled by constant rioting",
                StringLineFormatter.Indent ("This destroys the economy, along with the Chinese who largely control it", 1),
                StringLineFormatter.Indent ("International opinion is largely for the protection of the non-Malays, but business concerns generally take priority", 1),
            ]
        );
        resultsLocs[ballotAPassed.ID] = (
            "Ballot A Passed",
            [
                "Government's refusal to backtrack on equality measures worsens riots",
                StringLineFormatter.Indent ("Eventually, traditional leaders force the government to step down", 1),
                "Malaysia is now truly a Malay state",
                StringLineFormatter.Indent ("For the foreseeable future, non-Malays will remain second-class citizens", 1),
                StringLineFormatter.Indent ("This situation has led to major brain drain", 1),
                StringLineFormatter.Indent ("This bodes very poorly for Malaysia's future", 2),
            ]
        );
        resultsLocs[ballotAFailed.ID] = (
            "Ballot A Failed",
            [
                "Government maintains some sympathy",
                StringLineFormatter.Indent ("Malay language remains officially dominant, which is one of the more important privileges", 1),
                StringLineFormatter.Indent ("Chinese and Indians will essentially be barred from public service if they cannot speak Malay", 1),
            ]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "Over time, Malays may come to accept their new place in Malaysia",
                StringLineFormatter.Indent ("Economic concessions aren't as important as governmental dominance, but are still welcome", 1),
                StringLineFormatter.Indent ("Circumvention of the NEP by some Chinese remains a sore spot, but Malays are starting to see some benefits", 1),
                "Race situation remains tense, but is gradually improving",
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "Malaysia is in a strange state of parallel governance",
                StringLineFormatter.Indent ("Malays dominate the public sector, while the Chinese dominate the private sector", 1),
                StringLineFormatter.Indent ("Officially, the Malays have no special privileges, but Chinese and Indians are overwhelmingly underrepresented in government\r\n", 1),
                "This situation is extremely tense and cannot last",
                StringLineFormatter.Indent ("Rebellion will probably be necessary to effect real change", 1),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Keeping Article 153 could lead to Malay support or apathy in any future concessions",
                StringLineFormatter.Indent ("Understandably, these non-binding guarantees are not convincing for Chinese and Indians", 1),
                StringLineFormatter.Indent ("Article 153 has become entrenched constitutional law", 1),
                StringLineFormatter.Indent ("There may never be another opportunity to challenge Article 153 ever again", 2),
            ]
        );
        resultsLocs[incidentAPassed.ID] = (
            "Incident A Passed",
            [
                "National Principles serve as the basis for civic nationalism",
                StringLineFormatter.Indent ("However, many argue that words alone are not enough", 1),
            ]
        );
        resultsLocs[ballotCPassed2.ID] = (
            "Ballot C Passed",
            [
                "NEP satisfies most Malay demands",
                StringLineFormatter.Indent ("Its results will depend on implementation, although corruption seems to be a major issue", 1),
                "Chinese and Indians reluctantly accept their place in society",
                StringLineFormatter.Indent ("It seems impossible to challenge Malay Supremacy now", 1),
                StringLineFormatter.Indent ("Perhaps future generations can take up the fight, once Malays feel secure in their economic status", 2),
                StringLineFormatter.Indent ("However, at least their rights are secured", 1),
            ]
        );
        resultsLocs[ballotCFailed2.ID] = (
            "Ballot C Failed",
            [
                "It seems a more drastic redistribution of economic assets may be forthcoming",
            ]
        );
        resultsLocs[ballotAPassed2.ID] = (
            "Ballot A Passed",
            [
                "There is apparently no will in the government for drastic measures",
                StringLineFormatter.Indent ("This is unpopular with Chinese and Indians, who want more than minor change", 1),
                StringLineFormatter.Indent ("This is also unpopular with Malays, who expected the government to do something meaningful", 1),
                "Current situation does not satisfy anyone in Malaysia",
                StringLineFormatter.Indent ("There will likely be major racial conflict for decades to come", 1),
            ]
        );
        resultsLocs[ballotAFailed2.ID] = (
            "Ballot A Failed",
            [
                "Government begins to openly discriminate against Chinese businesses",
                StringLineFormatter.Indent ("Goal of these policies is to push the Chinese out of the economy", 1),
                "Malaysia is now truly a Malay state",
                StringLineFormatter.Indent ("For the foreseeable future, non-Malays will remain second-class citizens", 1),
                StringLineFormatter.Indent ("This situation has led to major brain drain", 1),
                StringLineFormatter.Indent ("This bodes very poorly for Malaysia's future", 2),
            ]
        );
        resultsLocs[incidentAFailed.ID] = (
            "Incident A Failed",
            [
                "Refusal to adopt what amounts to a token pledge of allegiance is extremely concerning",
                StringLineFormatter.Indent ("Next steps taken will determine Malaysia's fate as a united state", 1),
            ]
        );
        resultsLocs[ballotCPassed3.ID] = (
            "Ballot C Passed",
            [
                "NEP satisfies most Malay demands",
                StringLineFormatter.Indent ("Its results will depend on implementation, although corruption seems to be a major issue", 1),
                "Chinese and Indians refuse to accept their place in society",
                StringLineFormatter.Indent ("It seems impossible to challenge Malay Supremacy now", 1),
                StringLineFormatter.Indent ("It doesn't seem like Malays will ever accept non-Malays", 2),
                StringLineFormatter.Indent ("This situation has led to major brain drain", 1),
                StringLineFormatter.Indent ("This bodes very poorly for Malaysia's future", 2),
            ]
        );
        resultsLocs[ballotCFailed3.ID] = (
            "Ballot C Failed",
            [
                "Rejection of the NEP is widely condemned",
                StringLineFormatter.Indent ("Sarawak and Sabah see it as proof of Malaya's colonial rule", 1),
                StringLineFormatter.Indent ("They declare independence as North Borneo", 2),
                StringLineFormatter.Indent ("Chinese begin to flee, fearing a more drastic redistribution and further racial riots", 1),
                StringLineFormatter.Indent ("Others join communist cells", 2),
                "Rump Malaysia is on the verge of collapse",
                StringLineFormatter.Indent ("Government's choice to favour Malays at any cost has completely destroyed its legitimacy", 1),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[ballotB.ID] = [stateEmergency.ID];
        ballotsProceduresDeclared[incidentA.ID] = [stateEmergency.ID];
        ballotsProceduresDeclared[ballotC.ID] = [stateEmergency.ID];
        History history = new (
            [ballotA.ID, incidentA.ID, ballotC.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            states,
            parties,
            [],
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Malaysia",
                "Elective Monarchy",
                [
                    "Malaysia gained independence under difficult circumstances",
                    StringLineFormatter.Indent ("Communist (often Chinese) insurgents lengthened British occupation and damaged societal unity", 1),
                    StringLineFormatter.Indent ("Union with Sarawak and Sabah came with armed Indonesian opposition", 1),
                    StringLineFormatter.Indent ("Singapore was expelled from the union", 1),
                    "Racial conflict permeates Malaysian politics",
                    StringLineFormatter.Indent ("Chinese control most of the economy", 1),
                    StringLineFormatter.Indent ("Malays have special rights as first-class citizens", 1),
                ],
                "11 August 1966",
                "End of Confrontation",
                "Malaysian Malaysia",
                rolesLocs,
                "Speaker",
                ("State", "States"),
                statesLocs,
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
