using congress_cucuta.Converters;
using congress_cucuta.Data;

namespace congress_cucuta.Simulations;

internal class Hungary : ISimulation {
    public Simulation Simulation { get; }

    public Hungary () {
        IDType member = Role.MEMBER;
        IDType primeMinister = Role.HEAD_GOVERNMENT;
        IDType regent = Role.HEAD_STATE;
        List<IDType> roles = [
            member,
            primeMinister,
            regent,
            0,
            1,
            2,
            3,
            Role.LEADER_PARTY,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[member] = ("Member", "Members");
        rolesLocs[primeMinister] = ("Prime Minister", "Prime Ministers");
        rolesLocs[regent] = ("Regent", "Regents");
        rolesLocs[0] = ("Party Leader of EP", "Party Leaders of EP");
        rolesLocs[1] = ("Party Leader of MSZDP", "Party Leaders of MSZDP");
        rolesLocs[2] = ("Party Leader of FKgP", "Party Leaders of FKgP");
        rolesLocs[3] = ("Party Leader of NAP", "Party Leaders of NAP");
        rolesLocs[Role.LEADER_PARTY] = ("Party Leader", "Party Leaders");

        Faction ep = new (0);
        Faction mszdp = new (1);
        Faction fkgp = new (2, false);
        Faction nap = new (3, false);
        List<Faction> parties = [ep, mszdp, fkgp, nap];
        Dictionary<IDType, (string, string[])> partiesLocs = [];
        partiesLocs[ep.ID] = (
            "Unity Party",
            [
                StringLineFormatter.Indent ("Relatively diverse umbrella party that holds nationalism at its core", 1),
                StringLineFormatter.Indent ("Foreign policy completely concerned with the revision of the Treaty of Trianon", 2),
            ]
        );
        partiesLocs[mszdp.ID] = (
            "Social Democratic Party of Hungary",
            [
                StringLineFormatter.Indent ("Liberal and socialist", 1),
                StringLineFormatter.Indent ("Only powerful party that agitates for democracy and equality", 2),
                StringLineFormatter.Indent ("Pacifist", 1),
                StringLineFormatter.Indent ("Doesn't want to anger the great powers over the Treaty of Trianon", 2),
            ]
        );
        partiesLocs[fkgp.ID] = (
            "Independent Smallholders' Party",
            []
        );
        partiesLocs[nap.ID] = (
            "Party of National Will",
            []
        );
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[ep.ID] = "EP";
        abbreviations[mszdp.ID] = "MSZDP";
        abbreviations[fkgp.ID] = "FKgP";
        abbreviations[nap.ID] = "NAP";

        Dictionary<IDType, sbyte> currenciesValues = [];
        IDType antisemitism = Currency.STATE;
        currenciesValues[antisemitism] = 1;
        Dictionary<IDType, string> currencies = [];
        currencies[antisemitism] = "Anti-Semitism";

        ProcedureImmediate kingdomWithoutKing = new (
            0,
            [
                new (Procedure.Effect.ActionType.ElectionNominated, [regent], 0),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [regent], 0),
            ]
        );
        ProcedureImmediate legislativeElection = new (
            1,
            [
                new (Procedure.Effect.ActionType.ElectionParty, [regent]),
                new (Procedure.Effect.ActionType.ElectionNominated, [primeMinister, regent]),
            ]
        );
        List<ProcedureImmediate> proceduresGovernmental = [kingdomWithoutKing, legislativeElection];
        ProcedureTargeted bulwarkAgainstBolshevism = new (
            2,
            [new (Procedure.Effect.ActionType.CurrencyInitialise, [])],
            []
        );
        ProcedureTargeted trianonTrauma = new (
            3,
            [new (Procedure.Effect.ActionType.VotePassAdd, [], 1)],
            [3, 5]
        );
        ProcedureTargeted bethlenPeyerPact = new (
            4,
            [
                new (Procedure.Effect.ActionType.ProcedureActivate, [legislativeElection.ID]),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [mszdp.ID], 0),
            ],
            []
        );
        ProcedureTargeted consolidation = new (
            5,
            [new (Procedure.Effect.ActionType.PermissionsVotes, [ep.ID], 1)],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            bulwarkAgainstBolshevism,
            trianonTrauma,
            bethlenPeyerPact,
            consolidation,
        ];
        ProcedureDeclared royalPrerogative = new (
            6,
            [new (Procedure.Effect.ActionType.BallotPass, [])],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [regent]
        );
        ProcedureDeclared royalVeto = new (
            7,
            [new (Procedure.Effect.ActionType.BallotFail, [])],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [regent]
        );
        ProcedureDeclared dissolutionDiet = new (
            8,
            [
                new (Procedure.Effect.ActionType.ElectionParty, [regent]),
                new (Procedure.Effect.ActionType.ElectionNominated, [primeMinister, regent]),
            ],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [regent]
        );
        List<ProcedureDeclared> proceduresDeclared = [royalPrerogative, royalVeto, dissolutionDiet];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[kingdomWithoutKing.ID] = (
            "Kingdom Without a King",
            "This country's head of state is chosen based on familial lineage, but the absence of a legitimate ruler means that a regent governs in his stead."
        );
        procedures[legislativeElection.ID] = (
            "Legislative Election",
            "This country has a legislature with regular elections, along with an empowered head of government."
        );
        procedures[bulwarkAgainstBolshevism.ID] = (
            "Bulwark Against Bolshevism",
            "After the collapse of Béla Kun's regime in 1920, Hungarian society has swung to the right and is deeply suspicious of leftist ideals. Furthermore, the involvement of Jews in the revolution has greatly heightened anti-Semitism in Hungarian society."
        );
        procedures[trianonTrauma.ID] = (
            "Trianon Trauma",
            "Politics in Hungary are dominated by revisionism regarding the Treaty of Trianon, which reduced Hungary's territory by two-thirds and left over three million Hungarians in foreign states. Popular slogans include \"No, no, never!\" and \"Return everything!\""
        );
        procedures[bethlenPeyerPact.ID] = (
            "Bethlen-Peyer Pact",
            "The Bethlen-Peyer Pact legalised the Social Democratic Party, but greatly limited the influence of labour unions, which is seen by many on the left as a great betrayal."
        );
        procedures[consolidation.ID] = (
            "Consolidation",
            "Hungary is currently receiving foreign loans and investment to rebuild and stabilise its economy, and the regent has backed and empowered the Unity Party in order to pursue these ends."
        );
        procedures[royalPrerogative.ID] = (
            "Royal Prerogative",
            "This country's head of state is empowered to promulgate legislation without legislative oversight."
        );
        procedures[royalVeto.ID] = (
            "Royal Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );
        procedures[dissolutionDiet.ID] = (
            "Dissolution of the Diet",
            "This country's legislature is responsible to the head of state, which means that it can be replaced if it is seen to be inept."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [antisemitism], 1)],
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
                [new Ballot.Effect (Ballot.Effect.ActionType.FoundParty, [fkgp.ID])],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [antisemitism], 1)],
                [new (new AlwaysCondition (), 2)]
            ),
            true
        );
        Ballot ballotB = new (
            2,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.FoundParty, [nap.ID])],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 4)]
            )
        );
        Ballot incidentB = new (
            3,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.RemoveProcedure, [consolidation.ID])],
                [new (new AlwaysCondition (), 5)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 5)]
            ),
            true
        );
        Ballot incidentC = new (
            4,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.RemoveProcedure, [bethlenPeyerPact.ID, consolidation.ID])],
                [new (new AlwaysCondition (), 5)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 5)]
            ),
            true
        );
        Ballot ballotC = new (
            5,
            new Ballot.Result (
                [],
                []
            ),
            new Ballot.Result (
                [],
                []
            )
        );
        
        List<Ballot> ballots = [ballotA, incidentA, ballotB, incidentB, incidentC, ballotC];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Introduce racial quotas in universities",
            [
                StringLineFormatter.Indent ("This would admit students to universities based on their overall percentage of the population", 1),
                StringLineFormatter.Indent ("Only Jews are over-represented in university studies", 2),
                StringLineFormatter.Indent ("There is much anger over the apparent supremacy of Jews in society", 3),
                StringLineFormatter.Indent ("Jews are also strongly associated with the failed communist revolution in Hungary", 4),
                StringLineFormatter.Indent ("Reserving seats for Hungarian students would appease right-wing parties", 2),
                StringLineFormatter.Indent ("However, this is sure to anger the League of Nations", 3),
                StringLineFormatter.Indent ("Hungary is only being kept afloat by generous loans from the great powers", 4),
            ],
            [
                "This is a very popular measure",
                StringLineFormatter.Indent ("Not very many people outright hate Jews, but everybody likes better opportunities for education", 1),
                StringLineFormatter.Indent ("However, the League of Nations is displeased", 1),
                StringLineFormatter.Indent ("This could derail the massive deficit spending that is currently taking place to stimulate the economy", 2),
            ],
            [
                "Nothing really changes",
                StringLineFormatter.Indent ("There is some backlash by right-wing parties", 1),
                StringLineFormatter.Indent ("However, they don't have enough power to be meaningful", 2),
                StringLineFormatter.Indent ("League of Nations is pleased", 1),
                StringLineFormatter.Indent ("However, this only results in diplomatic support, not necessarily investment or loan opportunities", 2),
                StringLineFormatter.Indent ("Diplomatic support could still be useful in the future", 3),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Repeal anti-Semitic laws",
            [
                StringLineFormatter.Indent ("There has been significant international backlash to the racial quota and other anti-Semitic laws", 1),
                StringLineFormatter.Indent ("At the moment, this is the first obvious and most severe anti-Semitic law in existence", 2),
                StringLineFormatter.Indent ("Though few people actually care about Jews, many are concerned about rising Hungarian populism and nationalism", 3),
                StringLineFormatter.Indent ("This has resulted in less economic support from the League of Nations and the liberal West", 2),
                StringLineFormatter.Indent ("Though the economy is currently stable, various investment schemes depend on the generosity of the UK to provide capital for industrial projects", 3),
                StringLineFormatter.Indent ("Hungary will be forced closer to Italy if this situation isn't rectified", 3),
            ],
            [
                "Diplomatic support is restored",
                StringLineFormatter.Indent ("This could be used to gain more economic support from the West in the future", 1),
                StringLineFormatter.Indent ("However, there are many people who are displeased with Western meddling in Hungarian politics", 1),
                "This does not preclude further anti-Semitic laws in the future",
            ],
            [
                "Hungary continues to be shunned by the liberal world",
                StringLineFormatter.Indent ("However, Italy happily accepts a revisionist ally into its sphere", 1),
                StringLineFormatter.Indent ("Hungarian dependence on Italian ports and imports ties it even closer to the growing fascist movement", 2),
                StringLineFormatter.Indent ("Fascism is growing in both Hungary and Germany", 1),
                StringLineFormatter.Indent ("Fascism is still fringe in Hungary, but many rightists have fascist sympathies", 2),
                StringLineFormatter.Indent ("This could lead to eventual rapprochement with Germany", 3),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Sign a trade deal with Germany",
            [
                StringLineFormatter.Indent ("Great Depression has completely destroyed Hungary's export-based economy", 1),
                StringLineFormatter.Indent ("This has driven industrial workers and peasants into poverty, unemployment, and desperation", 2),
                StringLineFormatter.Indent ("This trade deal would exchange Hungarian wheat for German industrial equipment", 1),
                StringLineFormatter.Indent ("Hungary is one of Europe's largest wheat producers", 2),
                StringLineFormatter.Indent ("Hungary relies totally on its wheat exports to survive", 3),
                StringLineFormatter.Indent ("Germany is one of Europe's largest industrial powers", 2),
                StringLineFormatter.Indent ("Germany has other ways to get the wheat it needs, so it will practically dictate the terms of the agreement", 3),
            ],
            [
                "Hungary's economy is finally stable",
                StringLineFormatter.Indent ("Increased support from Germany greatly increases support for fascism", 1),
                StringLineFormatter.Indent ("Many hope that Germany could push for revision to the Treaty of Trianon", 2),
                StringLineFormatter.Indent ("However, Germany sees Hungary as a disposable pawn", 3),
                StringLineFormatter.Indent ("There is no more support for the liberal West", 1),
                StringLineFormatter.Indent ("There will no longer be any way to revise the Treaty of Trianon without war with the West", 2),
            ],
            [
                "Hungary resists German pressure",
                StringLineFormatter.Indent ("This greatly bolsters liberal parties", 1),
                StringLineFormatter.Indent ("Surge of liberal political thought in Hungary is very attractive to the West", 2),
                StringLineFormatter.Indent ("This has severe economic consequences", 1),
                StringLineFormatter.Indent ("Trade hasn't recovered at all and there is still no way for Hungary to expand its own industrial capabilities", 2),
                StringLineFormatter.Indent ("However, it should now be relatively easy to seek economic aid from the West", 2),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Establish corporatism",
            [
                StringLineFormatter.Indent ("Though the economy is now recovering, unemployment is still rampant", 1),
                StringLineFormatter.Indent ("Factory workers have difficulty finding work", 2),
                StringLineFormatter.Indent ("Even with German equipment, a strong industrial base takes a long time to develop, so labour opportunities are still rare", 3),
                StringLineFormatter.Indent ("Corporatism could be used to create job opportunities and better conditions for workers", 2),
                StringLineFormatter.Indent ("However, most industrial enterprises are state-owned, so this will likely have little effect on factory workers", 3),
                StringLineFormatter.Indent ("This will push Hungary firmly into the Italian sphere", 1),
                StringLineFormatter.Indent ("Germany will also see this favourably as fascist sentiment", 2),
            ],
            [
                "Hungary is well on its path to fascism",
                StringLineFormatter.Indent ("This simultaneously hurts the regent's control over parliamentary politics", 1),
                StringLineFormatter.Indent ("All that remains is to destroy all remnants of the parliamentary system", 1),
                StringLineFormatter.Indent ("This brings Hungary irreversibly into the German and the Italian spheres", 1),
                StringLineFormatter.Indent ("Without any other reliable allies, Hungary ultimately can't pursue much of an independent foreign policy", 2),
            ],
            [
                "Development of fascism is hindered",
                StringLineFormatter.Indent ("Hungary avoids being too heavily influenced by Germany and Italy", 1),
                StringLineFormatter.Indent ("However, this could result in less support in the future", 2),
                StringLineFormatter.Indent ("This is relatively popular", 1),
                StringLineFormatter.Indent ("Most Hungarians are proud of their heritage and would rather not be too influenced by foreign ideologies", 2),
            ]
        );
        ballotsLocs[incidentC.ID] = (
            "Incident C",
            "Petition the UK for aid",
            [
                StringLineFormatter.Indent ("This would guarantee British access to the Manfréd Weiss Csepel Works and wheat production in exchange for loans, protection, and industrial aid", 1),
                StringLineFormatter.Indent ("UK desperately needs steel and wheat imports", 2),
                StringLineFormatter.Indent ("Csepel Works is Hungary's only major steel mill", 3),
                StringLineFormatter.Indent ("Losing control of it could have severe effects on self-sufficiency", 4),
                StringLineFormatter.Indent ("This requires that Hungary drops its claims on Backa and Mura", 2),
                StringLineFormatter.Indent ("These lands hold significant Hungarian minorities but amount to a small amount of territory", 3),
                StringLineFormatter.Indent ("This is necessary as Yugoslavia is the only easily-accessible UK-aligned state that can allow transit of goods", 3),
            ],
            [
                "British aid comes along with a major expansion of democratic ideals",
                StringLineFormatter.Indent ("Liberals begin to demonstrate for a dismantling of the old political system, citing the Twelve Points of 1848", 1),
                StringLineFormatter.Indent ("This eventually leads to the rehabilitation of the MSZDP", 2),
                "Many people are somewhat angry",
                StringLineFormatter.Indent ("Losing control of steel production is a massive hit to national prestige", 1),
            ],
            [
                "Hungary attempts to carve out a non-aligned path for itself",
                StringLineFormatter.Indent ("However, this comes at a severe cost to the people", 1),
                StringLineFormatter.Indent ("Refusing aid prevents the economy from recovering", 2),
                StringLineFormatter.Indent ("Many people care about independence, but they care more about escaping poverty", 3),
                StringLineFormatter.Indent ("Without any major allies, it will be impossible to demand a revision of the Treaty of Trianon", 1),
                StringLineFormatter.Indent ("All of the relevant countries, except Austria, could easily defeat Hungary in a war", 2),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Demand revisions to the Treaty of Trianon",
            [
                StringLineFormatter.Indent ("This will be the culmination of all policy efforts", 1),
                StringLineFormatter.Indent ("Demanding revisions without enough support from a major ally could be extremely problematic", 1),
                StringLineFormatter.Indent ("Demands could be refused", 2),
                StringLineFormatter.Indent ("Hungary could be considered too dangerous to continue supporting", 2),
                StringLineFormatter.Indent ("War seems forthcoming over German revisionism", 1),
                StringLineFormatter.Indent ("Choosing not to pursue revision could keep Hungary neutral, or at least not result in punitive border changes", 2),
            ],
            ["Intentionally left blank"],
            ["Intentionally left blank"]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 1),
                new (new BallotPassedCondition (ballotB.ID, false), 9),
            ]
        );
        Result ballotBPassed = new (
            1,
            [
                new (new BallotPassedCondition (incidentB.ID, true), 2),
                new (new BallotPassedCondition (incidentB.ID, false), 8),
            ]
        );
        Result incidentBPassed = new (
            2,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 3),
                new (new BallotPassedCondition (ballotC.ID, false), 7),
            ]
        );
        Result ballotCPassed = new (
            3,
            [
                new (new CurrencyValueCondition (antisemitism, ICondition.ComparisonType.Equal, 1), 4),
                new (new CurrencyValueCondition (antisemitism, ICondition.ComparisonType.Equal, 2), 5),
                new (new CurrencyValueCondition (antisemitism, ICondition.ComparisonType.Equal, 3), 6),
            ]
        );
        Result oneAntisemitismCPassed = new (
            4,
            []
        );
        Result twoAntisemitism = new (
            5,
            []
        );
        Result threeAntisemitism = new (
            6,
            []
        );
        Result ballotCFailed = new (
            7,
            []
        );
        Result incidentBFailed = new (
            8,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 4),
                new (new BallotPassedCondition (ballotC.ID, false), 7),
            ]
        );
        Result ballotBFailed = new (
            9,
            [
                new (new BallotPassedCondition (incidentC.ID, true), 10),
                new (new BallotPassedCondition (incidentC.ID, false), 15),
            ]
        );
        Result incidentCPassed = new (
            10,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 11),
                new (new BallotPassedCondition (ballotC.ID, false), 14),
            ]
        );
        Result ballotCPassed2 = new (
            11,
            [
                new (new CurrencyValueCondition (antisemitism, ICondition.ComparisonType.Equal, 1), 12),
                new (new CurrencyValueCondition (antisemitism, ICondition.ComparisonType.GreaterThanOrEqual, 2), 13),
            ]
        );
        Result oneAntisemitism2 = new (
            12,
            []
        );
        Result twoGreaterAntisemitism = new (
            13,
            []
        );
        Result ballotCFailed2 = new (
            14,
            []
        );
        Result incidentCFailed = new (
            15,
            []
        );
        List<Result> results = [
            resultStart,
            ballotBPassed,
            incidentBPassed,
            ballotCPassed,
            oneAntisemitismCPassed,
            twoAntisemitism,
            threeAntisemitism,
            ballotCFailed,
            incidentBFailed,
            ballotBFailed,
            incidentCPassed,
            ballotCPassed2,
            oneAntisemitism2,
            twoGreaterAntisemitism,
            ballotCFailed2,
            incidentCFailed,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Germany becomes Hungary's only ally",
                StringLineFormatter.Indent ("Shift towards fascism has forced Hungary away from its traditional allies in the West", 1),
                StringLineFormatter.Indent ("This means that Western aid and trade slowly ends", 2),
                "Domestic politics becomes very polarised",
                StringLineFormatter.Indent ("Traditional power structures have been greatly disrupted", 1),
            ]
        );
        resultsLocs[incidentBPassed.ID] = (
            "Incident B Passed",
            [
                "As Europe falls further and further into chaos, Hungary is forced closer and closer to Germany",
                StringLineFormatter.Indent ("Hungary's economy is almost totally under German control", 1),
                StringLineFormatter.Indent ("Eventually, this results in Hungary submitting control of its foreign policy to Germany", 1),
            ]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "Hungary calls for revision of the Treaty of Trianon with German mediation",
                StringLineFormatter.Indent ("Whether Germany will support Hungary depends on how much Hungary subscribes to fascist ideals", 1),
            ]
        );
        resultsLocs[oneAntisemitismCPassed.ID] = (
            "One Anti-Semitism or Ballot C Passed",
            [
                "Germany refuses to support Hungary's claims",
                StringLineFormatter.Indent ("This is met with great anger", 1),
                StringLineFormatter.Indent ("When the rest of the Axis invades Yugoslavia, Hungary invades as well", 1),
                StringLineFormatter.Indent ("Hungary manages to occupy Backa", 2),
                "Hungary has made an enemy of Germany",
                StringLineFormatter.Indent ("Germany immediately annexes Hungary once at war with the USSR", 1),
                StringLineFormatter.Indent ("Hungary becomes a Soviet puppet after the war", 2),
                StringLineFormatter.Indent ("Hungary loses all of its gains", 3),
            ]
        );
        resultsLocs[twoAntisemitism.ID] = (
            "Two Anti-Semitism",
            [
                "Germany reluctantly supports Hungary's claims",
                StringLineFormatter.Indent ("Hungary gains Backa and southern Slovakia", 1),
                "Hungary has maintained some of its independent identity",
                StringLineFormatter.Indent ("Germany turns Hungary into a puppet state near the end of its war with the USSR", 1),
                StringLineFormatter.Indent ("Hungary becomes a Soviet puppet after the war", 2),
                StringLineFormatter.Indent ("Hungary loses all of its gains as well as some Czechoslovak border villages", 3),
            ]
        );
        resultsLocs[threeAntisemitism.ID] = (
            "Three Anti-Semitism",
            [
                "Germany eagerly supports Hungary's claims",
                StringLineFormatter.Indent ("Hungary gains Backa, southern Slovakia, Carpathian Ruthenia, and Szeklerland", 1),
                "Hungary has totally submitted to Germany",
                StringLineFormatter.Indent ("Germany turns Hungary into a puppet state once at with the USSR", 1),
                StringLineFormatter.Indent ("Hungary becomes a Soviet puppet after the war", 2),
                StringLineFormatter.Indent ("Hungary loses all of its gains as well as Transtisza and some Czechoslovak border villages", 3),
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "Hungary refuses to call for revision of the Treaty of Trianon",
                StringLineFormatter.Indent ("This completely contradicts Hungary's general shift to the right and is met with extreme anger", 1),
                StringLineFormatter.Indent ("This also makes Germany question Hungary's reliability as an ally", 2),
                StringLineFormatter.Indent ("However, once war breaks out, the people begin to calm down and worry instead about an invasion of Hungary", 2),
                StringLineFormatter.Indent ("Germany eventually invades Hungary once at war with the USSR", 3),
                "Hungary becomes a Soviet puppet after the war",
                StringLineFormatter.Indent ("There are no punitive border changes", 1),
            ]
        );
        resultsLocs[incidentBFailed.ID] = (
            "Incident B Failed",
            [
                "Hungary attempts not to fall into Germany's sphere",
                StringLineFormatter.Indent ("Germany is mostly satisfied with dominating Hungary's economy", 1),
                StringLineFormatter.Indent ("This prevents Hungary from being too independent anyway", 2),
                StringLineFormatter.Indent ("However, Germany is still angry that Hungary is attempting to subvert German domination", 2),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Hungary refuses to enter the German sphere",
                StringLineFormatter.Indent ("This is problematic as Germany is a natural ally due to its geographical location", 1),
                StringLineFormatter.Indent ("This makes economic development a greater challenge", 2),
                StringLineFormatter.Indent ("This sparks some interest by the Western powers, which want to strengthen their hold over the Balkans", 1),
                StringLineFormatter.Indent ("However, many are wary of Hungary's traditional enemies", 2),
            ]
        );
        resultsLocs[incidentCPassed.ID] = (
            "Incident C Passed",
            [
                "Hungary joins the British sphere",
                StringLineFormatter.Indent ("Many powerful people are Anglophiles, but this still comes as a surprise to the common people", 1),
                StringLineFormatter.Indent ("Concessions necessary to maintain British support infuriate many", 1),
                StringLineFormatter.Indent ("Nationalists are especially angry at the revocation of claims to Backa and Mura and British rights to the Csepel Works", 2),
                StringLineFormatter.Indent ("This severely weakens Hungary's position in the Balkans", 3),
            ]
        );
        resultsLocs[ballotCPassed2.ID] = (
            "Ballot C Passed",
            [
                "Hungary calls for revision of the Treaty of Trianon with British mediation",
                StringLineFormatter.Indent ("Whether the UK will support Hungary depends on how much Hungary subscribes to democratic ideals", 1),
            ]
        );
        resultsLocs[oneAntisemitism2.ID] = (
            "One Anti-Semitism",
            [
                "UK reluctantly supports Hungary's claims",
                StringLineFormatter.Indent ("Hungary gains Maramures", 1),
                StringLineFormatter.Indent ("This is only possible because of Romania's submission to Germany", 2),
                "Hungary has made an enemy of Germany",
                StringLineFormatter.Indent ("Germany immediately invades Hungary once at war with the USSR", 1),
                StringLineFormatter.Indent ("Hungary remains independent after the war", 2),
                StringLineFormatter.Indent ("Hungary retains its gains", 3),
                StringLineFormatter.Indent ("This is dependent on total neutrality, as with Austria", 3),
            ]
        );
        resultsLocs[twoGreaterAntisemitism.ID] = (
            "Two or Greater Anti-Semitism",
            [
                "UK refuses to support Hungary's claims",
                StringLineFormatter.Indent ("This is met with great anger", 1),
                StringLineFormatter.Indent ("Hungary attempts to realign behind Germany", 1),
                StringLineFormatter.Indent ("However, Germany doesn't trust Hungary", 2),
                "Hungary has made an enemy of both Germany and the UK",
                StringLineFormatter.Indent ("Germany immediately annexes Hungary once at war with the USSR", 1),
                StringLineFormatter.Indent ("Hungary becomes a Soviet puppet after the war", 2),
                StringLineFormatter.Indent ("Hungary loses Transtisza and some Czechoslovak border villages", 3),
            ]
        );
        resultsLocs[ballotCFailed2.ID] = (
            "Ballot C Failed",
            [
                "Hungary refuses to call for revision of the Treaty of Trianon",
                StringLineFormatter.Indent ("This is somewhat consistent with Hungary's shift to the left, but is still very disappointing", 1),
                "Hungary has made an enemy of Germany",
                StringLineFormatter.Indent ("Germany immediately invades Hungary once at war with the USSR", 1),
                StringLineFormatter.Indent ("Hungary remains independent after the war", 2),
                StringLineFormatter.Indent ("Hungary is rewarded with parts of Crisana and Maramures for the efforts of her partisans", 3),
            ]
        );
        resultsLocs[incidentCFailed.ID] = (
            "Incident C Failed",
            [
                "Hungary attempts to maintain a totally non-aligned path",
                StringLineFormatter.Indent ("Hungary can't find any support for its ailing economy or irredentist claims", 1),
                StringLineFormatter.Indent ("This causes massive unrest", 2),
                StringLineFormatter.Indent ("However, once war breaks out, the people begin to calm down and worry instead about an invasion of Hungary", 3),
                StringLineFormatter.Indent ("Germany eventually invades Hungary once at war with the USSR", 4),
                "Hungary remains independent after the war",
                StringLineFormatter.Indent ("There are no punitive border changes", 1),
                StringLineFormatter.Indent ("This is dependent on total neutrality, as with Austria", 1),
            ]
        );

        History history = new (
            [ballotA.ID, incidentA.ID, ballotB.ID, incidentB.ID, ballotC.ID],
            []
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
                "Hungary",
                "Elective Dictatorship",
                [
                    "Hungary gained true independence after the Great War",
                    StringLineFormatter.Indent ("However, Hungary was punished through the Treaty of Trianon for its participation in the war", 1),
                    StringLineFormatter.Indent ("Hungary was forced to accept severe army restrictions and cede more than two-thirds of her land", 2),
                    StringLineFormatter.Indent ("Over three million Hungarians now live in foreign states", 3),
                    StringLineFormatter.Indent ("Post-independence period has been extremely unstable", 1),
                    StringLineFormatter.Indent ("There has been a communist revolution, two attempts to restore the Habsburgs (one of which was a coup), and a major economic crisis", 2),
                ],
                "22 December 1921",
                "Signing of the Bethlen-Peyer Pact",
                "The Horthy Regency",
                rolesLocs,
                "Speaker",
                (Localisation.UNUSED, Localisation.UNUSED),
                [],
                ("Party", "Parties"),
                partiesLocs,
                abbreviations,
                currencies,
                procedures,
                ballotsLocs,
                resultsLocs
            )
        );
    }
}
