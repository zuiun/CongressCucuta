using CongressCucuta.Converters;
using CongressCucuta.Data;

namespace CongressCucuta.Simulations;

internal class Argentina : ISimulation {
    public Simulation Simulation { get; }

    public Argentina () {
        IDType minister = Role.MEMBER;
        IDType president = Role.HEAD_STATE;
        IDType lieutenantGeneral = Role.RESERVED_1;
        IDType admiral = Role.RESERVED_2;
        IDType brigadierGeneral = Role.RESERVED_3;
        List<IDType> roles = [
            minister,
            president,
            lieutenantGeneral,
            admiral,
            brigadierGeneral,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[minister] = ("Minister", "Ministers");
        rolesLocs[president] = ("President", "Presidents");
        rolesLocs[admiral] = ("Admiral", "Admirals");
        rolesLocs[lieutenantGeneral] = ("Lieutenant General", "Lieutenant Generals");
        rolesLocs[brigadierGeneral] = ("Brigadier General", "Brigadier Generals");

        Dictionary<IDType, sbyte> currenciesValues = [];
        IDType opposition = Currency.STATE;
        currenciesValues[opposition] = 0;
        Dictionary<IDType, string> currencies = [];
        currencies[opposition] = "Opposition";

        ProcedureImmediate authoritarianBureaucraticState = new (
            0,
            [
                new (Procedure.Effect.ActionType.ElectionNominated, [lieutenantGeneral]),
                new (Procedure.Effect.ActionType.ElectionNominated, [admiral, lieutenantGeneral]),
                new (Procedure.Effect.ActionType.ElectionNominated, [brigadierGeneral, lieutenantGeneral, admiral]),
            ]
        );
        ProcedureImmediate statuteArgentineRevolution = new (
            1,
            [new (Procedure.Effect.ActionType.ElectionNominated, [president, lieutenantGeneral, admiral, brigadierGeneral])]
        );
        List<ProcedureImmediate> proceduresGovernmental = [authoritarianBureaucraticState, statuteArgentineRevolution];
        ProcedureTargeted violetDominance = new (
            2,
            [
                new (Procedure.Effect.ActionType.CurrencyInitialise, []),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [minister], 0),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [president, lieutenantGeneral, admiral, brigadierGeneral], 1),
            ],
            []
        );
        ProcedureTargeted peronists = new (
            3,
            [new (Procedure.Effect.ActionType.VoteFailAdd, [], 1)],
            [0, 1, 2]
        );
        ProcedureTargeted vandorists = new (
            4,
            [new (Procedure.Effect.ActionType.CurrencySubtract, [], 1)],
            []
        );
        ProcedureTargeted politicalViolence = new (
            5,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [], 1)],
            []
        );
        ProcedureTargeted nightLongBatons = new (
            6,
            [
                new (Procedure.Effect.ActionType.VoteFailAdd, [], 1),
                new (Procedure.Effect.ActionType.CurrencyAdd, [], 1)
            ],
            [1, 2],
            false
        );
        ProcedureTargeted cycleAzos = new (
            7,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [], 2)],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [violetDominance, peronists, vandorists, politicalViolence, nightLongBatons, cycleAzos];
        ProcedureDeclared decree = new (
            8,
            [new (Procedure.Effect.ActionType.BallotPass, [])],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            [president]
        );
        ProcedureDeclared veto = new (
            9,
            [new (Procedure.Effect.ActionType.BallotFail, [])],
            new (Procedure.Confirmation.CostType.Always),
            [president]
        );
        ProcedureDeclared coup = new (
            10,
            [
                new (Procedure.Effect.ActionType.ElectionAppointed, [president]),
                new (Procedure.Effect.ActionType.ElectionAppointed, [lieutenantGeneral, president]),
            ],
            new (Procedure.Confirmation.CostType.Always),
            [lieutenantGeneral]
        );
        ProcedureDeclared rebellion = new (
            11,
            [
                new (Procedure.Effect.ActionType.ElectionAppointed, [president]),
                new (Procedure.Effect.ActionType.ElectionAppointed, [lieutenantGeneral, president]),
                new (Procedure.Effect.ActionType.ElectionAppointed, [admiral, president, lieutenantGeneral]),
                new (Procedure.Effect.ActionType.ElectionAppointed, [brigadierGeneral, president, lieutenantGeneral, admiral]),
            ],
            new (Procedure.Confirmation.CostType.DiceValue, 4),
            [admiral, brigadierGeneral]
        );
        List<ProcedureDeclared> proceduresDeclared = [decree, veto, coup, rebellion];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[authoritarianBureaucraticState.ID] = (
            "Authoritarian-Bureaucratic State",
            "This country has resorted to military rule in order to restore stability."
        );
        procedures[statuteArgentineRevolution.ID] = (
            "Statute of the Argentine Revolution",
            "Unlike many other countries with military regimes, this country is planning to institutionalise its dictatorship as a revolutionary government."
        );
        procedures[violetDominance.ID] = (
            "Violet Dominance",
            "The Blues and the Reds have just concluded a military conflict and have since united as the Violets in order to save Argentina from Peronism."
        );
        procedures[peronists.ID] = (
            "Peronists",
            "The Peronists are a powerful political movement represented by the Justicialist Party and whose ideologue, Juan Perón, is currently exiled in Spain. The movement may be fractured over policy, but it remains a dangerous threat to the military regime."
        );
        procedures[vandorists.ID] = (
            "Vandorists",
            "While Perón may personally abhor the military regime, one faction has united behind Augusto Vandor in the belief that Peronism is better off without him. This faction is willing to cooperate with the military if it leads to real change."
        );
        procedures[politicalViolence.ID] = (
            "Political Violence",
            "Argentine politics have always been coloured by political violence, and the current peace is unlikely to last for very long."
        );
        procedures[nightLongBatons.ID] = (
            "Night of the Long Batons",
            Localisation.UNUSED
        );
        procedures[cycleAzos.ID] = (
            "Cycle of the Azos",
            Localisation.UNUSED
        );
        procedures[decree.ID] = (
            "Decree Law",
            "This country's head of state is empowered to promulgate legislation without legislative oversight."
        );
        procedures[veto.ID] = (
            "Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );
        procedures[coup.ID] = (
            "Coup",
            "This country's head of government is part of the military and could be forcibly replaced if he is seen to be inept."
        );
        procedures[rebellion.ID] = (
            "Rebellion",
            "This country's government relies on the support of the military and could fall if the military decided to overthrow the government."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ReplaceProcedure, [peronists.ID, nightLongBatons.ID])],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [opposition], 1)],
                [new (new AlwaysCondition (), 1)]
            )
        );
        Ballot ballotB = new (
            1,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.RemoveProcedure, [vandorists.ID])],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [opposition], -1)],
                [new (new AlwaysCondition (), 3)]
            )
        );
        Ballot incidentA = new (
            2,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ReplaceProcedure, [politicalViolence.ID, cycleAzos.ID])],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [opposition], -1)],
                [new (new AlwaysCondition (), 3)]
            ),
            true
        );
        Ballot ballotC = new (
            3,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.RemoveProcedure, [politicalViolence.ID, cycleAzos.ID])],
                [new (new AlwaysCondition (), 4)]
            ),
            new Ballot.Result (
                [],
                []
            )
        );
        Ballot incidentB = new (
            4,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [opposition], 1)],
                []
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [opposition], -1)],
                []
            ),
            true
        );
        List<Ballot> ballots = [ballotA, ballotB, incidentA, ballotC, incidentB];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Reverse university reforms",
            [
                StringLineFormatter.Indent ("Argentina has several freedoms guaranteed for universities", 1),
                StringLineFormatter.Indent ("These reforms include institutional autonomy, democratic co-governance, and secular education", 2),
                StringLineFormatter.Indent ("Many students and professors oppose the regime", 1),
                StringLineFormatter.Indent ("Economy was improving, so there was no real reason to overthrow the democratically elected government", 2),
                StringLineFormatter.Indent ("There are some concerns that the universities may harbour communist sympathisers", 2),
                StringLineFormatter.Indent ("Reversing reforms would mean violent suppression", 2),
                StringLineFormatter.Indent ("Opposition is so vehement that the students will forcibly occupy the universities to prevent them from falling under government control", 3),
            ],
            [
                "Students and professors forcibly occupy the universities to prevent the reversal of reforms",
                StringLineFormatter.Indent ("Police violently expel the occupiers, including foreigners", 1),
                StringLineFormatter.Indent ("This leads to massive brain drain and international outrage", 2),
                StringLineFormatter.Indent ("This clamps down on dissident elements", 1),
                StringLineFormatter.Indent ("However, this will promote more dissent in the long run", 2),
            ],
            [
                "Students remain the most outspoken critics of the regime",
                StringLineFormatter.Indent ("Respecting university autonomy does nothing to quiet their opposition to totalitarianism", 1),
                StringLineFormatter.Indent ("However, this does prevent the students from gaining much support", 2),
                StringLineFormatter.Indent ("Nobody really knows what to criticise about a regime that has just come into power, other than its illegitimacy", 3),
                StringLineFormatter.Indent ("Regime will likely never win the support of the students", 2),
                StringLineFormatter.Indent ("Public support will have to come from other sectors of society", 3),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Suppress the unions",
            [
                StringLineFormatter.Indent ("This would suspend collective bargaining and outlaw strikes", 1),
                StringLineFormatter.Indent ("This will cause labour unions to become extremely weak", 2),
                StringLineFormatter.Indent ("However, this will greatly please the companies, who have historically been greatly restricted under Peronism", 2),
                StringLineFormatter.Indent ("General Confederation of Labour (CGT) has split over its support for the regime", 1),
                StringLineFormatter.Indent ("Vandor's CGT-Azopardo wants to collaborate, if only to preserve existing privileges", 2),
                StringLineFormatter.Indent ("CGT of the Argentines (CGTA) opposes the regime at every turn and has support from the clergy (liberation theology)", 2),
            ],
            [
                "There is little support for this measure",
                StringLineFormatter.Indent ("CGT-Azopardo begins to oppose the regime", 1),
                StringLineFormatter.Indent ("CGTA begins violent demonstrations and rebellions", 1),
                StringLineFormatter.Indent ("Guerrilla organisations begin to form", 1),
                StringLineFormatter.Indent ("Businesses are pleased with the suppression", 1),
                StringLineFormatter.Indent ("However, opposition by the CGTA decreases production, driving up prices and greatly weakening the economy", 2),
            ],
            [
                "CGT-Azopardo slowly overcomes the CGTA",
                StringLineFormatter.Indent ("Without too much anti-union action, the CGTA loses much of its appeal and its members return to the CGT-Azopardo", 1),
                StringLineFormatter.Indent ("Eventually, the CGTA is dismantled by lack of support and military suppression", 2),
                StringLineFormatter.Indent ("For now, the CGT-Azopardo agrees to cooperate with companies", 1),
                StringLineFormatter.Indent ("This is essentially reverting to the pre-coup situation", 2),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Freeze wages and prices",
            [
                StringLineFormatter.Indent ("Though inflation was originally low, it is now increasing at a concerning rate", 1),
                StringLineFormatter.Indent ("Liberalising the economy has led to economic growth at a major cost to domestic producers", 2),
                StringLineFormatter.Indent ("This would prohibit wage increases for two years and establish price controls for gas and agricultural products", 1),
                StringLineFormatter.Indent ("However, such a severe policy would greatly decrease national output", 2),
                StringLineFormatter.Indent ("If this policy fails, then both the labour unions and the companies will be extremely angry", 2),
            ],
            [
                "This policy fails",
                StringLineFormatter.Indent ("Companies are forced to increase prices to make up for low production", 1),
                StringLineFormatter.Indent ("Inflation increases even more quickly, worsening the already-bad trade deficit", 2),
                StringLineFormatter.Indent ("This does lead to some modernisation of industry, but this hardly comes as much solace", 2),
                StringLineFormatter.Indent ("There are now outright rebellions in many regions", 1),
            ],
            [
                "Not much happens",
                StringLineFormatter.Indent ("Labour unions and companies alike are grateful that no extreme measures are being taken", 1),
                StringLineFormatter.Indent ("Pursuing reduction of inflation at all costs is an extremely dangerous policy that could have resulted in economic collapse, given Argentina's current vulnerability", 2),
                StringLineFormatter.Indent ("Inflation grows and domestic products continue to lose ground to imports", 1),
                StringLineFormatter.Indent ("This is nothing new and won't lead to an economic disaster", 2),
                StringLineFormatter.Indent ("However, economic growth and national output are hurting", 3),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Adopt the Great National Accord",
            [
                StringLineFormatter.Indent ("This will call for elections, with the possibility of restoring democracy if the anti-military faction wins", 1),
                StringLineFormatter.Indent ("In order to facilitate elections, political parties will be legalised and their assets will be returned", 2),
                StringLineFormatter.Indent ("Peronists will not be allowed to participate in this election", 2),
                StringLineFormatter.Indent ("This should only be done if the Argentine Revolution has failed or is completed", 1),
                StringLineFormatter.Indent ("If the Argentine Revolution is still in progress, there is no guarantee that a democratic government will continue to carry out reforms", 2),
            ],
            [
                "Response to the Great National Accord is mixed",
                StringLineFormatter.Indent ("Nationalist sectors of the army oppose it", 1),
                StringLineFormatter.Indent ("They believe that democracy would weaken Argentina", 2),
                StringLineFormatter.Indent ("Peronists and liberals oppose it", 1),
                StringLineFormatter.Indent ("They argue that this is not true democracy", 2),
                StringLineFormatter.Indent ("Radicals support it", 1),
                StringLineFormatter.Indent ("This is probably the only chance they will ever have of getting in power", 2),
            ],
            [
                "Most people question whether the Argentine Revolution will ever actually be completed",
                StringLineFormatter.Indent ("Few beneficial reforms have been undertaken while the military was in power", 1),
                StringLineFormatter.Indent ("Whether the military regime can survive or not will ultimately depend on popular support", 1),
                StringLineFormatter.Indent ("Most of the military supports continuing the dictatorship, but some believe that giving way to democracy may have saved the military's reputation and influence", 2),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Lift Perón's exile",
            [
                StringLineFormatter.Indent ("Now that elections are being held, Perón is demanding to return to Argentina", 1),
                StringLineFormatter.Indent ("Perón and his family are the most prominent figures in Argentine politics", 2),
                StringLineFormatter.Indent ("Peronists are split on whether to bring him back or not", 2),
                StringLineFormatter.Indent ("Generally, there is wide support for this", 3),
                StringLineFormatter.Indent ("If Perón returns, then the military's favoured candidate in the upcoming election will probably be defeated", 1),
                StringLineFormatter.Indent ("However, this could help the military rehabilitate its image", 2),
            ],
            [
                "Perón returns to Argentina",
                StringLineFormatter.Indent ("He is met with cheering crowds, despite attempts to prevent gatherings", 1),
                StringLineFormatter.Indent ("Perón feverishly works to unite Peronist and anti-Peronist parties into an electoral alliance", 1),
                StringLineFormatter.Indent ("Whether this will successfully unseat the military is uncertain, but chances are very high", 2),
            ],
            [
                "Perón decries the military regime",
                StringLineFormatter.Indent ("However, no major realignment takes place", 1),
                StringLineFormatter.Indent ("There are many Peronists who don't want Perón in power", 2),
                StringLineFormatter.Indent ("This worsens the split between the Peronist parties", 3),
                StringLineFormatter.Indent ("Those who care about Perón's complaints are already in opposition anyway", 2),
                "Politics become increasingly multipolar",
                StringLineFormatter.Indent ("Without Perón, there is little chance of a unified front forming against the military", 1),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 1),
                new (new BallotPassedCondition (ballotC.ID, false), 4),
            ]
        );
        Result ballotCPassed = new (
            1,
            [
                new (new CurrencyValueCondition (opposition, ICondition.ComparisonType.FewerThanOrEqual, 1), 2),
                new (new CurrencyValueCondition (opposition, ICondition.ComparisonType.GreaterThanOrEqual, 2), 3),
            ]
        );
        Result oneFewerOpposition = new (
            2,
            []
        );
        Result twoGreaterOpposition = new (
            3,
            []
        );
        Result ballotCFailed = new (
            4,
            [
                new (new CurrencyValueCondition (opposition, ICondition.ComparisonType.FewerThanOrEqual, 1), 5),
                new (new CurrencyValueCondition (opposition, ICondition.ComparisonType.GreaterThanOrEqual, 2), 6),
            ]
        );
        Result oneFewerOpposition2 = new (
            5,
            []
        );
        Result twoGreaterOpposition2 = new (
            6,
            []
        );
        List<Result> results = [
            resultStart,
            ballotCPassed,
            oneFewerOpposition,
            twoGreaterOpposition,
            ballotCFailed,
            oneFewerOpposition2,
            twoGreaterOpposition2,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "Elections are held",
                StringLineFormatter.Indent ("Peronists are represented by the Justicialist Liberation Front", 1),
                StringLineFormatter.Indent ("Anti-Peronists are represented by the Radical Civic Union", 1),
                StringLineFormatter.Indent ("Military is represented by the Popular Federalist Alliance", 1),
            ]
        );
        resultsLocs[oneFewerOpposition.ID] = (
            "One or Fewer Opposition",
            [
                "Peronists barely win a plurality of votes",
                StringLineFormatter.Indent ("Military makes some concessions to the labour unions in order to win the support of the anti-Peronists", 1),
                StringLineFormatter.Indent ("As a result, the military barely wins the run-off elections", 2),
                "Now in power, the military respects the democratic regime",
                StringLineFormatter.Indent ("Promised concessions are implemented, but there remains significant Peronist opposition", 1),
                StringLineFormatter.Indent ("Most opposition is limited to protests, rather than rebellions", 2),
                StringLineFormatter.Indent ("Over time, it is likely a political shift will take place and Peronism will become a weaker political force", 2),
            ]
        );
        resultsLocs[twoGreaterOpposition.ID] = (
            "Two or Greater Opposition",
            [
                "Peronists win a plurality of votes",
                StringLineFormatter.Indent ("This technically does not win them the election, as a run-off election is necessary", 1),
                StringLineFormatter.Indent ("However, the President waives the run-off for the sake of national stability", 2),
                "Now in power, the Peronists begin reversing the Argentine Revolution",
                StringLineFormatter.Indent ("Though they have much popular support, the military is incredibly dissatisfied", 1),
                StringLineFormatter.Indent ("Argentina will remain unstable for years to come, unless a great national crisis can reunite the people", 2),
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "Regime attempts take back initiative by forcibly disappearing Peronists",
                StringLineFormatter.Indent ("This predictably leads to rebellions and guerrilla insurgencies in the countryside", 1),
                StringLineFormatter.Indent ("Insurgents have Chilean diplomatic support, which helps some of them evade capture", 2),
                StringLineFormatter.Indent ("Regime gains some US support in arms and materiel", 1),
            ]
        );
        resultsLocs[oneFewerOpposition2.ID] = (
            "One or Fewer Opposition",
            [
                "Over time, the insurgents lose support",
                StringLineFormatter.Indent ("Government comes to a negotiated settlement with the labour unions", 1),
                StringLineFormatter.Indent ("Most students only peacefully protest", 1),
                "Now that the military is in total control, the Argentine Revolution continues",
                StringLineFormatter.Indent ("Some protectionism is put in place, but most liberal reforms are maintained", 1),
                StringLineFormatter.Indent ("This allows Argentina to build up its economic base", 2),
                StringLineFormatter.Indent ("Eventually, the military may decide to restore democratic governance once it is satisfied with the growth of the economy", 3),
            ]
        );
        resultsLocs[twoGreaterOpposition2.ID] = (
            "Two or Greater Opposition",
            [
                "Insurgency gains traction",
                StringLineFormatter.Indent ("Cuban support helps perpetuate it", 1),
                StringLineFormatter.Indent ("Though the insurgents have no chance of overthrowing the government, the regime is spending unsustainable amounts of resources to suppress them", 1),
                StringLineFormatter.Indent ("As a result, no amount of economic reforms can save Argentina's economy", 2),
                "Eventually, Argentina collapses into anarchy",
                StringLineFormatter.Indent ("Local militias take control of many regions", 1),
                StringLineFormatter.Indent ("It rests upon foreign intervention to restore any semblance of order to Argentina", 2),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[incidentA.ID] = [coup.ID];
        ballotsProceduresDeclared[ballotC.ID] = [coup.ID];
        History history = new (
            [ballotA.ID, ballotB.ID, incidentA.ID, ballotC.ID, incidentB.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            [],
            [],
            currenciesValues,
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Argentina",
                "Military Dictatorship",
                [
                    "Juan Perón, a populist, was elected president",
                    StringLineFormatter.Indent ("His interventionist policies and labour protections were extremely popular", 1),
                    StringLineFormatter.Indent ("These ideals came to be known as Peronism", 2),
                    StringLineFormatter.Indent ("However, these policies angered conservative Catholics", 2),
                    "More than a decade of instability passed",
                    StringLineFormatter.Indent ("Peronist and anti-Peronist tensions toppled both military and civilian governments alike", 1),
                    StringLineFormatter.Indent ("Eventually, the military launched a final coup to end the cycle and create a new Argentina", 2),
                    StringLineFormatter.Indent ("This military dictatorship was intended to be permanent", 3),
                ],
                "29 June 1966",
                "Ascension of Juan Onganía",
                "The Argentine Revolution",
                rolesLocs,
                "Secretary of the Government",
                (Localisation.UNUSED, Localisation.UNUSED),
                [],
                (Localisation.UNUSED, Localisation.UNUSED),
                [],
                [],
                currencies,
                procedures,
                ballotsLocs,
                resultsLocs
            )
        );
    }
}
