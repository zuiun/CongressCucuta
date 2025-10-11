using congress_cucuta.Converters;
using congress_cucuta.Data;

namespace congress_cucuta.Simulations;

internal class Canada : ISimulation {
    public Simulation Simulation { get; }

    public Canada () {
        IDType member = Role.MEMBER;
        IDType primeMinister = Role.HEAD_GOVERNMENT;
        IDType governorGeneral = Role.HEAD_STATE;
        List<IDType> roles = [
            member,
            primeMinister,
            governorGeneral,
            Role.LEADER_REGION,
            Role.LEADER_PARTY,
            0,
            1,
            2,
            3,
            4,
            5,
            6,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[member] = ("Member", "Members");
        rolesLocs[primeMinister] = ("Prime Minister", "Prime Ministers");
        rolesLocs[governorGeneral] = ("Governor General", "Governors General");
        rolesLocs[0] = ("Premier of Ontario", "Premiers of Ontario");
        rolesLocs[1] = ("Premier of Quebec", "Premiers of Quebec");
        rolesLocs[2] = ("Premier of Western Canada", "Premiers of Western Canada");
        rolesLocs[3] = ("Party Leader of LPC", "Party Leaders of LPC");
        rolesLocs[4] = ("Party Leader of PC", "Party Leaders of PC");
        rolesLocs[5] = ("Party Leader of RC", "Party Leaders of RC");
        rolesLocs[6] = ("Party Leader of NDP", "Party Leaders of NDP");
        rolesLocs[Role.LEADER_REGION] = ("Premier", "Premiers");
        rolesLocs[Role.LEADER_PARTY] = ("Party Leader", "Party Leaders");

        Faction ontario = new (0);
        Faction quebec = new (1);
        Faction westernCanada = new (2);
        List<Faction> provinces = [ontario, quebec, westernCanada];
        Dictionary<IDType, (string, string[], string)> provincesLocs = [];
        provincesLocs[ontario.ID] = (
            "Ontario",
            [
                StringLineFormatter.Indent ("Predominantly Anglophone", 1),
                StringLineFormatter.Indent ("Split between liberals and conservatives", 1),
                StringLineFormatter.Indent ("Largely dominates Anglophone politics", 1),
            ],
            "Premier"
        );
        provincesLocs[quebec.ID] = (
            "Quebec",
            [
                StringLineFormatter.Indent ("Predominantly Francophone", 1),
                StringLineFormatter.Indent ("Liberal and social creditist stronghold", 1),
                StringLineFormatter.Indent ("Seeks wide-ranging provincial powers", 1),
            ],
            "Premier"
        );
        provincesLocs[westernCanada.ID] = (
            "Western Canada",
            [
                StringLineFormatter.Indent ("Predominantly Anglophone, but with large Chinese and First Nations populations", 1),
                StringLineFormatter.Indent ("Generally conservative with some social creditists", 1),
                StringLineFormatter.Indent ("Rapidly growing population and economy", 1),
                StringLineFormatter.Indent ("Has significantly less political representation than the eastern provinces", 2),
            ],
            "Premier"
        );

        Faction lpc = new (3);
        Faction pc = new (4);
        Faction rc = new (5);
        Faction ndp = new (6, false);
        List<Faction> parties = [lpc, pc, rc, ndp];
        Dictionary<IDType, (string, string[], string)> partiesLocs = [];
        partiesLocs[lpc.ID] = (
            "Liberal Party of Canada",
            [
                StringLineFormatter.Indent ("Economically and socially centre-left", 1),
                StringLineFormatter.Indent ("Supports a strong federal government", 1),
            ],
            "Party Leader"
        );
        partiesLocs[pc.ID] = (
            "Progressive Conservative Party of Canada",
            [
                StringLineFormatter.Indent ("Economically and socially centre-right", 1),
                StringLineFormatter.Indent ("Supports increasing provincial rights", 1),
            ],
            "Party Leader"
        );
        partiesLocs[rc.ID] = (
            "Social Credit Rally",
            [
                StringLineFormatter.Indent ("Economically populist, socially right", 1),
                StringLineFormatter.Indent ("Only federal supporter of Quebec sovereignty", 1),
            ],
            "Party Leader"
        );
        partiesLocs[ndp.ID] = (
            "New Democratic Party",
            [],
            "Party Leader"
        );
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[lpc.ID] = "LPC";
        abbreviations[pc.ID] = "PC";
        abbreviations[rc.ID] = "RC";
        abbreviations[ndp.ID] = "NDP";

        Dictionary<IDType, sbyte> currenciesValues = [];
        currenciesValues[Currency.STATE] = 0;
        Dictionary<IDType, string> currencies = [];
        currencies[Currency.STATE] = "Tension";

        ProcedureImmediate commonwealthRealm = new (
            0,
            [
                new (Procedure.Effect.ActionType.ElectionAppointed, [governorGeneral]),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [governorGeneral], 0),
            ]
        );
        ProcedureImmediate federalProvincialConferences = new (
            1,
            [
                new (Procedure.Effect.ActionType.ElectionRegion, [governorGeneral]),
                new (Procedure.Effect.ActionType.PermissionsVotes, [Role.LEADER_REGION], 1),
            ]
        );
        ProcedureImmediate generalElection = new (
            2,
            [
                new (Procedure.Effect.ActionType.ElectionParty, [governorGeneral]),
                new (Procedure.Effect.ActionType.ElectionNominated, [primeMinister, governorGeneral, Role.LEADER_REGION]),
            ]
        );
        List<ProcedureImmediate> proceduresGovernmental = [commonwealthRealm, federalProvincialConferences, generalElection];
        ProcedureTargeted greatFlagDebate = new (
            3,
            [
                new (Procedure.Effect.ActionType.CurrencyInitialise, []),
                new (Procedure.Effect.ActionType.ProcedureActivate, [generalElection.ID]),
            ],
            []
        );
        ProcedureTargeted trudeaumania = new (
            4,
            [new (Procedure.Effect.ActionType.PermissionsVotes, [primeMinister], 1)],
            [0, 1]
        );
        ProcedureTargeted coffeeTableRevolutionaries = new (
            5,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [], 1)],
            []
        );
        ProcedureTargeted newFederalism = new (
            6,
            [new (Procedure.Effect.ActionType.CurrencySubtract, [], 1)],
            []
        );
        ProcedureTargeted justWatchMe = new (
            7,
            [new (Procedure.Effect.ActionType.PermissionsVotes, [quebec.ID, primeMinister], 1)],
            [],
            false
        );
        ProcedureTargeted weWillVanquish = new (
            8,
            [
                new (Procedure.Effect.ActionType.CurrencyAdd, [], 1),
                new (Procedure.Effect.ActionType.PermissionsCanVote, [], 0),
                new (Procedure.Effect.ActionType.PermissionsCanSpeak, [], 0),
            ],
            [],
            false
        );
        ProcedureTargeted westernAlienation = new (
            9,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [], 1)],
            [],
            false
        );
        ProcedureTargeted abandonedPetroCanada = new (
            10,
            [new (Procedure.Effect.ActionType.PermissionsVotes, [westernCanada.ID], 1)],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            greatFlagDebate,
            trudeaumania,
            coffeeTableRevolutionaries,
            newFederalism,
            justWatchMe,
            weWillVanquish,
            westernAlienation,
            abandonedPetroCanada
        ];
        ProcedureDeclared royalVeto = new (
            11,
            [
                new (Procedure.Effect.ActionType.BallotFail, [])
            ],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [governorGeneral]
        );
        ProcedureDeclared voteNoConfidence = new (
            12,
            [new (Procedure.Effect.ActionType.ElectionNominated, [primeMinister, governorGeneral, Role.LEADER_REGION])],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.DivisionChamber),
            0,
            []
        );
        ProcedureDeclared viceregalAppointment = new (
            13,
            [new (Procedure.Effect.ActionType.ElectionAppointed, [governorGeneral, primeMinister, Role.LEADER_PARTY, Role.LEADER_REGION])],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.Always),
            0,
            [primeMinister]
        );
        List<ProcedureDeclared> proceduresDeclared = [royalVeto, voteNoConfidence, viceregalAppointment];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[commonwealthRealm.ID] = (
            "Commonwealth Realm",
            "This country's head of state is chosen based on familial lineage, but exercises his prerogatives through a representative."
        );
        procedures[federalProvincialConferences.ID] = (
            "Federal-Provincial Conferences",
            "Canada is a federation with a long tradition of inter-governmental enmity. In order to better coordinate policy, the government semi-regularly holds meetings between the Prime Minister and the Premiers. These are often used by the provinces to pressure the federal government into making concessions."
        );
        procedures[generalElection.ID] = (
            "General Election",
            "This country has a legislature with regular elections, along with an empowered head of government."
        );
        procedures[greatFlagDebate.ID] = (
            "Great Flag Debate",
            "The Maple Leaf is like no other flag in the world, but its adoption, marred with ugly rhetoric, was symbolic of the conflict between the Anglophone and Francophone communities."
        );
        procedures[trudeaumania.ID] = (
            "Trudeaumania",
            "The current Prime Minister is unusually young, which has bolstered his popularity among younger voters and could lead to a new current in politics."
        );
        procedures[coffeeTableRevolutionaries.ID] = (
            "Coffee-Table Revolutionaries",
            "Separatists in Quebec dream of blood on the streets, cheering on the terrorist Quebec Liberation Front. The question is, just how much violence is too much for these privileged observers?"
        );
        procedures[newFederalism.ID] = (
            "New Federalism",
            "Provincial power has grown at the expense of federal power over the past decade, but support for a stronger federal government has been increasing as of late."
        );
        procedures[justWatchMe.ID] = (
            "Just Watch Me",
            Localisation.UNUSED
        );
        procedures[weWillVanquish.ID] = (
            "We Will Vanquish",
            Localisation.UNUSED
        );
        procedures[westernAlienation.ID] = (
            "Western Alienation",
            Localisation.UNUSED
        );
        procedures[abandonedPetroCanada.ID] = (
            "Abandoned Petro-Canada",
            Localisation.UNUSED
        );
        procedures[royalVeto.ID] = (
            "Royal Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );
        procedures[voteNoConfidence.ID] = (
            "Vote of No Confidence",
            "This country's head of government is responsible to the legislature, which means that he can be removed if he is seen to be inept."
        );
        procedures[viceregalAppointment.ID] = (
            "Viceregal Appointment",
            "By convention, the Governor General is replaced every five years, alternating between native English- and French-speakers."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.FoundParty, [ndp.ID])],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [Currency.STATE], 1),
                    new Ballot.Effect (Ballot.Effect.ActionType.FoundParty, [ndp.ID]),
                ],
                [new (new AlwaysCondition (), 1)]
            )
        );
        Ballot incidentA = new (
            1,
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ReplaceProcedure,
                        [coffeeTableRevolutionaries.ID, justWatchMe.ID]
                    ),
                ],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.ActionType.DissolveParty, [rc.ID]),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ReplaceProcedure,
                        [coffeeTableRevolutionaries.ID, weWillVanquish.ID]
                    ),
                ],
                [new (new AlwaysCondition (), 2)]
            ),
            true
        );
        Ballot ballotB = new (
            2,
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.ActionType.DissolveParty, [rc.ID]),
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ReplaceProcedure,
                        [trudeaumania.ID, westernAlienation.ID]
                    ),
                ],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (
                        Ballot.Effect.ActionType.ReplaceProcedure,
                        [trudeaumania.ID, abandonedPetroCanada.ID]
                    ),
                ],
                [new (new AlwaysCondition (), 3)]
            )
        );
        Ballot ballotC = new (
            3,
            new Ballot.Result (
                [],
                [
                    new (new BallotPassedCondition (ballotB.ID, true), 4),
                    new (new BallotPassedCondition (ballotB.ID, false), 5),
                ]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [Currency.STATE], -1)],
                [
                    new (new BallotPassedCondition (ballotB.ID, true), 4),
                    new (new BallotPassedCondition (ballotB.ID, false), 5),
                ]
            )
        );
        Ballot incidentB = new (
            4,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [Currency.STATE], 1)],
                [new (new AlwaysCondition (), 5)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 5)]
            ),
            true
        );
        Ballot ballotD = new (
            5,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [Currency.STATE], -1)],
                []
            ),
            new Ballot.Result (
                [],
                []
            )
        );
        List<Ballot> ballots = [ballotA, incidentA, ballotB, ballotC, incidentB, ballotD];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Adopt official bilingualism",
            [
                StringLineFormatter.Indent ("Currently, both English and French can be used in government, but English heavily dominates", 1),
                StringLineFormatter.Indent ("There are French-speaking minorities in several provinces who generally cannot access public services in French", 2),
                StringLineFormatter.Indent ("Quebec is ironically the only province with significant linguistic rights for its English-speaking minority", 2),
                StringLineFormatter.Indent ("This would give English and French equal force in all federal services and legal proceedings", 1),
                StringLineFormatter.Indent ("This means some proportion of public servants must be bilingual to provide services in both languages", 2),
            ],
            [
                "This is very popular in Quebec and unpopular outside of it",
                StringLineFormatter.Indent ("Bilingual education is especially ineffective", 1),
                StringLineFormatter.Indent ("Non-Francophone minorities don’t want to learn two non-native languages", 1),
                StringLineFormatter.Indent ("Still, there is little active Anglophone opposition", 1),
                "In time this may be seen as a unifying measure",
                StringLineFormatter.Indent ("For now, this is just a hurdle to entering public service", 1),
            ],
            [
                "This leads to massive discontent in Quebec, but little reaction elsewhere",
                StringLineFormatter.Indent ("Anglophone community in Quebec is especially fearful that their protections may be repealed without federal protection", 1),
                StringLineFormatter.Indent ("This is seen in Quebec as proof that Quebec does not belong to the Canadian nation", 1),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Dismantle the Quebec Liberation Front",
            [
                StringLineFormatter.Indent ("Quebec Liberation Front (FLQ) is a terrorist, secessionist organisation", 1),
                StringLineFormatter.Indent ("It is especially popular among students", 2),
                StringLineFormatter.Indent ("FLQ has kidnapped two high-ranking ministers and threatened to execute them", 1),
                StringLineFormatter.Indent ("It also claims to be able to mobilise nearly a hundred-thousand armed workers if its demands are not met", 2),
                StringLineFormatter.Indent ("This will invoke the War Measures Act", 1),
                StringLineFormatter.Indent ("This means Quebec will be militarily occupied until the FLQ is no longer an insurrection threat", 2),
                StringLineFormatter.Indent ("Police will have the unlimited right to detain anyone, even without charge", 2),
            ],
            [
                "Violent separatism is eliminated as a political force",
                StringLineFormatter.Indent ("However, this heavy-handed approach will only encourage separatism in the future", 1),
                StringLineFormatter.Indent ("Further attempts to secede will be more moderate", 2),
                StringLineFormatter.Indent ("For now, Canada is united in opposition to terrorism", 2),
            ],
            [
                "Apparent weakness emboldens separatists",
                StringLineFormatter.Indent ("This bodes poorly for national unity", 1),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Establish oil price controls",
            [
                StringLineFormatter.Indent ("Arab oil embargo has severely increased oil prices across Canada", 1),
                StringLineFormatter.Indent ("This benefits Alberta, which is Canada’s largest oil producer", 2),
                StringLineFormatter.Indent ("Price controls would overwhelmingly benefit the eastern provinces, which import almost all of their energy", 2),
                StringLineFormatter.Indent ("Additionally, the federal government would have to pay the difference in prices, which would be funded through taxes", 2),
                StringLineFormatter.Indent ("This could lay the framework for further oil controls", 1),
                StringLineFormatter.Indent ("This would be extremely unpopular across the western provinces, which would no longer benefit from resource production", 2),
            ],
            [
                "Western provinces fear continued government intervention to their detriment",
                StringLineFormatter.Indent ("This destroys the credibility of the social creditists", 1),
                StringLineFormatter.Indent ("This widens the traditional Anglophone-Francophone conflict to an east-west conflict", 1),
            ],
            [
                "Oil prices fall naturally over time",
                StringLineFormatter.Indent ("However, this sets the precedent that the provinces can threaten the federal government with finances", 1),
                StringLineFormatter.Indent ("This is ironically unpopular in Quebec and Ontario", 2),
                StringLineFormatter.Indent ("Eventually, the government gives up all remaining public ownership of oil", 2),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Prohibit a Quebec sovereignty referendum",
            [
                StringLineFormatter.Indent ("Quebec is currently under the control of the Quebec Party (PQ)", 1),
                StringLineFormatter.Indent ("PQ has made a sovereignty referendum central to its agenda", 2),
                StringLineFormatter.Indent ("PQ wants “sovereignty-association”", 2),
                StringLineFormatter.Indent ("This is political independence for Quebec, but economic union with Canada", 3),
                StringLineFormatter.Indent ("This referendum could go either way", 2),
                StringLineFormatter.Indent ("Francophone voters are very divided on independence", 3),
                StringLineFormatter.Indent ("Anglophone voters are overwhelmingly opposed to independence", 3),
                StringLineFormatter.Indent ("This referendum may not be legally binding", 2),
                StringLineFormatter.Indent ("Supreme Court has not made a judgement on it yet", 3),
            ],
            [
                "Referendum goes ahead anyway",
                StringLineFormatter.Indent ("Police allow the referendum to happen", 1),
                StringLineFormatter.Indent ("Anger with federal intervention could result in a “Yes” victory", 1),
                "Federal government refuses to negotiate with the Quebecer government",
                StringLineFormatter.Indent ("Supreme Court declares the referendum non-binding", 1),
                StringLineFormatter.Indent ("Anglophone Quebecers begin to fear for their language rights", 1),
                "This is a very tense situation",
            ],
            [
                "Referendum goes ahead as planned",
                StringLineFormatter.Indent ("Federal government may intervene on behalf of the “No” case", 1),
                StringLineFormatter.Indent ("In addition, it may choose to refuse negotiations if the referendum passes", 2),
                StringLineFormatter.Indent ("Quebec Liberals are poorly organised and haughty", 1),
                StringLineFormatter.Indent ("They are very unlikely to make a good “No” case", 2),
                StringLineFormatter.Indent ("No other federal-level party has enough of a presence in Quebec to affect the outcome", 2),
                StringLineFormatter.Indent ("PQ is putting every effort into the “Yes” case", 1),
                StringLineFormatter.Indent ("However, there are some tensions between male and female Francophone voters", 2),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Adopt the National Energy Program",
            [
                StringLineFormatter.Indent ("National Energy Program:", 1),
                StringLineFormatter.Indent ("Levies a tax on oil producers", 2),
                StringLineFormatter.Indent ("Subsidises oil costs", 2),
                StringLineFormatter.Indent ("Further limits the price of oil below American oil", 2),
                StringLineFormatter.Indent ("Financially incentivises oil production and exploration on federally owned land", 2),
                StringLineFormatter.Indent ("This could improve overall oil production capabilities", 3),
                StringLineFormatter.Indent ("This is obviously extremely unpopular in Alberta", 1),
                StringLineFormatter.Indent ("Should oil prices fall naturally, this program could make oil outright unprofitable to produce", 2),
            ],
            [
                "Alberta is extremely angry with this",
                StringLineFormatter.Indent ("Provincial government threatens to massively decrease oil production to force prices up", 1),
                StringLineFormatter.Indent ("Economic growth slows down further", 1),
                StringLineFormatter.Indent ("There is even some talk of Albertan separatism", 1),
                "This reduces Canada’s reliance on foreign oil",
            ],
            [
                "Alberta is satisfied with the rejection of further controls",
                StringLineFormatter.Indent ("This does come at a cost in the eastern provinces, which see Alberta as greedy and insular", 1),
                StringLineFormatter.Indent ("This irony is not lost on Quebec", 2),
                "This increases Canada’s reliance on foreign oil",
                StringLineFormatter.Indent ("However, falling oil prices may remedy this somewhat", 1),
            ]
        );
        ballotsLocs[ballotD.ID] = (
            "Ballot D",
            "Adopt a charter of rights",
            [
                StringLineFormatter.Indent ("Canada already has a bill of rights", 1),
                StringLineFormatter.Indent ("However, this is just a federal law, rather than a constitutional document", 2),
                StringLineFormatter.Indent ("It doesn’t apply to the provinces", 3),
                StringLineFormatter.Indent ("This will significantly strengthen the federal government", 1),
                StringLineFormatter.Indent ("Supreme Court will be given the authority to overturn all laws, both federal and provincial, that run contrary to the charter", 2),
                StringLineFormatter.Indent ("Notably, this only protects the language rights of Anglophone and Francophone communities", 2),
            ],
            [
                "Unsurprisingly, constitutionally protecting basic rights is very popular",
                StringLineFormatter.Indent ("This is especially poignant given the debate over invoking the War Measures Act in Quebec", 1),
                StringLineFormatter.Indent ("This does lead to concerns of judicial overreach", 1),
                "Provincial opposition leads to the adoption of the Notwithstanding Clause",
                StringLineFormatter.Indent ("This allows provincial and federal legislatures to temporarily override some basic rights", 1),
            ],
            [
                "This is seen as a victory for the provinces",
                StringLineFormatter.Indent ("This is very likely to make any other constitutional changes very difficult without provincial approval", 1),
                StringLineFormatter.Indent ("This could hamper efforts to gain constitutional independence", 2),
                "There is some outrage, but also understanding",
                StringLineFormatter.Indent ("Westminster systems generally have parliamentary supremacy", 1),
                StringLineFormatter.Indent ("Courts normally don’t have strong override powers", 2),
                StringLineFormatter.Indent ("It may have been difficult to get British approval for this change", 1),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new CurrencyValueCondition (Currency.STATE, ICondition.ComparisonType.FewerThanOrEqual, 1), 1),
                new (new AndCondition (
                    new CurrencyValueCondition (Currency.STATE, ICondition.ComparisonType.GreaterThan, 1),
                    new CurrencyValueCondition (Currency.STATE, ICondition.ComparisonType.FewerThan, 5)
                ), 4),
                new (new CurrencyValueCondition (Currency.STATE, ICondition.ComparisonType.GreaterThanOrEqual, 5), 11),
            ]
        );
        Result oneFewerTension = new (
            1,
            [
                new (new BallotPassedCondition (ballotD.ID, true), 2),
                new (new BallotPassedCondition (ballotD.ID, false), 3),
            ]
        );
        Result ballotDPassed = new (
            2,
            []
        );
        Result ballotDFailed = new (
            3,
            []
        );
        Result twoFourTension = new (
            4,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 5),
                new (new BallotPassedCondition (incidentA.ID, false), 10),
            ]
        );
        Result incidentAPassed = new (
            5,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 6),
                new (new BallotPassedCondition (ballotB.ID, false), 9),
            ]
        );
        Result ballotBPassed = new (
            6,
            [
                new (new BallotPassedCondition (incidentB.ID, true), 7),
                new (new BallotPassedCondition (incidentB.ID, false), 8),
            ]
        );
        Result incidentBPassed = new (
            7,
            []
        );
        Result incidentBFailed = new (
            8,
            []
        );
        Result ballotBFailed = new (
            9,
            []
        );
        Result incidentAFailed = new (
            10,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 6),
                new (new BallotPassedCondition (ballotB.ID, false), 9),
            ]
        );
        Result fiveGreaterTension = new (
            11,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 12),
                new (new BallotPassedCondition (ballotB.ID, false), 13),
            ]
        );
        Result ballotBPassed2 = new (
            12,
            []
        );
        Result ballotBFailed2 = new (
            13,
            []
        );
        List<Result> results = [
            resultStart,
            oneFewerTension,
            ballotDPassed,
            ballotDFailed,
            twoFourTension,
            incidentAPassed,
            ballotBPassed,
            incidentBPassed,
            incidentBFailed,
            ballotBFailed,
            incidentAFailed,
            fiveGreaterTension,
            ballotBPassed2,
            ballotBFailed2,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[oneFewerTension.ID] = (
            "One or Fewer Tension",
            [
                "Quebec votes against sovereignty",
                StringLineFormatter.Indent ("This gives impetus to the constitutional independence movement", 1),
                StringLineFormatter.Indent ("This does not preclude another referendum if Quebec’s demands are not satisfied", 1),
                StringLineFormatter.Indent ("However, tensions have cooled down somewhat and violent revolt is extremely unlikely", 2),
            ]
        );
        resultsLocs[ballotDPassed.ID] = (
            "Ballot D Passed",
            [
                "Approval of a charter leads to provincial backlash",
                StringLineFormatter.Indent ("Quebec sees this as a betrayal", 1),
                "Federal government threatens to go ahead with patriation without provincial approval",
                StringLineFormatter.Indent ("Eventually, all provinces except Quebec agree to patriation", 1),
                "Canada becomes constitutionally independent",
                StringLineFormatter.Indent ("Canada, though divided, may unite around the Charter as a symbol of civic identity", 1),
            ]
        );
        resultsLocs[ballotDFailed.ID] = (
            "Ballot D Failed",
            [
                "Rejection of a charter leads to popular backlash",
                "Federal government refuses to go ahead with patriation without a charter",
                StringLineFormatter.Indent ("British wipe their hands of Canada and patriate the constitution with an amending formula subject to referendum", 1),
                "Constitutional debates will dominate politics for the coming years",
                StringLineFormatter.Indent ("Though this may detract from policy issues, it is still much better than a more divided nation", 1),
            ]
        );
        resultsLocs[twoFourTension.ID] = (
            "Greater than One and Fewer than Five Tension",
            [
                "British are very reluctant to patriate the constitution",
                StringLineFormatter.Indent ("They don’t believe that there is enough consensus in Canadian society to grant constitutional independence", 1),
            ]
        );
        resultsLocs[incidentAPassed.ID] = (
            "Incident A Passed",
            [
                "Quebec barely votes for sovereignty",
                StringLineFormatter.Indent ("Federal government refuses to recognise this result", 1),
                StringLineFormatter.Indent ("Massive civil unrest breaks out, but no large-scale violence", 1),
            ]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Tensions in Quebec contribute to civil unrest in the west",
                StringLineFormatter.Indent ("This is especially true for Alberta and Saskatchewan", 1),
                StringLineFormatter.Indent ("Quebec is very unlikely to remain in Canada by choice", 1),
            ]
        );
        resultsLocs[incidentBPassed.ID] = (
            "Incident B Passed",
            [
                "Without support from the west, the British refuse to patriate the constitution",
                StringLineFormatter.Indent ("Negotiations on oil revenues sharing will have to be resolved for anything to change", 1),
                StringLineFormatter.Indent ("Alberta’s successful opposition to the federal government also empowers British Columbia", 1),
                StringLineFormatter.Indent ("This especially impacts Chinese and First Nations who don’t have federal protections", 2),
                "Canada is badly divided and at an impasse",
                StringLineFormatter.Indent ("Constitutional projects will have to be put on hold", 1),
            ]
        );
        resultsLocs[incidentBFailed.ID] = (
            "Incident B Failed",
            [
                "Negotiations with Alberta result in some begrudging agreement",
                StringLineFormatter.Indent ("British warily patriate the constitution with a 7/50 amending formula", 1),
                StringLineFormatter.Indent ("This is seen in Quebec as a betrayal by Anglophone Canada", 2),
                "Canada faces severe issues over identity",
                StringLineFormatter.Indent ("Federal government has much to do to create a united nation", 1),
                StringLineFormatter.Indent ("Anglophone-Francophone conflict continues", 2),
                StringLineFormatter.Indent ("East-West conflict will likely get worse", 2),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Incident B Failed",
            [
                "With support from the west, the federal government convinces the British that it has provincial support",
                StringLineFormatter.Indent ("British warily patriate the constitution with a 7/50 amending formula", 1),
                StringLineFormatter.Indent ("This is seen in Quebec as a betrayal by Anglophone Canada", 2),
                "Canada faces severe issues over identity",
                StringLineFormatter.Indent ("Federal government has much to do to create a united nation", 1),
                StringLineFormatter.Indent ("Anglophone-Francophone conflict continues", 2),
            ]
        );
        resultsLocs[incidentAFailed.ID] = (
            "Incident A Failed",
            [
                "Quebec votes for sovereignty",
                StringLineFormatter.Indent ("Federal government refuses to recognise this result", 1),
                StringLineFormatter.Indent ("FLQ makes good on its threat to raise a rebellion, although it is nowhere near as massive as threatened", 1),
                StringLineFormatter.Indent ("Separatists are eventually suppressed, but this is extremely bad for Canada’s global image", 2),
            ]
        );
        resultsLocs[fiveGreaterTension.ID] = (
            "Five or Greater Tension",
            [
                "Federal-provincial relations are horrific",
                StringLineFormatter.Indent ("British categorically refuse to patriate the constitution on account of provincial opposition", 1),
                "Quebec unilaterally declares independence",
                StringLineFormatter.Indent ("Revolt is suppressed, but guerrilla warfare continues across the province", 1),
            ]
        );
        resultsLocs[ballotBPassed2.ID] = (
            "Ballot B Passed",
            [
                "Albertan separatists begin agitating for independence",
                StringLineFormatter.Indent ("This agitation spills over into Saskatchewan", 1),
                "If the federal government doesn’t find a solution to these tensions, Canada could collapse",
                StringLineFormatter.Indent ("International outrage at the status of Quebec is especially severe", 1),
                StringLineFormatter.Indent ("Maritime provinces draw closer, believing the federal government is unreliable", 1),
                StringLineFormatter.Indent ("Some even float the idea of joining the US", 2),
                StringLineFormatter.Indent ("Western provinces may declare independence soon", 1),
            ]
        );
        resultsLocs[ballotBFailed2.ID] = (
            "Ballot B Failed",
            [
                "Western support keeps Anglophone Canada largely united",
                StringLineFormatter.Indent ("This bodes extremely poorly for the future of Canada as a bilingual nation", 1),
                StringLineFormatter.Indent ("Acadians in New Brunswick question their future and status", 2),
                "If the federal government doesn’t find a solution to these tensions, Quebec will probably become independent",
                StringLineFormatter.Indent ("International outrage at the status of Quebec is especially severe", 1),
            ]
        );

        History history = new (
            [ballotA.ID, incidentA.ID, ballotB.ID, incidentB.ID, ballotD.ID],
            []
        );

        Simulation = new (
            history,
            roles,
            provinces,
            parties,
            currenciesValues,
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Canada",
                "Constitutional Monarchy",
                [
                    "Canada is going through major societal changes",
                    StringLineFormatter.Indent ("Maple leaf flag has just been adopted", 1),
                    StringLineFormatter.Indent ("Quebec is going through the Quiet Revolution, spearheaded by the Quebec Liberal Party", 1),
                    StringLineFormatter.Indent ("This has led to an outpouring of Quebecer nationalism", 2),
                    "Canada now has an opportunity to unite its disparate peoples into one nation",
                    StringLineFormatter.Indent ("There is a strong desire for constitutional independence", 1),
                    StringLineFormatter.Indent ("However, Quebec’s wariness is not easy to overcome", 2),
                ],
                "20 April 1968",
                "Ascension of Pierre Trudeau",
                "Patriation",
                rolesLocs,
                "Speaker",
                ("Province", "Provinces"),
                provincesLocs,
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
