using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Simulations;

[ExcludeFromCodeCoverage]
internal class Finland : ISimulation {
    public Simulation Simulation { get; }

    public Finland () {
        IDType member = Role.MEMBER;
        IDType primeMinister = Role.HEAD_GOVERNMENT;
        IDType president = Role.HEAD_STATE;
        List<IDType> roles = [
            member,
            primeMinister,
            president,
            0,
            1,
            2,
            3,
            Role.LEADER_PARTY,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[member] = ("Member", "Members");
        rolesLocs[primeMinister] = ("Prime Minister", "Prime Ministers");
        rolesLocs[president] = ("President", "Presidents");
        rolesLocs[0] = ("Chairman of Ml", "Chairmen of Ml");
        rolesLocs[1] = ("Chairman of SDP", "Chairmen of SDP");
        rolesLocs[2] = ("Chairman of Kok", "Chairmen of Kok");
        rolesLocs[3] = ("Chairman of SFP", "Chairmen of SFP");
        rolesLocs[4] = ("Chairman of Kesk", "Chairmen of Kesk");
        rolesLocs[Role.LEADER_PARTY] = ("Chairman", "Chairmen");

        Faction ml = new (0);
        Faction sdp = new (1);
        Faction kok = new (2);
        Faction sfp = new (3, false);
        IDType ml2 = 4;
        List<Faction> factions = [ml, sdp, kok, sfp];
        Dictionary<IDType, (string, string[])> factionsLocs = [];
        factionsLocs[ml.ID] = (
            "Agrarian League",
            [
                StringLineFormatter.Indent ("Centre, populist party", 1),
                StringLineFormatter.Indent ("Supports small businesses and decentralisation", 1),
            ]
        );
        factionsLocs[sdp.ID] = (
            "Social Democratic Party of Finland",
            [
                StringLineFormatter.Indent ("Left, anti-Soviet party", 1),
                StringLineFormatter.Indent ("Nebulous ideology, but mostly supports local powers", 1),
            ]
        );
        factionsLocs[kok.ID] = (
            "National Coalition Party",
            [
                StringLineFormatter.Indent ("Centre-right, anti-Soviet party", 1),
                StringLineFormatter.Indent ("Classically liberal and interested in individual rights", 1),
            ]
        );
        factionsLocs[sfp.ID] = (
            "Swedish People's Party of Finland",
            []
        );
        factionsLocs[ml2] = ("Centre Party", []);
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[ml.ID] = "Ml";
        abbreviations[sdp.ID] = "SDP";
        abbreviations[kok.ID] = "Kok";
        abbreviations[sfp.ID] = "SFP";
        abbreviations[ml2] = "Kesk";

        ProcedureImmediate legislativeElection = new (
            0,
            [
                new (Procedure.Effect.EffectType.ElectionParty, []),
                new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister, president]),
            ]
        );
        ProcedureImmediate presidentialElection = new (
            1,
            [
                new (Procedure.Effect.EffectType.ElectionNominated, [president, primeMinister]),
                new (Procedure.Effect.EffectType.PermissionsCanVote, [president], 0),
            ]
        );
        List<ProcedureImmediate> proceduresGovernmental = [legislativeElection, presidentialElection];
        ProcedureTargeted paasikiviDoctrine = new (
            2,
            [new (Procedure.Effect.EffectType.ProcedureActivate, [legislativeElection.ID])],
            []
        );
        ProcedureTargeted kLine = new (
            3,
            [new (Procedure.Effect.EffectType.ProcedureActivate, [presidentialElection.ID])],
            [1, 3, 4]
        );
        ProcedureTargeted lingeringNightFrost = new (
            4,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            [0]
        );
        ProcedureTargeted academicKareliaSociety = new (
            5,
            [new (Procedure.Effect.EffectType.PermissionsVotes, [kok.ID], 1)],
            []
        );
        ProcedureTargeted activatedYyaTreaty = new (
            6,
            [new (Procedure.Effect.EffectType.VotePassAdd, [], 2)],
            [3],
            false
        );
        ProcedureTargeted noteCrisis = new (
            7,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            [4],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            paasikiviDoctrine,
            kLine,
            lingeringNightFrost,
            academicKareliaSociety,
            activatedYyaTreaty,
            noteCrisis,
        ];
        ProcedureDeclared voteNoConfidence = new (
            8,
            [new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister, president])],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            []
        );
        ProcedureDeclared veto = new (
            9,
            [new (Procedure.Effect.EffectType.BallotFail, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [president]
        );
        ProcedureDeclared dissolutionParliament = new (
            10,
            [
                new (Procedure.Effect.EffectType.ElectionParty, []),
                new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister, president]),
            ],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [president]
        );
        ProcedureDeclared ministerialAppointment = new (
            11,
            [
                new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister, president]),
            ],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [president]
        );
        List<ProcedureDeclared> proceduresDeclared = [voteNoConfidence, veto, dissolutionParliament, ministerialAppointment];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[legislativeElection.ID] = (
            "Legislative Election",
            "This country has a legislature with regular elections, along with an empowered head of government."
        );
        procedures[presidentialElection.ID] = (
            "Presidential Election",
            "This country has a powerful, elected executive who is separate from the legislature."
        );
        procedures[paasikiviDoctrine.ID] = (
            "Paasikivi Doctrine",
            "Finland's continued existence depends on the mercy of the USSR - a mercy which may not last long, and which previous governments have committed to preserving at almost any cost."
        );
        procedures[kLine.ID] = (
            "K-Line",
            "Supporters of the president manipulate politics through backdoor channels, avoiding official decrees and votes. Their rationale is simple: only Urho Kekkonen can maintain the confidence of the USSR, and he must stay in power."
        );
        procedures[lingeringNightFrost.ID] = (
            "Lingering Night Frost",
            "The current government lives in the shadow of a Soviet spectre: a \"night frost\" hangs in the air, and threatens to descend at any time."
        );
        procedures[academicKareliaSociety.ID] = (
            "Academic Karelia Society",
            "The Academic Karelia Society was an anti-Soviet organisation that demanded the conquest of Karelia. Though it no longer exists, many societal leaders remember the organisation with fondness and continue to sympathise with its precepts."
        );
        procedures[activatedYyaTreaty.ID] = (
            "Activated YYA Treaty",
            Localisation.UNUSED
        );
        procedures[noteCrisis.ID] = (
            "Note Crisis",
            Localisation.UNUSED
        );
        procedures[voteNoConfidence.ID] = (
            "Vote of No Confidence",
            "This country's head of government is responsible to the legislature, which means that he can be removed if he is seen to be inept."
        );
        procedures[veto.ID] = (
            "Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );
        procedures[dissolutionParliament.ID] = (
            "Dissolution of Parliament",
            "This country's legislature is responsible to the head of state, which means that it can be put up for election if it is seen to be inept."
        );
        procedures[ministerialAppointment.ID] = (
            "Ministerial Appointment",
            "This country's head of state is empowered to appoint government ministers, which drastically reduces legislative independence."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [lingeringNightFrost.ID, activatedYyaTreaty.ID])],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [lingeringNightFrost.ID, noteCrisis.ID])],
                [new (new AlwaysCondition (), 1)]
            )
        );
        Ballot ballotB = new (
            1,
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.FoundParty, [sfp.ID]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [ml.ID, ml2]),
                    new Ballot.Effect (Ballot.Effect.EffectType.RemoveProcedure, [academicKareliaSociety.ID]),
                ],
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
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [ml.ID, ml2])],
                [
                    new (new ProcedureActiveCondition (activatedYyaTreaty.ID, true), 3),
                    new (new ProcedureActiveCondition (activatedYyaTreaty.ID, false), 4),
                ]

            ),
            new Ballot.Result (
                [],
                [
                    new (new ProcedureActiveCondition (noteCrisis.ID, true), 3),
                    new (new ProcedureActiveCondition (noteCrisis.ID, false), 4),
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
        Ballot incidentB = new (
            4,
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
        List<Ballot> ballots = [ballotA, ballotB, ballotC, incidentA, incidentB];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Join the European Free Trade Association",
            [
                StringLineFormatter.Indent ("European Free Trade Association (EFTA) consists of the Nordics, Austria, Switzerland, and the UK", 1),
                StringLineFormatter.Indent ("It eliminates industrial tariffs between members, but does not constrain external trade", 2),
                StringLineFormatter.Indent ("Essentially, the EFTA exists on the periphery of Europe", 2),
                StringLineFormatter.Indent ("While this would normally be a good idea, recent events make it somewhat inadvisable", 1),
                StringLineFormatter.Indent ("Berlin Wall is currently being built, with American and Soviet forces on high alert", 2),
                StringLineFormatter.Indent ("Cuba just underwent a communist revolution", 2),
                StringLineFormatter.Indent ("USSR has just tested the world's most powerful nuclear weapon", 2),
                StringLineFormatter.Indent ("USSR could very well make a show of force to discourage a westward shift", 3),
                "We will settle for association with the EFTA if this ballot fails",
            ],
            [
                "USSR summons Finnish leadership for military consultations",
                StringLineFormatter.Indent ("They cite the threat of \"German\" imperialism, though this is obviously a cover for the West", 1),
                StringLineFormatter.Indent ("They propose to activate the military provisions of the YYA Treaty", 1),
                StringLineFormatter.Indent ("This would essentially place Finnish defence and foreign policy under Soviet control", 2),
                "Even after a tense meeting with the Soviets, they refuse to back down",
                StringLineFormatter.Indent ("With no American support forthcoming and not much of a military, Finland becomes a Soviet ally", 1),
                StringLineFormatter.Indent ("Mercifully, the Soviets allow significant internal autonomy", 2),
                StringLineFormatter.Indent ("US takes the matter to the UN, but little of substance happens", 1),
            ],
            [
                "USSR summons Finnish leadership for military consultations",
                StringLineFormatter.Indent ("They cite the threat of \"German\" imperialism, though this is obviously a cover for the West", 1),
                StringLineFormatter.Indent ("They propose to activate the military provisions of the YYA Treaty", 1),
                StringLineFormatter.Indent ("This would essentially place Finnish defence and foreign policy under Soviet control", 2),
                "After a tense meeting with the Soviets, they back down",
                StringLineFormatter.Indent ("USSR agrees to postpone consultations to a later date, as long as their interests aren't threatened", 1),
                StringLineFormatter.Indent ("Finland retains its neutrality for the time being", 1),
                StringLineFormatter.Indent ("Finland associates with EFTA as planned", 2),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Introduce Swedish in primary schools",
            [
                StringLineFormatter.Indent ("Less than 8% of the population speaks Swedish", 1),
                StringLineFormatter.Indent ("However, western and southern coasts have Swedish majorities", 2),
                StringLineFormatter.Indent ("Swedish was the state and elite language for most of Finland's history, so it retains significant prestige", 2),
                StringLineFormatter.Indent ("Despite Soviet dominance, Sweden remains one of Finland's most important allies and trading partners", 1),
                StringLineFormatter.Indent ("This makes it relatively important for business", 2),
                StringLineFormatter.Indent ("Introducing Swedish will be seen as a step towards the West", 1),
                StringLineFormatter.Indent ("Ironically, it will also be seen as a nationalist policy, given the importance of Swedish literature to Finnish culture", 2),
                StringLineFormatter.Indent ("Even the national anthem is actually a Swedish tune, translated to Finnish", 3),
            ],
            [
                "This policy is extemely unpopular",
                StringLineFormatter.Indent ("However, mandatory Swedish becomes the rallying cry of the Swedish minority", 1),
                StringLineFormatter.Indent ("This is the first step towards their end goal of full bilingualism", 2),
                StringLineFormatter.Indent ("Many question why students can't opt for a more useful language, like English", 1),
                "On the other hand, learning Swedish becomes a unique facet of Finnish culture",
                StringLineFormatter.Indent ("Anything is better than learning Russian", 1),
            ],
            [
                "Swedish minority is disgruntled",
                StringLineFormatter.Indent ("They complain about \"mandatory Finnish,\" although there is no widespread discontent", 1),
                "Business largely goes on as usual",
                StringLineFormatter.Indent ("Some speculate that refusing to introduce Swedish could actually lead to a resurgence in its popularity among bilingual Finnish-Swedish speakers", 1),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Negotiate a comprehensive income policy agreement",
            [
                StringLineFormatter.Indent ("Finnish economy is almost entirely unionised", 1),
                StringLineFormatter.Indent ("Non-union members benefit from labour agreements being universally valid", 2),
                StringLineFormatter.Indent ("This would take collective bargaining to a national level", 1),
                StringLineFormatter.Indent ("Trade unions, companies, and the government would negotiate on wages for all industries", 2),
                StringLineFormatter.Indent ("Goal is to control inflation, increase real income, and maintain industrial competitiveness", 2),
                StringLineFormatter.Indent ("Even if an agreement is reached, it's only applicable to those who sign", 2),
            ],
            [
                "This is a very popular policy",
                StringLineFormatter.Indent ("While there is some corporate grumbling, in practice everyone is satisfied", 1),
                StringLineFormatter.Indent ("Workers are guaranteed certain livable wages", 2),
                StringLineFormatter.Indent ("Employers avoid unforseen strikes or wage hikes", 2),
                StringLineFormatter.Indent ("This is seen as a victory for the socialists", 1),
                StringLineFormatter.Indent ("This may have effects on attitudes towards the USSR", 2),
            ],
            [
                "Many are quite disappointed",
                StringLineFormatter.Indent ("Given globalisation, prices could increase faster than wages, which would lead to worse living standards", 1),
                StringLineFormatter.Indent ("This is now practically a certainty", 2),
                StringLineFormatter.Indent ("However, the current system seems to be working just fine", 1),
                StringLineFormatter.Indent ("While large strikes remain a threat in the future, nothing much actually happens now", 2),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Join the Warsaw Pact",
            [
                StringLineFormatter.Indent ("Finland was exempted from joining the Warsaw Pact due to her neutrality", 1),
                StringLineFormatter.Indent ("With Finnish defence under Soviet control, this is no longer true", 2),
                StringLineFormatter.Indent ("USSR strongly \"suggests\" that Finland pledge to defend the socialist world from capitalist imperialists", 1),
                StringLineFormatter.Indent ("Economy continues to be heavily reliant on trade with both the West and the East, so this is impracticable", 2),
                StringLineFormatter.Indent ("This will have very severe consequences", 1),
            ],
            [
                "Finland is now a Soviet puppet state",
                StringLineFormatter.Indent ("Finland's defence and foreign policy were already under Soviet control", 1),
                StringLineFormatter.Indent ("In order to further coordinate policy, the Soviet ambassador is empowered request governmental actions", 2),
                StringLineFormatter.Indent ("This is enforced by Soviet forces, who are now stationed across Finland", 3),
            ],
            [
                "USSR is incredibly disappointed and stops the flow of industrial trade",
                StringLineFormatter.Indent ("Luckily, that's all that happens", 1),
                StringLineFormatter.Indent ("US loudly complains about Soviet actions, but doesn't do much", 1),
                StringLineFormatter.Indent ("After all, the conflict hasn't gone hot yet", 2),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Join the European Economic Community",
            [
                StringLineFormatter.Indent ("European Economic Community (EEC) is a EFTA competitor", 1),
                StringLineFormatter.Indent ("However, the two blocs cooperate often", 2),
                StringLineFormatter.Indent ("EEC is largely led by France and Germany", 2),
                StringLineFormatter.Indent ("Notably, the UK has decided to join the EEC", 2),
                StringLineFormatter.Indent ("Finnish-British trade has grown over the years", 3),
                StringLineFormatter.Indent ("USSR will be extremely displeased if we join the EEC", 1),
                StringLineFormatter.Indent ("This would be outright giving in to Western pressure", 2),
                "We will settle for a trade agreement with the EEC if this ballot fails",
            ],
            [
                "USSR completely cuts off all trade with Finland",
                StringLineFormatter.Indent ("This plunges Finland into an economic recession", 1),
                StringLineFormatter.Indent ("Finland is forced to refuse aid from the West, in order to avoid a Soviet occupation", 2),
                StringLineFormatter.Indent ("Western trade can't make up for the sheer quantity of Soviet trade", 2),
                StringLineFormatter.Indent ("If nothing changes, then the Finnish economy could completely collapse", 1),
            ],
            [
                "USSR is relieved",
                StringLineFormatter.Indent ("Trade with the West increases, but doesn't threaten Soviet dominance", 1),
                StringLineFormatter.Indent ("US accepts Finnish neutrality, pledging to support Finland within its constraints", 1),
                StringLineFormatter.Indent ("USSR hopes to maintain the current state of affairs", 1),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 1),
                new (new BallotPassedCondition (incidentA.ID, false), 2),
            ]
        );
        Result incidentAPassed = new (
            1,
            []
        );
        Result incidentAFailed = new (
            2,
            [
                new (new BallotPassedCondition (incidentB.ID, true), 3),
                new (new BallotPassedCondition (incidentB.ID, false), 6),
            ]
        );
        Result incidentBPassed = new (
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
        Result incidentBFailed = new (
            6,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 7),
                new (new BallotPassedCondition (ballotB.ID, false), 8),
            ]
        );
        Result ballotBPassed = new (
            7,
            []
        );
        Result ballotBFailed = new (
            8,
            []
        );
        List<Result> results = [
            resultStart,
            incidentAPassed,
            incidentAFailed,
            incidentBPassed,
            ballotCPassed,
            ballotCFailed,
            incidentBFailed,
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
                "Finland is dominated by the USSR",
                StringLineFormatter.Indent ("Soviet ambassador is effectively a policy-making authority", 1),
                StringLineFormatter.Indent ("Candidates for elections are screened by Soviet authorities", 1),
                StringLineFormatter.Indent ("Luckily, these candidates are not hand-picked, and are merely checked for \"fascist\" lackeys", 2),
                StringLineFormatter.Indent ("Any concerns about mandatory Swedish are quickly silenced by mandatory Russian", 1),
                StringLineFormatter.Indent ("This is probably the worst possible outcome", 1),
            ]
        );
        resultsLocs[incidentAFailed.ID] = (
            "Incident A Failed",
            [
                "USSR warily watches Finland turn to the West",
                StringLineFormatter.Indent ("Outcome will depend on preparedness and foreign ties", 1),
            ]
        );
        resultsLocs[incidentBPassed.ID] = (
            "Incident B Passed",
            [
                "Wariness turns to anger, potentially precipitating an invasion",
                StringLineFormatter.Indent ("However, USSR doesn't yet have a good reason to occupy Finland", 1),
                StringLineFormatter.Indent ("They hope that a total economic collapse will bring Finland back under Soviet suzerainty", 2),
            ]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "Income policy agreement mitigates some of the worst economic impacts",
                StringLineFormatter.Indent ("After all, it was intended to withstand changes in the global economy", 1),
                StringLineFormatter.Indent ("Anti-Soviet socialism and individualism both become popular and patriotic", 1),
                StringLineFormatter.Indent ("USSR's policy has apparently backfired", 2),
                "Over some years, Finland expands trade with the West",
                StringLineFormatter.Indent ("USSR finally backs down and its reputation is badly tarnished", 1),
                StringLineFormatter.Indent ("Economy hasn't quite recovered, but Finland remains independent", 1),
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "Economy indeed collapses",
                StringLineFormatter.Indent ("Wages and output drop precipitously low, which forces the government to appeal for Soviet assistance", 1),
                StringLineFormatter.Indent ("USSR ties trade liberalisation to political changes", 2),
                StringLineFormatter.Indent ("USSR now exercises direct control over government formation", 3),
                "Finland is now a Soviet puppet state",
                StringLineFormatter.Indent ("Only silver lining is that Soviet forces haven't yet occupied Finland", 1),
            ]
        );
        resultsLocs[incidentBFailed.ID] = (
            "Incident B Failed",
            [
                "USSR is satisfied with Finnish neutrality",
                StringLineFormatter.Indent ("Meanwhile, Finnish society continues to develop", 1),
            ]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Over time, Swedish becomes mandatory in all levels of education and the bureaucracy",
                StringLineFormatter.Indent ("This is considered a sign of Westernisation", 1),
                StringLineFormatter.Indent ("Of course, it is also patently annoying for almost all students", 2),
                StringLineFormatter.Indent ("Despite popular opposition, mandatory Swedish persists", 1),
                StringLineFormatter.Indent ("In a strange way, honouring Finland's Swedish past becomes also a celebration of Finnish culture and independence", 2),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Swedish never becomes mandatory",
                StringLineFormatter.Indent ("This results in a more insular outlook, with Finnish- and Swedish-speaking communities self-segregating", 1),
                StringLineFormatter.Indent ("This also contributes to isolationism with regards to the other Nordic states", 2),
                StringLineFormatter.Indent ("This effectively blocks Finland off from the West", 3),
                "Finland has rejected ties with both the East and the West",
                StringLineFormatter.Indent ("This is the neutral foreign policy Finnish leaders have always wanted", 1),
                StringLineFormatter.Indent ("Perhaps this will protect Finland in the dangerous years to come", 2),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[ballotA.ID] = [dissolutionParliament.ID];
        ballotsProceduresDeclared[ballotB.ID] = [ministerialAppointment.ID];
        ballotsProceduresDeclared[ballotC.ID] = [ministerialAppointment.ID];
        ballotsProceduresDeclared[incidentB.ID] = [dissolutionParliament.ID];
        History history = new (
            [ballotB.ID, ballotC.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            [],
            factions,
            [],
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Finland",
                "Mixed Republic",
                [
                    "Finland exited WWII with extremely strained relations with the USSR",
                    StringLineFormatter.Indent ("USSR demanded war reparations, a naval base, military transit, and some foreign policy alignment", 1),
                    StringLineFormatter.Indent ("This became known as the YYA Treaty", 2),
                    StringLineFormatter.Indent ("Balancing Finland's independence with trade and military considerations was paramount", 2),
                    StringLineFormatter.Indent ("Soviets regularly intervene to replace unsatisfactory government ministers with the threat of sanctions", 3),
                    "Finland was also facing its own past",
                    StringLineFormatter.Indent ("Nationalism was a major factor contributing to war", 1),
                    StringLineFormatter.Indent ("History of repression and autocracy", 1),
                ],
                "3 February 1959",
                "End of the Night Frost crisis",
                "Finlandisation",
                rolesLocs,
                "Speaker",
                (Localisation.UNUSED, Localisation.UNUSED),
                [],
                ("Party", "Parties"),
                factionsLocs,
                abbreviations,
                [],
                procedures,
                ballotsLocs,
                resultsLocs
            )
        );
    }
}
