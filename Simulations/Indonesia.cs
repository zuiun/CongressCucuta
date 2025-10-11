using congress_cucuta.Converters;
using congress_cucuta.Data;

namespace congress_cucuta.Simulations;

internal class Indonesia : ISimulation {
    public Simulation Simulation { get; }

    public Indonesia () {
        IDType member = Role.MEMBER;
        IDType president = Role.HEAD_STATE;
        IDType chiefStaff = 0;
        IDType generalSecretary = 1;
        IDType chairman = 2;
        List<IDType> roles = [
            member,
            president,
            chiefStaff,
            generalSecretary,
            chairman,
            Role.LEADER_PARTY,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[member] = ("Member", "Members");
        rolesLocs[president] = ("President", "Presidents");
        rolesLocs[chiefStaff] = ("Chief of Staff", "Chiefs of Staff");
        rolesLocs[generalSecretary] = ("General Secretary", "General Secretaries");
        rolesLocs[chairman] = ("Chairman", "Chairmen");
        rolesLocs[Role.LEADER_PARTY] = ("Faction Leader", "Faction Leaders");

        Faction abri = new (0);
        Faction pki = new (1);
        Faction masyumi = new (2);
        List<Faction> factions = [abri, pki, masyumi];
        Dictionary<IDType, (string, string[], string)> factionsLocs = [];
        factionsLocs[abri.ID] = (
            "Republic of Indonesia Armed Forces",
            [
                StringLineFormatter.Indent ("Anti-imperialist, anti-communist, and nationalist", 1),
                StringLineFormatter.Indent ("Seen as a bastion of stability and independence", 1),
            ],
            "Chief of Staff"
        );
        factionsLocs[pki.ID] = (
            "Communist Party of Indonesia",
            [
                StringLineFormatter.Indent ("Anti-imperialist, secularist, and nationalist", 1),
                StringLineFormatter.Indent ("Seeks to implement socialism on a national scale", 1),
            ],
            "General Secretary"
        );
        factionsLocs[masyumi.ID] = (
            "Council of Indonesian Muslim Associations",
            [
                StringLineFormatter.Indent ("Relatively rightist and supports Sharia law", 1),
            ],
            "Chairman"
        );
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[abri.ID] = "ABRI";
        abbreviations[pki.ID] = "PKI";
        abbreviations[masyumi.ID] = "Masyumi";

        Dictionary<IDType, sbyte> currenciesValues = [];
        IDType influenceAbri = 0;
        IDType influencePki = 1;
        IDType influenceMasyumi = 2;
        currenciesValues[influenceAbri] = 1;
        currenciesValues[influencePki] = 1;
        currenciesValues[influenceMasyumi] = 1;
        Dictionary<IDType, string> currencies = [];
        currencies[influenceAbri] = "Influence";
        currencies[influencePki] = "Influence";
        currencies[influenceMasyumi] = "Influence";
        currencies[Currency.PARTY] = "Influence";

        ProcedureImmediate constitution1945 = new (
            0,
            [
                new (Procedure.Effect.ActionType.ElectionNominated, [president]),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [president], 0),
            ]
        );
        ProcedureImmediate nasakom = new (
            1,
            [new (Procedure.Effect.ActionType.ElectionParty, [president])]
        );
        List<ProcedureImmediate> proceduresGovernmental = [constitution1945, nasakom];
        ProcedureTargeted pancasila = new (
            2,
            [new (Procedure.Effect.ActionType.CurrencyInitialise, [])],
            []
        );
        ProcedureTargeted houseIslam = new (
            3,
            [new (Procedure.Effect.ActionType.CurrencySubtract, [influenceMasyumi], 1)],
            []
        );
        ProcedureTargeted eastSuez = new (
            4,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [influenceAbri, influencePki], 1)],
            []
        );
        List<ProcedureTargeted> proceduresSpecial = [pancasila, houseIslam, eastSuez];
        //ProcedureDeclared industryTakeover = new (
        //    4,
        //    [],
        //    new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
        //    0,
        //    []
        //);
        ProcedureDeclared rubberStamp = new (
            5,
            [
                new (Procedure.Effect.ActionType.BallotPass, [])
            ],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [president]
        );
        ProcedureDeclared veto = new (
            6,
            [
                new (Procedure.Effect.ActionType.BallotFail, [])
            ],
            new (Procedure.Confirmation.CostType.Always),
            0,
            [president]
        );
        ProcedureDeclared coup = new (
            7,
            [
                new (Procedure.Effect.ActionType.ElectionAppointed, [president]),
                new (Procedure.Effect.ActionType.CurrencyAdd, [influenceAbri], 1),
            ],
            new (Procedure.Confirmation.CostType.SingleDiceCurrency),
            0,
            [pki.ID, masyumi.ID]
        );
        ProcedureDeclared purge = new (
            8,
            [new (Procedure.Effect.ActionType.ElectionAppointed, [president])],
            new (Procedure.Confirmation.CostType.CurrencyValue, 4),
            0,
            [abri.ID]
        );
        List<ProcedureDeclared> proceduresDeclared = [/*industryTakeover, */rubberStamp, veto, coup, purge];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[constitution1945.ID] = (
            "Constitution of 1945",
            "The Constitution of 1945 was a provisional constitution that granted many prerogatives to the president."
        );
        procedures[nasakom.ID] = (
            "Nasakom",
            "Nasakom is a combination of nationalism, religion, and communism and was an attempt to fuse the three major ideologies of Indonesia into one government-mandated ideology, ostensibly to maintain stability."
        );
        procedures[pancasila.ID] = (
            "Pancasila",
            "Pancasila is Indonesia's founding ideology, a form of socialist nationalism that has proven rather controversial, especially to the Islamists. The end of Liberal Democracy put an end to legislative opposition to the ideology, and political factions are now primarily evaluated by their adherence to it."
        );
        procedures[houseIslam.ID] = (
            "House of Islam",
            "The House of Islam, an Islamist organisation, has been rebelling across Indonesia for a decade, thereby damaging popular and governmental support for Islamists."
        );
        procedures[eastSuez.ID] = (
            "East of Suez",
            "Indonesia's foreign policy is primarily concerned with the UK's East of Suez policy, which is seen as a threat to Indonesian influence and independence."
        );
        //procedures[industryTakeover.ID] = (
        //    "Industry Takeover",
        //    "The communists have unilaterally seized foreign industry in the past, and many in the government believe that the only way to stop this is to pre-empt the communists and seize foreign industry in the name of the government."
        //);
        procedures[rubberStamp.ID] = (
            "Rubber Stamp",
            "Reverting to the Constitution of 1945 has several implications - among them, that the president is also the head of government. In other words, he wields extraordinary legislative power, in addition to his moral authority."
        );
        procedures[veto.ID] = (
            "Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );
        procedures[coup.ID] = (
            "Coup",
            "The military may play a powerful role in politics, but its officers are extremely politicised and not necessarily loyal. It might not take much convincing to turn a military parade into a march on Jakarta."
        );
        procedures[purge.ID] = (
            "Purge",
            "The military has a special perception in Indonesian society, and if any faction grows too powerful or misguided, the military could step in to eliminate such a threat."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.RemoveProcedure,
                        [houseIslam.ID]
                    ),
                ],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influenceMasyumi],
                        -1
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influenceAbri],
                        1
                    ),
                ],
                [new (new AlwaysCondition (), 1)]
            )
        );
        Ballot ballotB = new (
            1,
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influenceAbri],
                        1
                    ),
                ],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influenceAbri, influencePki],
                        -1
                    ),
                ],
                [new (new AlwaysCondition (), 3)]
            )
        );
        Ballot incidentA = new (
            2,
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influenceAbri, influencePki],
                        1
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.RemoveProcedure,
                        [eastSuez.ID]
                    ),
                ],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influencePki],
                        -1
                    ),
                ],
                [new (new AlwaysCondition (), 3)]
            ),
            true
        );
        Ballot ballotC = new (
            3,
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influencePki],
                        1
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influenceMasyumi],
                        1
                    ),
                ],
                [new (new AlwaysCondition (), 4)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influenceAbri],
                        1
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [influencePki],
                        -1
                    ),
                ],
                []
            )
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
        List<Ballot> ballots = [ballotA, ballotB, incidentA, ballotC, incidentB];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Reconcile with the House of Islam",
            [
                StringLineFormatter.Indent ("House of Islam has been rebelling for a decade", 1),
                StringLineFormatter.Indent ("It wants to establish an Islamic state in Indonesia", 2),
                StringLineFormatter.Indent ("However, the rebellion is almost defeated", 2),
                StringLineFormatter.Indent ("Aceh and Borneo are on the verge of reclamation", 3),
                StringLineFormatter.Indent ("Pasundan and Celebes will take longer to pacify", 3),
                StringLineFormatter.Indent ("Completely defeating the rebellion would greatly weaken all Islamists, even moderate ones, and promote the image of the ABRI", 3),
                StringLineFormatter.Indent ("This would lessen tensions, especially in Aceh", 2),
                "House of Islam will be militarily suppressed if this ballot fails",
            ],
            [
                "House of Islam is destroyed",
                StringLineFormatter.Indent ("Aceh becomes an autonomous province", 1),
                StringLineFormatter.Indent ("Borneo, Celebes, and Pasundan surrender after a few years, receiving no concessions", 1),
                "Masyumi loses influence in the government",
                StringLineFormatter.Indent ("However, political circumstances could change this", 1),
            ],
            [
                "House of Islam is brutally crushed",
                StringLineFormatter.Indent ("Masyumi is purged and NU is closely monitored", 1),
                StringLineFormatter.Indent ("ABRI is hailed as a bringer of stability", 1),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Invade Dutch New Guinea",
            [
                StringLineFormatter.Indent ("Dutch New Guinea was originally part of the Dutch East Indies", 1),
                StringLineFormatter.Indent ("However, in the process of gaining independence, the Netherlands retained control over western New Guinea", 2),
                StringLineFormatter.Indent ("This has led to anti-imperialist sentiment among Indonesians", 3),
                StringLineFormatter.Indent ("Nobody in Dutch New Guinea wants to be part of Indonesia", 3),
                StringLineFormatter.Indent ("New Guineans are Melanesians, not Malayic, and are Christian", 4),
                StringLineFormatter.Indent ("In fact, Dutch New Guinea is on the verge of independence", 4),
                StringLineFormatter.Indent ("Military has been bolstered by Soviet aid", 1),
                StringLineFormatter.Indent ("Though the Netherlands is still better equipped and prepared, their supply lines are extremely long and untenable", 2),
            ],
            [
                "Invasion fails spectacularly",
                StringLineFormatter.Indent ("Dutch forces were already aware of invasion points and easily repel most attacks", 1),
                StringLineFormatter.Indent ("Some airdrops succeed and now there is a small contingent of Indonesian troops in Dutch New Guinea", 2),
                StringLineFormatter.Indent ("However, the Dutch are unwilling to fight another long war to defend a colonial territory", 2),
                StringLineFormatter.Indent ("As a result, the UN transfers ownership of Dutch New Guinea to Indonesia", 4),
            ],
            [
                "Anti-imperialist rhetoric is seen as posturing",
                StringLineFormatter.Indent ("Many Indonesians see Dutch New Guinea as part of the Indonesian homeland", 1),
                StringLineFormatter.Indent ("As a result, the people begin to distrust the ABRI and the PKI", 2),
                StringLineFormatter.Indent ("However, this reassures the US and the UK", 1),
                StringLineFormatter.Indent ("This should draw Indonesia out of the Soviet sphere", 2),
                StringLineFormatter.Indent ("Therefore, there may be an opportunity for foreign investment or alliances", 3),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Adopt the policy of Confrontation",
            [
                StringLineFormatter.Indent ("This will begin a border conflict with Malaysia", 1),
                StringLineFormatter.Indent ("Goal is to force Malaysia to cede Sarawak and Sabah", 2),
                StringLineFormatter.Indent ("Malaya has united with Sarawak, Sabah, and Singapore to form Malaysia", 1),
                StringLineFormatter.Indent ("However, Malaysia remains firmly within the British sphere and is key to the British East of Suez policy", 2),
                StringLineFormatter.Indent ("This also directly inhibits Indonesia’s historical ambitions to unite the Malay Archipelago", 2),
                StringLineFormatter.Indent ("Sarawak and Sabah both have significant ethnic minorities which do not appreciate Malayan dominance", 2),
                StringLineFormatter.Indent ("However, they likely will not appreciate Indonesian dominance either", 3),
            ],
            [
                "Confrontation is a complete failure",
                StringLineFormatter.Indent ("UK, Australia, and New Zealand help defend Malaysia", 1),
                StringLineFormatter.Indent ("President declares the Year of Living Dangerously", 1),
                StringLineFormatter.Indent ("Conflict expands unsuccessfully to the Malay Peninsula", 2),
                StringLineFormatter.Indent ("British and Malaysian units start counter-infiltrations", 2),
                StringLineFormatter.Indent ("Though the operations continue, many people question the government’s competence and intentions", 2),
            ],
            [
                "There is little reaction from most people",
                StringLineFormatter.Indent ("ABRI is relieved, but concerned with British influence", 1),
                StringLineFormatter.Indent ("However, the PKI is dissatisfied with inaction", 1),
                StringLineFormatter.Indent ("It claims that Malaysia is an imperialist puppet that the UK will use to dominate the Malay Archipelago", 2),
                "Malaysia reaches out for mutual investment",
                StringLineFormatter.Indent ("Peace could have great benefits for both Malaysia and the declining Indonesia", 1),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Implement Berdikari",
            [
                StringLineFormatter.Indent ("Berdikari is an economic idea stressing autarky", 1),
                StringLineFormatter.Indent ("This has only been adopted thus far as a motto, though some tenets have been previously implemented to some degree", 2),
                StringLineFormatter.Indent ("Government has previously relied upon the useless Eight Year Plan", 3),
                StringLineFormatter.Indent ("Berdikari:", 1),
                StringLineFormatter.Indent ("Complete all vital or unfinished infrastructure projects from the Eight Year Plan", 2),
                StringLineFormatter.Indent ("Limit imports to goods that can’t be produced domestically and expand exports", 2),
                StringLineFormatter.Indent ("Nationalise all foreign investment and industries", 2),
                StringLineFormatter.Indent ("In practice, this means Dutch, British, and American companies", 3),
            ],
            [
                "All foreign industry is seized",
                StringLineFormatter.Indent ("Economy falls further into collapse", 1),
                StringLineFormatter.Indent ("Most fields controlled by foreign industry were in fields where domestic industry had little-to-no experience", 2),
                StringLineFormatter.Indent ("US-aligned powers declare an embargo", 1),
                StringLineFormatter.Indent ("Indonesia can only trade with the USSR and China", 2),
                StringLineFormatter.Indent ("This pushes Indonesia into the communist sphere", 3),
                StringLineFormatter.Indent ("This is hailed as a victory for the anti-imperialist cause", 1),
                StringLineFormatter.Indent ("However, living conditions are getting worse and worse", 2),
                StringLineFormatter.Indent ("This drives the poor towards the Islamists, who are generally more pragmatic about economic issues", 3),
            ],
            [
                "PKI attempts to seize foreign assets anyway",
                StringLineFormatter.Indent ("It is eventually stopped by the ABRI", 1),
                "Relations with the West are gradually restored",
                StringLineFormatter.Indent ("Most foreign assets seized over the years are returned", 1),
                StringLineFormatter.Indent ("However, foreign powers don’t want to bail out Indonesia", 2),
                StringLineFormatter.Indent ("Much more drastic measures will be needed to save the economy", 3),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Reorganise the armed forces",
            [
                StringLineFormatter.Indent ("This will Nasakomise the armed forces and create a Fifth Force", 1),
                StringLineFormatter.Indent ("Nasakomisation will subject the armed forces to total government control", 2),
                StringLineFormatter.Indent ("This will be achieved through the use of political commissars and re-education", 3),
                StringLineFormatter.Indent ("Fifth Force would be made of armed militias not subject to governmental control", 2),
                StringLineFormatter.Indent ("This would essentially create an army for the PKI", 3),
                StringLineFormatter.Indent ("All of these measures would drastically weaken the influence and the independence of the ABRI", 2),
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
                new (new BallotPassedCondition (ballotB.ID, true), 1),
                new (new BallotPassedCondition (ballotB.ID, false), 8),
            ]
        );
        Result ballotBPassed = new (
            1,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 2),
                new (new BallotPassedCondition (ballotC.ID, false), 5),
            ]
        );
        Result ballotCPassed = new (
            2,
            [
                new (new BallotPassedCondition (incidentB.ID, true), 3),
                new (new BallotPassedCondition (incidentB.ID, false), 4),
            ]
        );
        Result incidentBPassed = new (
            3,
            []
        );
        Result incidentBFailed = new (
            4,
            []
        );
        Result ballotCFailed = new (
            5,
            [
                new (new BallotPassedCondition (ballotA.ID, true), 6),
                new (new BallotPassedCondition (ballotA.ID, false), 7),
            ]
        );
        Result ballotAPassed = new (
            6,
            []
        );
        Result ballotAFailed = new (
            7,
            []
        );
        Result ballotBFailed = new (
            8,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 9),
                new (new BallotPassedCondition (ballotC.ID, false), 10),
            ]
        );
        Result ballotCPassed2 = new (
            9,
            []
        );
        Result ballotCFailed2 = new (
            10,
            [
                new (new BallotPassedCondition (ballotA.ID, true), 6),
                new (new BallotPassedCondition (ballotA.ID, false), 11),
            ]
        );
        Result ballotAFailed2 = new (
            11,
            []
        );
        List<Result> results = [
            resultStart,
            ballotBPassed,
            ballotCPassed,
            incidentBPassed,
            incidentBFailed,
            ballotCFailed,
            ballotAPassed,
            ballotAFailed,
            ballotBFailed,
            ballotCPassed2,
            ballotCFailed2,
            ballotAFailed2,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Annexation of Dutch New Guinea greatly enhances Indonesia’s prestige",
                StringLineFormatter.Indent ("Indonesia is now the dominant power in the Malay Archipelago", 1),
                "New Guineans are extremely furious",
                StringLineFormatter.Indent ("Though nothing has happened yet, there will certainly be an insurgency in the future", 1),
                StringLineFormatter.Indent ("New Guinea will be a major issue for the foreseeable future", 2),
            ]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "Indonesia, now in the communist sphere, struggles to satisfactorily implement Berdikari",
                StringLineFormatter.Indent ("Soviet and Chinese aid and expertise is instrumental in improving Indonesian industries", 1),
                StringLineFormatter.Indent ("However, there are almost no targets for export", 1),
                StringLineFormatter.Indent ("This makes Indonesia even more reliant on Soviet and Chinese goodwill", 2),
            ]
        );
        resultsLocs[incidentBPassed.ID] = (
            "Incident B Passed",
            [
                "Indonesia becomes a communist state",
                StringLineFormatter.Indent ("With the gradual weakening of the ABRI and the Islamists, the PKI is able to seize power", 1),
                StringLineFormatter.Indent ("This leads to total collapse", 1),
                StringLineFormatter.Indent ("Many industries were already being run inefficiently", 2),
                StringLineFormatter.Indent ("Inexperienced PKI ministers are unable to repair the state of the economy", 2),
                "Eventually, Indonesia requests a Chinese bailout, but this comes at the cost of ministerial positions",
                StringLineFormatter.Indent ("Indonesia becomes a Chinese puppet state", 1),
            ]
        );
        resultsLocs[incidentBFailed.ID] = (
            "Incident B Failed",
            [
                "Indonesia attempts to lessen its reliance on the communist sphere",
                StringLineFormatter.Indent ("However, this is largely unsuccessful", 1),
                StringLineFormatter.Indent ("Outside of Africa, there are few states that need or want Indonesian products", 2),
                StringLineFormatter.Indent ("Indonesia needs imports of most consumer goods", 2),
                "China, the US, and the USSR all fail to influence Indonesia",
                StringLineFormatter.Indent ("Isolationism will continue to impede Indonesia’s development, barring a major regime change", 1),
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "PKI is marginalised and eventually destroyed",
                StringLineFormatter.Indent ("This pushes Indonesia into the Western sphere", 1),
                StringLineFormatter.Indent ("US greatly expands aid and investment to secure Indonesia", 2),
                StringLineFormatter.Indent ("Though this improves the economy, it doesn’t result in universal growth", 3),
                StringLineFormatter.Indent ("ABRI institutes a military dictatorship without the PKI to counterbalance", 1),
                StringLineFormatter.Indent ("This causes serious concerns about whether democracy can be restored or not", 2),
            ]
        );
        resultsLocs[ballotAPassed.ID] = (
            "Ballot A Passed",
            [
                "Islamists begin resurging in popularity",
                StringLineFormatter.Indent ("Rebellion begins in Aceh to oppose foreign exploitation", 1),
                StringLineFormatter.Indent ("NU supports the rebellion, which prevents it from being suppressed", 2),
                StringLineFormatter.Indent ("With the emergence of an opposition faction, there is now a possibility for a return to democracy", 1),
                StringLineFormatter.Indent ("If the ABRI can’t improve living conditions, then it will likely be overthrown", 1),
            ]
        );
        resultsLocs[ballotAFailed.ID] = (
            "Ballot A Failed",
            [
                "Without any other opposition factions, the ABRI completely dominates",
                StringLineFormatter.Indent ("Regionalist dissent and rebellions are easily crushed", 1),
                "Economy only improves somewhat",
                StringLineFormatter.Indent ("There is no good reason for the Western states to invest in Indonesia", 1),
                StringLineFormatter.Indent ("Overall, Indonesia remains impoverished and the people are rebellious", 1),
                StringLineFormatter.Indent ("If the ABRI can’t maintain total dominance, this could result in a coup", 2),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Indonesia loses its credibility as an anti-imperialist power",
                StringLineFormatter.Indent ("Netherlands and the UK were the last major colonial powers in the Malay Archipelago", 1),
                StringLineFormatter.Indent ("Australia expands its influence", 1),
                StringLineFormatter.Indent ("Australia had previously focused on the Pacific, but realises that it must guarantee the Malay Archipelago’s stability", 2),
            ]
        );
        resultsLocs[ballotCPassed2.ID] = (
            "Ballot C Passed",
            [
                "Indonesia has no allies",
                StringLineFormatter.Indent ("Western states are infuriated by the seizure of their industry", 1),
                StringLineFormatter.Indent ("Passivity convinces China and the USSR that Indonesia isn’t worth supporting", 1),
                StringLineFormatter.Indent ("This means no financial aid or investment", 1),
                StringLineFormatter.Indent ("Population remains incredibly impoverished", 2),
                "ABRI splits into radical, opposing factions",
                StringLineFormatter.Indent ("After a failed coup, Indonesia collapses into civil war", 1),
                StringLineFormatter.Indent ("Indonesia becomes a proxy battleground", 2),
            ]
        );
        resultsLocs[ballotCFailed2.ID] = (
            "Ballot C Failed",
            [
                "PKI slowly loses influence",
                StringLineFormatter.Indent ("Indonesia is seen as having accepting Western dominance", 1),
                StringLineFormatter.Indent ("This greatly improves Western opinions of Indonesia", 2),
                StringLineFormatter.Indent ("US greatly expands investment to make use of its new client state", 2),
                StringLineFormatter.Indent ("Though revenues are unequally distributed, the large quantity of investment slowly increases prosperity overall", 3),
            ]
        );
        resultsLocs[ballotAFailed2.ID] = (
            "Ballot A Failed",
            [
                "Without any other opposition factions, the ABRI completely dominates",
                StringLineFormatter.Indent ("Regionalist dissent is easily suppressed", 1),
                "Economy improves steadily",
                StringLineFormatter.Indent ("Though there are always those who clamour for democracy, most are satisfied with the current state of affairs", 1),
                StringLineFormatter.Indent ("Indonesia remains a rare example of a functional and stable military dictatorship", 2),
                StringLineFormatter.Indent ("However, the ABRI will likely peacefully hand over power to civilian successors as international and domestic opinions shift", 3),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[incidentB.ID] = [coup.ID, purge.ID];
        History history = new (
            [ballotA.ID, ballotB.ID, incidentA.ID, ballotC.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            [],
            factions,
            currenciesValues,
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Indonesia",
                "Oligarchic Dictatorship",
                [
                    "Indonesia has been extremely unstable after gaining independence",
                    StringLineFormatter.Indent ("Indonesia is made of hundreds of different ethnicities", 1),
                    StringLineFormatter.Indent ("However, the Javanese are the dominant ethnicity and Java is the dominant polity, which has led to regionalist rebellions", 2),
                    StringLineFormatter.Indent ("Economic recession has begun due to lack of experience managing the economy", 1),
                    StringLineFormatter.Indent ("Government, previously a parliamentary republic, has been completely paralysed", 1),
                    StringLineFormatter.Indent ("As a result, Sukarno, the president and a war hero, declared Guided Democracy to solve the country’s problems", 2),
                ],
                "5 July 1959",
                "Restoration of the Constitution of 1945",
                "Guided Democracy",
                rolesLocs,
                "Speaker",
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
