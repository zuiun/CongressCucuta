using congress_cucuta.Converters;
using congress_cucuta.Data;

namespace congress_cucuta.Simulations;

internal class Colombia {
    public Simulation Simulation { get; }

    public Colombia () {
        Dictionary<IDType, Permissions> rolesPermissions = [];
        IDType deputy = Role.MEMBER;
        IDType president = Role.HEAD_STATE;
        Permissions normal = new (canVote: true);
        Permissions cantVoteCanVeto = new (canVote: false, votes: 0, canVeto: true);
        rolesPermissions[deputy] = normal;
        rolesPermissions[president] = cantVoteCanVeto;
        Dictionary<IDType, (string, string)> roles = [];
        roles[deputy] = ("Deputy", "Deputies");
        roles[president] = ("President", "Presidents");

        Faction cundinamarca = new (0);
        Faction venezuela = new (1);
        Faction quito = new (2);
        List<Faction> regions = [cundinamarca, venezuela, quito];
        Dictionary<IDType, (string, string[], string)> regionsLocs = [];
        regionsLocs[cundinamarca.ID] = (
            "Cundinamarca",
            [
                StringLineFormatter.Indent ("Largest department", 1),
                StringLineFormatter.Indent ("Seeks centralisation", 1),
            ],
            Localisation.UNUSED
        );
        regionsLocs[venezuela.ID] = (
            "Venezuela",
            [
                StringLineFormatter.Indent ("Wealthiest department", 1),
                StringLineFormatter.Indent ("Seeks federalisation", 1),
            ],
            Localisation.UNUSED
        );
        regionsLocs[quito.ID] = (
            "Quito",
            [
                StringLineFormatter.Indent ("Smallest department", 1),
                StringLineFormatter.Indent ("Completely marginalised in national politics", 1),
            ],
            Localisation.UNUSED
        );

        Dictionary<IDType, sbyte> currenciesValues = [];
        IDType prosperityCundinamarca = 0;
        IDType prosperityVenezuela = 1;
        IDType prosperityQuito = 2;
        currenciesValues[prosperityCundinamarca] = 1;
        currenciesValues[prosperityVenezuela] = 1;
        currenciesValues[prosperityQuito] = 1;
        Dictionary<IDType, string> currencies = [];
        currencies[prosperityCundinamarca] = "Prosperity";
        currencies[prosperityVenezuela] = "Prosperity";
        currencies[prosperityQuito] = "Prosperity";
        currencies[Currency.REGION] = "Prosperity";

        ProcedureImmediate presidentialElection = new (
            0,
            [new (Procedure.Effect.ActionType.ElectionNominated, [Role.HEAD_STATE])]
        );
        ProcedureImmediate constitutionCucuta = new (
            1,
            [new (Procedure.Effect.ActionType.ElectionRegion, [])]
        );
        List<ProcedureImmediate> proceduresGovernmental = [presidentialElection, constitutionCucuta];
        ProcedureTargeted liberalReforms = new (
            2,
            [
                new (Procedure.Effect.ActionType.CurrencyInitialise, []),
                new (Procedure.Effect.ActionType.CurrencyAdd, [prosperityCundinamarca, prosperityVenezuela], 1),
                new (Procedure.Effect.ActionType.CurrencySubtract, [prosperityQuito], 1),
            ],
            []
        );
        ProcedureTargeted bolivarianism = new (
            3,
            [new (Procedure.Effect.ActionType.VotePassAdd, [], 1)],
            [1, 2]
        );
        ProcedureTargeted repressedFederalism = new (
            4,
            [new (Procedure.Effect.ActionType.VotePassAdd, [], 1)],
            [3]
        );
        List<ProcedureTargeted> proceduresSpecial = [liberalReforms, bolivarianism, repressedFederalism];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[presidentialElection.ID] = (
            "Presidential Election",
            "This country has a powerful, elected executive who is separate from the legislature."
        );
        procedures[constitutionCucuta.ID] = (
            "Constitution of Cucuta",
            "Despite the erosion of departmental powers, Colombia remains a federation in name and in vision. It will be very difficult to change this state of affairs through constitutional means."
        );
        procedures[liberalReforms.ID] = (
            "Liberal Reforms",
            "Colombia has adopted liberal economic reforms to stimulate the economy, but not every department benefits from these reforms."
        );
        procedures[bolivarianism.ID] = (
            "Bolivarianism",
            "The ideals of Simón Bolívar, which most prominently include the liberation and unification of all of Latin America under a massive federation, are extremely popular in Colombia."
        );
        procedures[repressedFederalism.ID] = (
            "Repressed Federalism",
            "Colombia was originally established as a federation, but the power of the departments has been suppressed in order to wage the war against Spain; this is an extremely unpopular measure in Venezuela, which sees itself as dominated by Cundinamarca."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.RemoveProcedure,
                        [repressedFederalism.ID]
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [prosperityQuito],
                        2
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
                        [prosperityVenezuela, prosperityQuito],
                        -2
                    ),
                ],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 3)]
            )
        );
        Ballot incidentA = new (
            2,
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ModifyCurrency,
                        [Currency.REGION],
                        -2
                    ),
                ],
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
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.RemoveProcedure,
                        [repressedFederalism.ID]
                    ),
                ],
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
            "Divide the departments",
            [
                StringLineFormatter.Indent ("This will divide Gran Colombia into twelve departments, suppressing regional tendencies", 1),
                StringLineFormatter.Indent ("This will abolish the current departments", 2),
                StringLineFormatter.Indent ("Each of the twelve departments will answer directly to the President", 2),
                StringLineFormatter.Indent ("This will improve the war effort against Spain, allowing the war to end more quickly", 2),
                StringLineFormatter.Indent ("This arrangement is unacceptable to Venezuela and will be contested once Spain is defeated", 1),
            ],
            [
                "With greater control over the departments, the President is able to better allocate resources without the interference of middle-men",
                StringLineFormatter.Indent ("Venezuelans agitate for the restoration of the old federalist system as soon as is expedient", 1),
            ],
            [
                "Federalism is preserved",
                StringLineFormatter.Indent ("Venezuelans are satisfied with their political position", 1),
                StringLineFormatter.Indent ("Quitonians expect their own concessions once the Spaniards are fully driven out", 1),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Intervene in Peru",
            [
                StringLineFormatter.Indent ("This will lengthen the war against Spain", 1),
                StringLineFormatter.Indent ("In order to properly fund an extended campaign, Quitonian and Venezuelan resources will need to be exploited", 1),
                StringLineFormatter.Indent ("Quito will also be brought under direct military rule in order to better manage the frontline", 2),
            ],
            ["Peru is liberated, but Venezuela and Quito are practically emptied to do so"],
            ["Intentionally left blank"]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Intervene in Upper Peru",
            [
                StringLineFormatter.Indent ("Peru has largely been liberated, but Upper Peru is still fighting for independence", 1),
                StringLineFormatter.Indent ("This will lengthen the war against Spain", 1),
                StringLineFormatter.Indent ("With funding running low, all departments will need to make significant sacrifices", 1),
                StringLineFormatter.Indent ("This includes appropriating church lands", 2),
            ],
            [
                "Upper Peru is liberated and named Bolivia",
                StringLineFormatter.Indent ("Venezuela and Quito are extremely angry at the usage of national resources for the purposes of ego", 1),
            ],
            ["Intentionally left blank"]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Adopt a federalist constitution",
            [
                StringLineFormatter.Indent ("Now that Spain has been defeated, Venezuela wants Colombia to be a permanent federation", 1),
                StringLineFormatter.Indent ("Venezuela and Quito are extremely angry at being dragged into foreign wars", 2),
                StringLineFormatter.Indent ("Venezuela relies heavily on trade and doesn't want any further disruptions", 2),
                StringLineFormatter.Indent ("Quito wants to revert to civilian control", 2),
            ],
            [
                "Venezuelans are happy with the government fulfilling its promises",
                "Quitonians are happy with civilian rule",
            ],
            [
                "Venezuelans are infuriated with the government for breaking its promises",
                "Quitonians are starting to organise against the government for neglecting their needs",
                StringLineFormatter.Indent ("Separatism is starting to rise", 1),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new ProcedureActiveCondition (repressedFederalism.ID, false), 1),
                new (new ProcedureActiveCondition (repressedFederalism.ID, true), 2),
            ]
        );
        Result repressedFederalismInactive = new (
            1,
            [
                new (new CurrencyValueCondition (prosperityQuito, ICondition.ComparisonType.FewerThan, 0), 3),
                new (new CurrencyValueCondition (prosperityQuito, ICondition.ComparisonType.GreaterThanOrEqual, 0), 4),
            ]
        );
        Result repressedFederalismActive = new (
            2,
            []
        );
        Result quitoLessProsperity = new (
            3,
            [
                new (new CurrencyValueCondition (prosperityVenezuela, ICondition.ComparisonType.FewerThan, 0), 5),
                new (new CurrencyValueCondition (prosperityVenezuela, ICondition.ComparisonType.GreaterThanOrEqual, 0), 6),
            ]
        );
        Result quitoGreaterEqualProsperity = new (
            4,
            [
                new (new CurrencyValueCondition (prosperityVenezuela, ICondition.ComparisonType.FewerThan, 0), 5),
                new (new CurrencyValueCondition (prosperityVenezuela, ICondition.ComparisonType.GreaterThanOrEqual, 0), 6),
            ]
        );
        Result venezuelaLessProsperity = new (
            5,
            []
        );
        Result venezuelaGreaterEqualProsperity = new (
            6,
            []
        );
        List<Result> results = [
            resultStart,
            repressedFederalismInactive,
            repressedFederalismActive,
            quitoLessProsperity,
            quitoGreaterEqualProsperity,
            venezuelaLessProsperity,
            venezuelaGreaterEqualProsperity,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[repressedFederalismInactive.ID] = (
            "No Repressed Federalism",
            [
                "Colombia celebrates its victory over Spain as a united state",
                StringLineFormatter.Indent ("However, the state of the economy is very worrying to many", 1),
            ]
        );
        resultsLocs[repressedFederalismActive.ID] = (
            "Repressed Federalism",
            [
                "Quitonians begin petitioning the administration for change",
                StringLineFormatter.Indent ("Some want federalism, some want centralism, and some want independence", 1),
                StringLineFormatter.Indent ("In any case, tensions are high in Quito", 1),
                "Venezuela is infuriated with broken promises",
                StringLineFormatter.Indent ("While popular opinion is not currently for independence, a few generals and politicians are preparing to revolt", 1),
                "Colombia’s future seems very grim",
            ]
        );
        resultsLocs[quitoLessProsperity.ID] = (
            "Less than Zero Prosperity",
            [
                "Quito is devastated by exploitation",
                StringLineFormatter.Indent ("Appropriation of church lands was especially unpopular", 1),
                StringLineFormatter.Indent ("Quitonians understand that they are far too weak to win independence, but separatism is spreading", 1),
            ]
        );
        resultsLocs[quitoGreaterEqualProsperity.ID] = (
            "Zero or Greater Prosperity",
            [
                "Quito may not be an economic powerhouse, but it has survived",
                "For now, Quito is satisfied with the current state of affairs",
                StringLineFormatter.Indent ("However, if Quito is marginalised again in the future, it may demand independence", 1),
            ]
        );
        resultsLocs[venezuelaLessProsperity.ID] = (
            "Less than Zero Prosperity",
            [
                "Venezuela feels cheated by the war",
                StringLineFormatter.Indent ("There was practically nothing to gain by participating and Venezuela suffered for it", 1),
                StringLineFormatter.Indent ("Venezuela considers itself subjugated by Cundinamarca", 2),
                StringLineFormatter.Indent ("While popular opinion is not currently for independence, some generals and politicians are preparing to revolt", 1),
                StringLineFormatter.Indent ("If Venezuelan grievances are not addressed soon, Colombia will fall apart", 1),
                StringLineFormatter.Indent ("Quitonian separatists can count on Venezuelan support if the situation doesn’t improve", 2),
            ]
        );
        resultsLocs[venezuelaGreaterEqualProsperity.ID] = (
            "Zero or Greater Prosperity",
            [
                "Venezuela remains a powerful partner within Colombia",
                StringLineFormatter.Indent ("It seems that the dream of a powerful federation in Latin America will come true", 1),
                StringLineFormatter.Indent ("Venezuela is satisfied with the current state of affairs", 1),
                StringLineFormatter.Indent ("Without Venezuelan support, no Quitonian separatist movement would dare revolt", 2),
            ]
        );

        Simulation = new (
            new History (
                [ballotA.ID, ballotB.ID, incidentA.ID],
                []
            ),
            rolesPermissions,
            regions,
            [],
            currenciesValues,
            proceduresGovernmental,
            proceduresSpecial,
            [],
            ballots,
            results,
            new Localisation (
                "Colombia",
                "Presidential Republic",
                [
                    "New Granada was a Spanish viceroyalty",
                    StringLineFormatter.Indent ("While Spain was under French occupation, its Latin American colonies revolted for independence", 1),
                    "After multiple failed attempts, Simón Bolívar finally liberated New Granada and Venezuela",
                    StringLineFormatter.Indent ("Newly freed colonies united into what is now known as Great Colombia", 1),
                    StringLineFormatter.Indent ("He later conquered Quito", 1),
                    StringLineFormatter.Indent ("He wanted to unite all of Latin America under Great Colombia", 2),
                ],
                "13 July 1822",
                "Annexation of Guayaquil",
                "The Campaigns of the South",
                roles,
                "Congress President (Chairman)",
                ("Department", "Departments"),
                regionsLocs,
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
