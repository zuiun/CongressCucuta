using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Simulations;

[ExcludeFromCodeCoverage]
internal class China : ISimulation {
    public Simulation Simulation { get; }

    public China () {
        IDType member = Role.MEMBER;
        IDType directorGeneral = Role.HEAD_STATE;
        IDType chairman = Role.HEAD_GOVERNMENT;
        IDType general = Role.RESERVED_1;
        List<IDType> roles = [
            member,
            directorGeneral,
            chairman,
            general,
            0,
            1,
            2,
            Role.LEADER_PARTY,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[member] = ("Member", "Members");
        rolesLocs[directorGeneral] = ("Director-General", "Directors-General");
        rolesLocs[chairman] = ("Chairman", "Chairmen");
        rolesLocs[general] = ("General", "Generals");
        rolesLocs[0] = ("Ideologue of KMC", "Ideologues of KMC");
        rolesLocs[1] = ("Ideologue of KTH", "Ideologues of KTH");
        rolesLocs[2] = ("Ideologue of HHP", "Ideologues of HHP");
        rolesLocs[3] = ("Ideologue of CHH", "Ideologues of CHH");
        rolesLocs[4] = ("Ideologue of KHP", "Ideologues of KHP");
        rolesLocs[Role.LEADER_PARTY] = ("Ideologue", "Ideologues");

        Faction kmc = new (0);
        Faction kth = new (1);
        Faction khp = new (2);
        IDType kth2 = 3;
        IDType khp2 = 4;
        List<Faction> parties = [kmc, kth, khp];
        Dictionary<IDType, (string, string[])> factionsLocs = [];
        factionsLocs[kmc.ID] = (
            "National Revolutionary Army",
            [
                StringLineFormatter.Indent ("Armed wing of the party", 1),
                StringLineFormatter.Indent ("Nationalist with Christian influences", 1),
            ]
        );
        factionsLocs[kth.ID] = (
            "Reorganisation Comrades Association",
            [
                StringLineFormatter.Indent ("Socialist with pro-Soviet leanings", 1),
                StringLineFormatter.Indent ("Believes that Japan will always dominate China", 1),
            ]
        );
        factionsLocs[khp.ID] = (
            "Western Hills Conference",
            [
                StringLineFormatter.Indent ("Largest civilian faction", 1),
                StringLineFormatter.Indent ("Nationalist, conservative, and anti-communist", 1),
            ]
        );
        factionsLocs[kth2] = ("Political Study Association", []);
        factionsLocs[khp2] = ("Innovation Club", []);
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[kmc.ID] = "KMC";
        abbreviations[kth.ID] = "KTH";
        abbreviations[khp.ID] = "HHP";
        abbreviations[kth2] = "CHH";
        abbreviations[khp2] = "KHP";

        Dictionary<IDType, sbyte> currenciesValues = [];
        IDType unity = Currency.STATE;
        currenciesValues[unity] = 1;
        Dictionary<IDType, string> currencies = [];
        currencies[unity] = "Unity";

        ProcedureImmediate eternalPremier = new (
            0,
            [
                new (Procedure.Effect.EffectType.ElectionParty, []),
            ]
        );
        ProcedureImmediate partyState = new (
            1,
            [
                new (Procedure.Effect.EffectType.ElectionNominated, [chairman]),
                new (Procedure.Effect.EffectType.PermissionsVotes, [chairman], 2),
            ]
        );
        ProcedureImmediate warlordCliques = new (
            2,
            [
                new (Procedure.Effect.EffectType.ElectionAppointed, [general], 1),
            ]
        );
        List<ProcedureImmediate> proceduresGovernmental = [eternalPremier, partyState, warlordCliques];
        ProcedureTargeted threePrinciplesPeople = new (
            3,
            [new (Procedure.Effect.EffectType.CurrencyInitialise, [])],
            []
        );
        ProcedureTargeted dreamsUnitedFront = new (
            4,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            [3]
        );
        ProcedureTargeted ruralReconstructionMovement = new (
            5,
            [new (Procedure.Effect.EffectType.CurrencyAdd, [], 1)],
            []
        );
        ProcedureTargeted constitutionalProtectionMovement = new (
            6,
            [new (Procedure.Effect.EffectType.CurrencySubtract, [], 1)],
            []
        );
        ProcedureTargeted landTiller = new (
            7,
            [new (Procedure.Effect.EffectType.CurrencyAdd, [], 2)],
            [],
            false
        );
        ProcedureTargeted encirclementCampaigns = new (
            8,
            [new (Procedure.Effect.EffectType.CurrencySubtract, [], 2)],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            threePrinciplesPeople,
            dreamsUnitedFront,
            ruralReconstructionMovement,
            constitutionalProtectionMovement,
            landTiller,
            encirclementCampaigns,
        ];
        ProcedureDeclared impeachment = new (
            9,
            [
                new (Procedure.Effect.EffectType.ElectionNominated, [chairman]),
                new (Procedure.Effect.EffectType.CurrencySubtract, [], 1),
            ],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            [Role.LEADER_PARTY]
        );
        ProcedureDeclared extraordinaryNationalCongress = new (
            10,
            [
                new (Procedure.Effect.EffectType.ElectionNominated, [directorGeneral]),
                new (Procedure.Effect.EffectType.CurrencySubtract, [], 2),
            ],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [general]
        );
        ProcedureDeclared ultimateAuthority = new (
            11,
            [new (Procedure.Effect.EffectType.BallotPass, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [directorGeneral]
        );
        ProcedureDeclared veto = new (
            12,
            [new (Procedure.Effect.EffectType.BallotFail, [])],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [chairman, directorGeneral]
        );
        List<ProcedureDeclared> proceduresDeclared = [impeachment, extraordinaryNationalCongress, ultimateAuthority, veto];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[eternalPremier.ID] = (
            "Eternal Premier",
            "Yat-sen Sun left behind a ruling party with no recognised leader. Though without any achievements or aspirations to their name, various factions fight to be recognised as the legitimate inheritor of the doctor's legacy."
        );
        procedures[partyState.ID] = (
            "Party-State",
            "This country's government is synonymous with its ruling party, which governs as a dictatorship."
        );
        procedures[warlordCliques.ID] = (
            "Warlord Cliques",
            "The weakness of the central government has allowed local powerholders to create independent armies and polities."
        );
        procedures[threePrinciplesPeople.ID] = (
            "Three Principles of the People",
            "The Three Principles of the People were designed to bring China forward into modernity based on three key pillars: nationalism, democracy, and socialism. The government's legitimacy derives from its promise to fulfil these ideals."
        );
        procedures[dreamsUnitedFront.ID] = (
            "Dreams of a United Front",
            "Despite the anti-communist purge and the subsequent rebellion, leftists remain quite popular and continue to seek reconciliation with the communists."
        );
        procedures[ruralReconstructionMovement.ID] = (
            "Rural Reconstruction Movement",
            "The Rural Reconstruction Movement is a poorly funded, but dedicated group of intelligentsia who work to improve village life across China. Thus far, their activities have been limited to establishing schools and healthcare education."
        );
        procedures[constitutionalProtectionMovement.ID] = (
            "Constitutional Protection Movement",
            "Previous attempts to reform China have failed, but they are well within living memory. As long as the government continues to claim a revolutionary legacy, the people will not forget that they were once promised freedom and equality."
        );
        procedures[landTiller.ID] = (
            "Land to the Tiller",
            Localisation.UNUSED
        );
        procedures[encirclementCampaigns.ID] = (
            "Encirclement Campaigns",
            Localisation.UNUSED
        );
        procedures[impeachment.ID] = (
            "Impeachment",
            "This country's head of state is responsible to the ruling party, which means that he can be removed if he is seen to be inept."
        );
        procedures[extraordinaryNationalCongress.ID] = (
            "Extraordinary National Congress",
            "During national crises, the Central Executive Committee may be too slow to act. In such times, collective leadership should be abandoned in the interests of national unity and decisive action."
        );
        procedures[ultimateAuthority.ID] = (
            "Ultimate Authority",
            "This country's head of state is empowered to promulgate legislation without legislative oversight."
        );
        procedures[veto.ID] = (
            "Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [unity], 1)],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 2)]
            )
        );
        Ballot incidentA = new (
            1,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [ruralReconstructionMovement.ID, landTiller.ID])],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [unity], -1)],
                [new (new AlwaysCondition (), 2)]
            ),
            true
        );
        Ballot ballotB = new (
            2,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [constitutionalProtectionMovement.ID, encirclementCampaigns.ID])],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [unity], -1)],
                [new (new AlwaysCondition (), 3)]
            )
        );
        Ballot incidentB = new (
            3,
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [kth.ID, kth2]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [khp.ID, khp2]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [unity], -1),
                ],
                [new (new AlwaysCondition (), 4)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [kth.ID, kth2]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceParty, [khp.ID, khp2]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [unity], 1),
                ],
                [new (new AlwaysCondition (), 4)]
            ),
            true
        );
        Ballot ballotC = new (
            4,
            new Ballot.Result (
                [],
                []
            ),
            new Ballot.Result (
                [],
                []
            )
        );
        List<Ballot> ballots = [ballotA, incidentA, ballotB, incidentB, ballotC];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Expand the postal system",
            [
                StringLineFormatter.Indent ("China is a massive state with poor rural infrastructure and massive wealth disparity", 1),
                StringLineFormatter.Indent ("Remittances are the main equaliser of wealth", 2),
                StringLineFormatter.Indent ("This would empower Chunghwa Post beyond its official scope", 1),
                StringLineFormatter.Indent ("Air lines and other transportation infrastructure would be established across the state", 2),
                StringLineFormatter.Indent ("Unobstructed access to every province would weaken warlords", 3),
                StringLineFormatter.Indent ("Chunghwa Post could offer insurance policies and savings accounts, funded by the remittance industry", 2),
            ],
            [
                "This greatly improves the rural economy",
                StringLineFormatter.Indent ("Significantly greater shipping also leads to the abolition of domestic customs taxes", 1),
                StringLineFormatter.Indent ("This is only the basis of future reform", 1),
                StringLineFormatter.Indent ("There is still very little wealth being generated in rural areas", 2),
            ],
            [
                "Provincial governors are now responsible for interprovincial communications",
                StringLineFormatter.Indent ("This is wildly ineffective", 1),
                StringLineFormatter.Indent ("Most warlords are corrupt", 2),
                StringLineFormatter.Indent ("Many provinces have limited sources of revenue", 2),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Establish a land bank",
            [
                StringLineFormatter.Indent ("China currently already has an agricultural bank, the Farmers Bank of China", 1),
                StringLineFormatter.Indent ("Farmers Bank only issues loans for purchasing crops and equipment", 2),
                StringLineFormatter.Indent ("Land bank would issue loans for the purchase and development of land", 1),
                StringLineFormatter.Indent ("This would be a prerequisite for land reform", 2),
                StringLineFormatter.Indent ("Most land is owned by landlords, not farmers", 2),
                StringLineFormatter.Indent ("There are some concerns about this bank's solvency and potential for corruption", 2),
            ],
            [
                "Effects will not be known for some time",
                StringLineFormatter.Indent ("However, it improves the popularity of the government in rural areas", 1),
                StringLineFormatter.Indent ("Funding issues remain", 1),
                StringLineFormatter.Indent ("Heavier taxation will likely be necessary to fund the bank", 2),
            ],
            [
                "This is disappointing to many",
                StringLineFormatter.Indent ("Lack of funding is seen as an excuse", 1),
                StringLineFormatter.Indent ("Funding could certainly be discovered somewhere by tightening down on corruption", 2),
                StringLineFormatter.Indent ("Urban businesses and industrialists are not particularly heavily taxed", 2),
                StringLineFormatter.Indent ("Many farmers might turn to the communists", 1),
                StringLineFormatter.Indent ("Rural Reconstruction Movement continues to operate", 1),
                StringLineFormatter.Indent ("However, they continue to be hampered by lack of governmental support", 2),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Begin Political Tutelage",
            [
                StringLineFormatter.Indent ("China is technically unified", 1),
                StringLineFormatter.Indent ("Theoretically, the next stage of the revolution should be a period of rapid reform", 2),
                StringLineFormatter.Indent ("This would limit political freedoms until the people are ready for democracy", 3),
                StringLineFormatter.Indent ("Though this would result in a basic law, it would not change much", 1),
                StringLineFormatter.Indent ("However, this would clarify what reforms the government plans to implement to prepare the people for democracy", 2),
                StringLineFormatter.Indent ("This would also establish a civilian government in advance", 2),
            ],
            [
                "Political shift begins to take place",
                StringLineFormatter.Indent ("There is a flowering of independent thought in liberal and technocratic circles", 1),
                StringLineFormatter.Indent ("Communists are still being militarily suppressed", 1),
            ],
            [
                "Pseudo-military rule continues",
                StringLineFormatter.Indent ("This is wildly unpopular with the civilian factions of the government", 1),
                StringLineFormatter.Indent ("Their backers, mainly intellectuals with ties to the US, are also extremely displeased", 2),
                StringLineFormatter.Indent ("People are losing faith in the revolution", 1),
                StringLineFormatter.Indent ("Steady growth of communism means this could have severe consequences in the future", 2),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Withdraw from Manchuria",
            [
                StringLineFormatter.Indent ("Japan has just invaded Manchuria", 1),
                StringLineFormatter.Indent ("League of Nations has officially sided with China", 2),
                StringLineFormatter.Indent ("China is thus likely to receive international military and financial support", 3),
                StringLineFormatter.Indent ("Japan plans to establish a Manchu puppet state, legitimised through the Ching dynasty", 2),
                StringLineFormatter.Indent ("Most Chinese want to fight back", 1),
                StringLineFormatter.Indent ("Hsinhai Revolution was mainly spurred on by nationalism", 2),
                StringLineFormatter.Indent ("China is extremely weak right now and would not win", 2),
                StringLineFormatter.Indent ("Given some more years to prepare, that could change", 3),
            ],
            [
                "This is wildly unpopular",
                StringLineFormatter.Indent ("Communists now have even more material to use for their propaganda", 1),
                "Unofficial border is now directly north of Peiping",
                StringLineFormatter.Indent ("While defended by the Great Wall, this is still a very dangerous frontline", 1),
            ],
            [
                "Fight for Manchuria is disastrous",
                StringLineFormatter.Indent ("Local militias support the army, but everyone (army included) is poorly equipped", 1),
                StringLineFormatter.Indent ("Severe reprisals take place against resistant villages", 2),
                StringLineFormatter.Indent ("Some are now calling for another United Front", 1),
                StringLineFormatter.Indent ("They claim that Japan is the greatest enemy and that the communists can be dealt with later", 2),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Inaugurate the New Life Movement",
            [
                StringLineFormatter.Indent ("Revolution was intended to bring the Chinese people into modernity", 1),
                StringLineFormatter.Indent ("This means eliminating anachronistic societal norms", 2),
                StringLineFormatter.Indent ("Laziness, gambling, addiction, and corruption, among others", 3),
                StringLineFormatter.Indent ("This also includes foreign ideologies, like communism", 3),
                StringLineFormatter.Indent ("This will be a social engineering experiment", 1),
                StringLineFormatter.Indent ("Four virtues (decorum, righteousness, honesty, shame)", 2),
                StringLineFormatter.Indent ("Enforcement will begin with the upper classes and hopefully the lower classes will emulate these virtues", 3),
                StringLineFormatter.Indent ("Many claim this is simply puritanical grandstanding without concrete economic achievements to support it", 2),
            ],
            [
                "Intentionally left blank",
            ],
            [
                "Intentionally left blank",
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 1),
                new (new BallotPassedCondition (ballotC.ID, false), 5),
            ]
        );
        Result ballotCPassed = new (
            1,
            [
                new (new CurrencyValueCondition (unity, ComparisonType.FewerThanOrEqual, -2), 2),
                new (new AndCondition (
                    new CurrencyValueCondition (unity, ComparisonType.GreaterThan, -2),
                    new CurrencyValueCondition (unity, ComparisonType.FewerThan, 2)
                ), 3),
                new (new CurrencyValueCondition (unity, ComparisonType.GreaterThanOrEqual, 2), 4),
            ]
        );
        Result negTwoFewerUnity = new (
            2,
            []
        );
        Result negTwoTwoUnity = new (
            3,
            []
        );
        Result twoGreaterUnity = new (
            4,
            []
        );
        Result ballotCFailed = new (
            5,
            [
                new (new BallotsPassedCountCondition (ComparisonType.FewerThanOrEqual, 1), 6),
                new (new BallotsPassedCountCondition (ComparisonType.GreaterThanOrEqual, 2), 7),
            ]
        );
        Result oneFewerUnity = new (
            6,
            []
        );
        Result twoGreaterUnity2 = new (
            7,
            []
        );
        List<Result> results = [
            resultStart,
            ballotCPassed,
            negTwoFewerUnity,
            negTwoTwoUnity,
            twoGreaterUnity,
            ballotCFailed,
            oneFewerUnity,
            twoGreaterUnity2,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            [
                "Japan invades China again",
                StringLineFormatter.Indent ("Massive numbers of peasants are conscripted to fight", 1),
                StringLineFormatter.Indent ("Communists fight the Japanese and the government", 1),
                StringLineFormatter.Indent ("China's internal stability will be extremely important if she wishes to survive", 1),
            ]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "New Life Movement is underway when the war begins in earnest",
                StringLineFormatter.Indent ("This may lead to a new civic identity for China", 1),
                StringLineFormatter.Indent ("However, the war needs to be won first", 2),
            ]
        );
        resultsLocs[negTwoFewerUnity.ID] = (
            "Negative Two or Fewer Unity",
            [
                "Peasants are unwilling to fight",
                StringLineFormatter.Indent ("They throw their weight behind communist and pro-Japanese rival governments", 1),
                StringLineFormatter.Indent ("Communists are especially popular as the true heirs to the revolution", 1),
                "War goes poorly, and may not be won",
                StringLineFormatter.Indent ("Government retreats to interior bases", 1),
                StringLineFormatter.Indent ("Control in remote areas has completely devolved to warlords", 2),
                StringLineFormatter.Indent ("Communists take up the fight against Japan", 1),
                StringLineFormatter.Indent ("However, they don't have a proper army or equipment", 2),
            ]
        );
        resultsLocs[negTwoTwoUnity.ID] = (
            "Greater than Negative Two and Fewer than Two Unity",
            [
                "Peasants are somewhat willing to fight",
                StringLineFormatter.Indent ("They believe the government to be corrupt and hypocritical, but hate the Japanese more", 1),
                StringLineFormatter.Indent ("Communists gain support by fighting both of the people's apparent oppressors", 1),
                "War goes poorly, but will probably be won",
                StringLineFormatter.Indent ("Government has put up stiff resistance", 1),
                StringLineFormatter.Indent ("However, the communists are increasingly seen as the true protectors of the people", 2),
                StringLineFormatter.Indent ("This could have major consequences once the war is over and any pretence of national unity is dropped", 3),
            ]
        );
        resultsLocs[twoGreaterUnity.ID] = (
            "Two or Greater Unity",
            [
                "Peasants are willing to fight",
                StringLineFormatter.Indent ("They believe that the government has their best interests in mind, even if they disagree with policy", 1),
                StringLineFormatter.Indent ("Communists find vanishingly little support", 1),
                "War goes poorly, but will probably be won",
                StringLineFormatter.Indent ("Government has generally united China", 1),
                StringLineFormatter.Indent ("Communists are confined mostly to northern rural areas", 2),
                StringLineFormatter.Indent ("China may yet complete its revolution", 1),
                StringLineFormatter.Indent ("At the heart of this revolution    will be a new civic nationalism and Chinese identity", 2),
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "Without any shift in civic identity, the revolution as originally defined will never be truly completed",
                StringLineFormatter.Indent ("However, without any strict doctrinal expectations, the peasants can stay aloof from national politics", 1),
                StringLineFormatter.Indent ("In any case, the war needs to be won first", 2),
            ]
        );
        resultsLocs[oneFewerUnity.ID] = (
            "One or Fewer Unity",
            [
                "Peasants are unwilling to fight",
                StringLineFormatter.Indent ("They throw their weight behind communist and pro-Japanese rival governments", 1),
                StringLineFormatter.Indent ("Communists are especially popular as the true heirs to the revolution", 1),
                "War goes poorly, and may not be won",
                StringLineFormatter.Indent ("Communists take over the government and take up the fight against Japan", 1),
                StringLineFormatter.Indent ("However, they don't have a proper army or equipment", 2),
            ]
        );
        resultsLocs[twoGreaterUnity2.ID] = (
            "Two or Greater Unity",
            [
                "Peasants are willing to fight",
                StringLineFormatter.Indent ("They don't care which flag they fight under", 1),
                StringLineFormatter.Indent ("Communists are especially popular as the true protectors of the people", 1),
                "War goes poorly, but will probably be won",
                StringLineFormatter.Indent ("Government and the communists are increasingly united in purpose", 1),
                StringLineFormatter.Indent ("Original revolution appears to have been hijacked, just as the USSR had always intended", 2),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[ballotB.ID] = [impeachment.ID];
        History history = new (
            [ballotA.ID, ballotB.ID, incidentB.ID, ballotC.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            [],
            parties,
            currenciesValues,
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "China",
                "Oligarchic Dictatorship",
                [
                    "China has been in political turmoil ever since the Hsinhai Revolution",
                    StringLineFormatter.Indent ("The failure of the Nationalist Party (KMT) to maintain power led to a Warlord Era", 1),
                    StringLineFormatter.Indent ("Japanese- and Soviet-backed warlords battled for supremacy across China", 1),
                    "KMT only recently created its own military",
                    StringLineFormatter.Indent ("Unification of China has only just been completed", 1),
                    StringLineFormatter.Indent ("It now must finish its revolution against severe opposition from warlords and communists", 1),
                ],
                "29 December 1928",
                "Replacement of the \"Five Races Under One Union\" Flag",
                "The Nanking Decade",
                rolesLocs,
                "Secretary-General",
                (Localisation.UNUSED, Localisation.UNUSED),
                [],
                ("Faction", "Factions"),
                factionsLocs,
                abbreviations,
                currencies,
                procedures,
                ballotsLocs,
                resultsLocs
            )
        );
    }
}
