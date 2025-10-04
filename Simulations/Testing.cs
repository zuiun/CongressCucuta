using congress_cucuta.Data;

namespace congress_cucuta.Simulations;

internal class Testing {
    public Simulation Colombia { get; }

    public Testing () {
#region Colombia
        Dictionary<Role, Permissions> rolesPermissions = [];
        Role member = new (Role.MEMBER, "Deputy", "Deputies");
        Role president = new (Role.HEAD_STATE, "Delegate", "Delegates");
        Permissions normal = new (canVote: true);
        Permissions cantVoteCanVeto = new (canVote: false, votes: 0, canVeto: true);
        rolesPermissions[member] = normal;
        rolesPermissions[president] = cantVoteCanVeto;

        Dictionary<Currency, sbyte> currenciesValues = [];
        Currency prosperityCundinamarca = new (0, "Prosperity");
        Currency prosperityVenezuela = new (1, "Prosperity");
        Currency prosperityQuito = new (2, "Prosperity");
        currenciesValues[prosperityCundinamarca] = 1;
        currenciesValues[prosperityVenezuela] = 1;
        currenciesValues[prosperityQuito] = 1;

        Region cundinamarca = new (0, "Cundinamarca", ["#.Largest department", "#.Seeks centralisation"]);
        Region venezuela = new (1, "Venezuela", ["#.Wealthiest department", "#.Seeks federalisation"]);
        Region quito = new (2, "Quito", ["#.Smallest department", "#.Completely marginalised in national politics"]);
        List <Region> regions = [cundinamarca, venezuela, quito];

        ProcedureImmediate presidentialElection = new (
            0,
            "Presidential Election",
            "This country has a powerful, elected executive who is separate from the legislature.",
            [new (Procedure.Effect.ActionType.ElectionNominated, [Role.HEAD_STATE])]
        );
        ProcedureImmediate constitutionCucuta = new (
            1,
            "Constitution of Cucuta",
            "Colombia was founded on the principles of federalism, but today departmental officials have few prerogatives and Colombia is essentially a unitary state.",
            [new (Procedure.Effect.ActionType.ElectionNominated, [Role.HEAD_STATE])]
        );
        List<ProcedureImmediate> proceduresGovernmental = [presidentialElection, constitutionCucuta];

        ProcedureTargeted liberalReforms = new (
            2,
            "Liberal Reforms",
            "Colombia has adopted liberal economic reforms to stimulate the economy, but not every department benefits from these reforms.",
            [
                new (Procedure.Effect.ActionType.CurrencyAdd, [0, 1], 1),
                new (Procedure.Effect.ActionType.CurrencySubtract, [2], 1),
            ]
        );
        ProcedureTargeted bolivarianism = new (
            3,
            "Bolivarianism",
            "The ideals of Simón Bolívar, which most prominently include the liberation and unification of all of Latin America under a massive federation, are extremely popular in Colombia.",
            [new (Procedure.Effect.ActionType.VotePassAdd, [1, 2], 1)]
        );
        ProcedureTargeted repressedFederalism = new (
            4,
            "Repressed Federalism",
            "Colombia was originally established as a federation, but the power of the departments has been suppressed in order to wage the war against Spain; this is an extremely unpopular measure in Venezuela, which sees itself as dominated by Cundinamarca.",
            [
                new (Procedure.Effect.ActionType.VotePassTwoThirds, [0]),
                new (Procedure.Effect.ActionType.VotePassAdd, [3], 1),
            ]
        );
        List<ProcedureTargeted> proceduresSpecial = [liberalReforms, bolivarianism, repressedFederalism];

        Ballot ballotA = new (
            0,
            "Ballot A",
            "Divide the Departments",
            [
                "#.This will divide Gran Colombia into twelve departments, suppressing regional tendencies",
                "##.This will abolish the current departments",
                "##.Each of the twelve departments will answer directly to the President",
                "##.This will improve the war effort against Spain, allowing the war to end more quickly",
                "#.This arrangement is unacceptable to Venezuela and will be contested once Spain is defeated",
            ],
            new Ballot.Result (
                [],
                [
                    "With greater control over the departments, the President is able to better allocate resources without the interference of middle-men",
                    "#.Venezuelans agitate for the restoration of the old federalist system as soon as is expedient",
                ],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Remove,
                        Ballot.Effect.TargetType.Procedure,
                        4
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Add,
                        Ballot.Effect.TargetType.Currency,
                        2,
                        value: 2
                    ),
                ],
                [
                    "Federalism is preserved",
                    "#.Venezuelans are satisfied with their political position",
                    "#.Quitonians expect that civilian rule will come once the Spaniards are fully driven out of Quito",
                ],
                [new (new AlwaysCondition (), 1)]
            )
        );
        Ballot ballotB = new (
            1,
            "Ballot B",
            "Intervene in Peru",
            [
                "#.This will lengthen the war against Spain",
                "#.In order to properly fund an extended campaign, Quitonian and Venezuelan resources will need to be exploited",
                "##.Quito will also be brought under direct military rule in order to better manage the frontline",
            ],
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Remove,
                        Ballot.Effect.TargetType.Currency,
                        1,
                        value: 2
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Remove,
                        Ballot.Effect.TargetType.Currency,
                        2,
                        value: 2
                    ),
                ],
                ["Peru is liberated, but Venezuela and Quito are practically emptied to do so"],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [],
                ["Intentionally left blank"],
                [new (new AlwaysCondition (), 3)]
            )
        );
        Ballot incidentA = new (
            2,
            "Incident A",
            "Intervene in Upper Peru",
            [
                "#.Peru has largely been liberated, but Upper Peru is still fighting for independence",
                "#.This will lengthen the war against Spain",
                "#.With funding running low, all departments will need to make significant sacrifices",
                "##.This includes appropriating church lands",
            ],
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Remove,
                        Ballot.Effect.TargetType.Currency,
                        0,
                        value: 2
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Remove,
                        Ballot.Effect.TargetType.Currency,
                        1,
                        value: 2
                    ),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Remove,
                        Ballot.Effect.TargetType.Currency,
                        2,
                        value: 2
                    ),
                ],
                [
                    "Upper Peru is liberated and named Bolivia",
                    "#.Venezuela and Quito are extremely angry at the usage of national resources for the purposes of ego",
                ],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [],
                ["Intentionally left blank"],
                [new (new AlwaysCondition (), 3)]
            )
        );
        Ballot ballotC = new (
            3,
            "Incident B",
            "Adopt a federalist constitution",
            [
                "#.Now that Spain has been defeated, Venezuela wants Colombia to be a permanent federation",
                "##.Venezuela and Quito are extremely angry at being dragged into foreign wars",
                "##.Venezuela relies heavily on trade and doesn't want any further disruptions",
                "##.Quito wants to revert to civilian control",
            ],
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.Remove,
                        Ballot.Effect.TargetType.Procedure,
                        4
                    ),
                ],
                [
                    "Venezuelans are happy with the government fulfilling its promises",
                    "Quitonians are happy with civilian rule",
                ],
                []
            ),
            new Ballot.Result (
                [],
                [
                    "Venezuelans are infuriated with the government for breaking its promises",
                    "Quitonians are starting to organise against the government for neglecting their needs",
                    "#.Separatism is starting to rise",
                ],
                []
            )
        );
        List<Ballot> ballots = [ballotA, ballotB, incidentA, ballotC];

        Result resultStart = new (
            0,
            "End Results",
            ["Intentionally left blank"],
            [
                new (new ProcedureActiveCondition (in repressedFederalism, true), 1),
                new (new ProcedureActiveCondition (in repressedFederalism, false), 2),
            ]
        );
        Result repressedFederalismActive = new (
            1,
            "Repressed Federalism",
            [
                "Quitonians begin petitioning the administration for change",
                "#.Some want federalism, some want centralism, and some want independence",
                "#.In any case, tensions are high in Quito",
                "Venezuela is infuriated with broken promises",
                "#.While popular opinion is not currently for independence, a few generals and politicians are preparing to revolt",
                "Colombia’s future seems very grim",
            ],
            []
        );
        Result repressedFederalismInactive = new (
            2,
            "No Repressed Federalism",
            [
                "Colombia celebrates its victory over Spain as a united state",
                "#.However, the state of the economy is very worrying to many",
            ],
            [
                new (new CurrencyValueCondition (in prosperityQuito, Condition.ComparisonType.GreaterThanOrEqual, 0), 3),
                new (new CurrencyValueCondition (in prosperityQuito, Condition.ComparisonType.FewerThan, 0), 4),
            ]
        );
        Result quitoGreaterEqualProsperity = new (
            3,
            "Zero or Greater Prosperity",
            [
                "Quito may not be an economic powerhouse, but it has survived",
                "For now, Quito is satisfied with the current state of affairs",
                "#.However, if Quito is marginalised again in the future, it may demand independence",
            ],
            [
                new (new CurrencyValueCondition (in prosperityVenezuela, Condition.ComparisonType.GreaterThanOrEqual, 0), 5),
                new (new CurrencyValueCondition (in prosperityVenezuela, Condition.ComparisonType.FewerThan, 0), 6),
            ]
        );
        Result quitoLessProsperity = new (
            4,
            "Less than Zero Prosperity",
            [
                "Quito is devastated by exploitation",
                "#.Appropriation of church lands was especially unpopular",
                "#.Quitonians understand that they are far too weak to win independence, but separatism is spreading",
            ],
            [
                new (new CurrencyValueCondition (in prosperityVenezuela, Condition.ComparisonType.GreaterThanOrEqual, 0), 5),
                new (new CurrencyValueCondition (in prosperityVenezuela, Condition.ComparisonType.FewerThan, 0), 6),
            ]
        );
        Result venezuelaGreaterEqualProsperity = new (
            5,
            "Zero or Greater Prosperity",
            [
                "Venezuela remains a powerful partner within Colombia",
                "#.It seems that the dream of a powerful federation in Latin America will come true",
                "#.Venezuela is satisfied with the current state of affairs",
                "##.Without Venezuelan support, no Quitonian separatist movement would dare revolt",
            ],
            []
        );
        Result venezuelaLessProsperity = new (
            6,
            "Less than Zero Prosperity",
            [
                "Venezuela feels cheated by the war",
                "#.There was practically nothing to gain by participating and Venezuela suffered for it",
                "##.Venezuela considers itself subjugated by Cundinamarca",
                "#.While popular opinion is not currently for independence, some generals and politicians are preparing to revolt",
                "#.If Venezuelan grievances are not addressed soon, Colombia will fall apart",
                "##.Quitonian separatists can count on Venezuelan support if the situation doesn’t improve",
            ],
            []
        );
        List<Result> results = [
            resultStart,
            repressedFederalismActive,
            repressedFederalismInactive,
            quitoGreaterEqualProsperity,
            quitoLessProsperity,
            venezuelaGreaterEqualProsperity,
            venezuelaLessProsperity,
        ];

        Colombia = new (
            new History (
                [0, 1, 2],
                []
            ),
            rolesPermissions,
            currenciesValues,
            regions,
            [],
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
                    "#.While Spain was under French occupation, its Latin American colonies revolted for independence",
                    "After multiple failed attempts, Simón Bolívar finally liberated New Granada and Venezuela",
                    "#.Newly freed colonies united into what is now known as Great Colombia",
                    "#.He later conquered Quito",
                    "##.He wanted to unite all of Latin America under Great Colombia"
                ],
                "13 July 1822",
                "Annexation of Guayaquil",
                "The Campaigns of the South",
                ("Deputy", "Deputies"),
                "Congress President (Chairman)",
                ("Department", "Departments"),
                ("UNUSED", "UNUSED")
            )
        );
#endregion


        // TODO: Something else
    }
}
