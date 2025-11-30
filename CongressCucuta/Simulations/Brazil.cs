using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Simulations;

[ExcludeFromCodeCoverage]
internal class Brazil : ISimulation {
    public Simulation Simulation { get; }

    public Brazil () {
        IDType deputy = Role.MEMBER;
        IDType president = Role.HEAD_STATE;
        List<IDType> roles = [
            deputy,
            president,
            Role.LEADER_REGION,
            0,
            1,
            2,
            3,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[deputy] = ("Deputy", "Deputies");
        rolesLocs[president] = ("President", "Presidents");
        rolesLocs[0] = ("Governor of Paulistania-Minas", "Governors of Paulistania-Minas");
        rolesLocs[1] = ("Governor of South Brazil", "Governors of South Brazil");
        rolesLocs[2] = ("Governor of Great Para", "Governors of Great Para");
        rolesLocs[3] = ("Governor of Maranhao-Pernambuco", "Governors of Maranhao-Pernambuco");
        rolesLocs[Role.LEADER_REGION] = ("Governor", "Governors");

        Faction paulistaniaMinas = new (0);
        Faction southBrazil = new (1);
        Faction greatPara = new (2);
        Faction maranhaoPernambuco = new (3);
        List<Faction> states = [paulistaniaMinas, southBrazil, greatPara, maranhaoPernambuco];
        Dictionary<IDType, (string, string[])> statesLocs = [];
        statesLocs[paulistaniaMinas.ID] = (
            "Paulistania-Minas",
            [
                StringLineFormatter.Indent ("Wealthiest states, reliant on exports of dairy and coffee", 1),
            ]
        );
        statesLocs[southBrazil.ID] = (
            "South Brazil",
            [
                StringLineFormatter.Indent ("Most politically radical states, but marginalised", 1),
                StringLineFormatter.Indent ("Large immigrant population", 1),
            ]
        );
        statesLocs[greatPara.ID] = (
            "Great Para",
            [
                StringLineFormatter.Indent ("Extremely poor and isolated states", 1),
                StringLineFormatter.Indent ("Formerly known for rubber production, but no longer generates any revenue", 1),
                StringLineFormatter.Indent ("Large indigenous population", 1),
            ]
        );
        statesLocs[maranhaoPernambuco.ID] = (
            "Northeast Brazil",
            [
                StringLineFormatter.Indent ("Historic centre of republicanism and incredibly rebellious", 1),
                StringLineFormatter.Indent ("Reliant on plantation crops, whose production suffered from abolition", 1),
                StringLineFormatter.Indent ("Large black and mixed populations", 1),
            ]
        );

        Dictionary<IDType, sbyte> currenciesValues = [];
        IDType influencePaulistaniaMinas = paulistaniaMinas.ID;
        IDType influenceSouthBrazil = southBrazil.ID;
        IDType influenceMaranhaoPernambuco = maranhaoPernambuco.ID;
        IDType influenceGreatPara = greatPara.ID;
        currenciesValues[influencePaulistaniaMinas] = 4;
        currenciesValues[influenceSouthBrazil] = 2;
        currenciesValues[influenceMaranhaoPernambuco] = 2;
        currenciesValues[influenceGreatPara] = 1;
        Dictionary<IDType, string> currencies = [];
        currencies[paulistaniaMinas.ID] = "Influence";
        currencies[southBrazil.ID] = "Influence";
        currencies[maranhaoPernambuco.ID] = "Influence";
        currencies[greatPara.ID] = "Influence";
        currencies[Currency.REGION] = "Influence";

        ProcedureImmediate presidentialElection = new (
            0,
            [
                new (Procedure.Effect.EffectType.ElectionNominated, [president]),
                new (Procedure.Effect.EffectType.PermissionsCanVote, [president], 0),
            ]
        );
        ProcedureImmediate federation = new (
            1,
            [new (Procedure.Effect.EffectType.ElectionRegion, [])]
        );
        List<ProcedureImmediate> proceduresGovernmental = [presidentialElection, federation];
        ProcedureTargeted ruleColonels = new (
            2,
            [new (Procedure.Effect.EffectType.CurrencyInitialise, [])],
            [1]
        );
        ProcedureTargeted coffeeMilkPolitics = new (
            3,
            [
                new (Procedure.Effect.EffectType.ProcedureActivate, [presidentialElection.ID]),
                new (Procedure.Effect.EffectType.CurrencyAdd, [influencePaulistaniaMinas], 1),
            ],
            []
        );
        ProcedureTargeted militaryClub = new (
            4,
            [new (Procedure.Effect.EffectType.CurrencySubtract, [influencePaulistaniaMinas], 1)],
            []
        );
        ProcedureTargeted brazilianNationImmigrant = new (
            5,
            [new (Procedure.Effect.EffectType.VoteFailAdd, [], 1)],
            []
        );
        ProcedureTargeted brazilianWorkersConfederation = new (
            6,
            [new (Procedure.Effect.EffectType.VotePassAdd, [], 2)],
            [3],
            false
        );
        ProcedureTargeted communistPartyBrazil = new (
            7,
            [new (Procedure.Effect.EffectType.VotePassTwoThirds, [])],
            [3],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            ruleColonels,
            coffeeMilkPolitics,
            militaryClub,
            brazilianNationImmigrant,
            brazilianWorkersConfederation,
            communistPartyBrazil
        ];
        ProcedureDeclared statesPolicy = new (
            8,
            [
                new (Procedure.Effect.EffectType.BallotPass, []),
                new (Procedure.Effect.EffectType.CurrencyAdd, [Currency.REGION], 1),
            ],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [president]
        );
        ProcedureDeclared veto = new (
            9,
            [
                new (Procedure.Effect.EffectType.BallotFail, [])
            ],
            new Confirmation (Confirmation.ConfirmationType.Always),
            [president]
        );
        ProcedureDeclared revolt = new (
            10,
            [new (Procedure.Effect.EffectType.BallotLimit, [])],
            new Confirmation (Confirmation.ConfirmationType.DiceCurrency),
            [Role.LEADER_REGION]
        );
        List<ProcedureDeclared> proceduresDeclared = [statesPolicy, veto, revolt];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[presidentialElection.ID] = (
            "Presidential Election",
            "This country has a powerful, elected executive who is separate from the legislature."
        );
        procedures[federation.ID] = (
            "Federation",
            "This country's government is organised as a federation, divided into powerful administrative divisions with individual representation in the national government."
        );
        procedures[ruleColonels.ID] = (
            "Rule of the Colonels",
            "Brazil is dominated by local oligarchs, colloquially known as colonels, who are responsible for most aspects of civic engagement and governance."
        );
        procedures[coffeeMilkPolitics.ID] = (
            "Coffee with Milk Politics",
            "The two wealthiest states of Brazil, São Paulo and Minas Geraes, have informally agreed to share national power between them, much to the chagrin of the other states. The name is an allusion to the dominant industries of the two states."
        );
        procedures[militaryClub.ID] = (
            "Military Club",
            "The Military Club, an organisation of high-ranking officers, masterminded the creation of the republic. While the organisation lacks ideological unity and popular support, it remains a powerful force that could threaten the civilian government."
        );
        procedures[brazilianNationImmigrant.ID] = (
            "Brazilian Nation to the Immigrant",
            "Brazil has always been multinational, and the abolition of slavery has only reinforced this fact. Thousands of Japanese, Italian, and even Syrian immigrants now call Brazil home, with consequences that are as of yet unknown."
        );
        procedures[brazilianWorkersConfederation.ID] = (
            "Brazilian Workers' Confederation",
            Localisation.UNUSED
        );
        procedures[communistPartyBrazil.ID] = (
            "Communist Party of Brazil",
            Localisation.UNUSED
        );
        procedures[statesPolicy.ID] = (
            "States' Policy",
            "Opposition by the military can be, at great cost, subverted through patronage networks. In exchange for their votes, these congressmen and state governors expect monetary favours."
        );
        procedures[veto.ID] = (
            "Veto",
            "This country's head of state is empowered to reject legislation that does not conform to his political beliefs."
        );
        procedures[revolt.ID] = (
            "Revolt",
            "The Public Forces are a uniquely republican innovation and a mechanism for state-led policy-making. Each state has its own army, with São Paulo and Rio Grande do Sul having the strongest, with predictable results when state interests aren't aligned with national interests."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [influenceGreatPara, influenceMaranhaoPernambuco], 1),
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [influencePaulistaniaMinas], -1),
                ],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [influencePaulistaniaMinas], 1)],
                [new (new AlwaysCondition (), 1)]
            )
        );
        Ballot ballotB = new (
            1,
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.RemoveProcedure, [militaryClub.ID]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [influenceGreatPara, influenceMaranhaoPernambuco], -1),
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
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [brazilianNationImmigrant.ID, brazilianWorkersConfederation.ID]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [influenceSouthBrazil], 1),
                ],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [brazilianNationImmigrant.ID, communistPartyBrazil.ID]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ModifyCurrency, [influenceSouthBrazil], 1),
                ],
                [
                    new (new ProcedureActiveCondition (militaryClub.ID, true), Ballot.END),
                    new (new ProcedureActiveCondition (militaryClub.ID, false), 3),
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
            "Negotiate the Funding Loan",
            [
                StringLineFormatter.Indent ("Brazil is badly indebted to foreign lenders", 1),
                StringLineFormatter.Indent ("In order to continue funding the government, the Rothschilds are willing to offer a large loan on several conditions", 1),
                StringLineFormatter.Indent ("Customs income from Rio de Janeiro and Santos will be mortgaged", 2),
                StringLineFormatter.Indent ("No additional loans may be taken", 2),
                StringLineFormatter.Indent ("Inflation must be controlled via destruction of paper money", 2),
                StringLineFormatter.Indent ("In exchange, this would place a temporary moratorium on loan repayments, then extend the repayment period over several decades", 1),
                StringLineFormatter.Indent ("These repayments will be funded by levying additional consumption taxes and reducing spending", 1),
            ],
            [
                "From certain points of view, the Funding Loan is a great success",
                StringLineFormatter.Indent ("Brazil finally has a budget surplus", 1),
                StringLineFormatter.Indent ("Currency appreciates for the first time", 1),
                "For most people, this is terrible",
                StringLineFormatter.Indent ("Imports become cheaper, which hurts Brazilian businesses", 1),
                StringLineFormatter.Indent ("Factories close down or reduce production", 2),
                StringLineFormatter.Indent ("Exports become more expensive, which makes Brazilian products less competitive", 1),
                StringLineFormatter.Indent ("Overall, living standards drop", 1),
                "Only time can tell if this will help the economy",
            ],
            [
                "Debt and inflation continue to be major problems",
                StringLineFormatter.Indent ("Brazil will continue to look for export-based solutions to financial problems", 1),
                StringLineFormatter.Indent ("This has the distinct danger of failing if the world economy crashes", 2),
                StringLineFormatter.Indent ("For now, businesses can look forward to a more liberal economy", 1),
                StringLineFormatter.Indent ("This reinforces the coffee and milk oligarchy", 2),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Eliminate infectious diseases",
            [
                StringLineFormatter.Indent ("Public health is a serious concern in many cities", 1),
                StringLineFormatter.Indent ("Yellow fever, the plague, and smallpox all run rampant in the cities", 2),
                StringLineFormatter.Indent ("This would begin an ambitious campaign to wipe out these diseases", 1),
                StringLineFormatter.Indent ("Concretely, this would lead to major city cleaning efforts", 2),
                StringLineFormatter.Indent ("Widening streets, improving sewage, reducing animal presence", 3),
                StringLineFormatter.Indent ("Additionally, smallpox vaccinations will become mandatory", 2),
                StringLineFormatter.Indent ("This is incredibly expensive and unpopular", 1),
            ],
            [
                "This is a huge success",
                StringLineFormatter.Indent ("Opposition to these efforts dies down after its completion", 1),
                StringLineFormatter.Indent ("This becomes a convenient moment to shut down the subversive Military Club", 2),
                StringLineFormatter.Indent ("Overarching public health goals are largely achieved", 2),
                StringLineFormatter.Indent ("Fewer and fewer cases of dangerous infectious diseases are reported each year", 3),
                "This has an unexpected side effect of increasing living costs",
                StringLineFormatter.Indent ("Cleaning efforts also reduced housing availability, forcing workers to commute larger distances", 1),
                StringLineFormatter.Indent ("Combined with the cost of the programme, this may be problematic", 1),
            ],
            [
                "Brazil remains disease-ridden and unsanitary",
                StringLineFormatter.Indent ("Healthcare is generally not sufficient to manage the current disease outbreaks", 1),
                StringLineFormatter.Indent ("This will only get worse as Brazil's population increases, especially due to immigration", 2),
                StringLineFormatter.Indent ("There is no political will to change this situation", 1),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Deport anarchists",
            [
                StringLineFormatter.Indent ("Influx of immigration has also led to the spread of anarchism and socialism", 1),
                StringLineFormatter.Indent ("Economic activity has increasingly been interrupted by strikes", 2),
                StringLineFormatter.Indent ("This is made worse by the Great War, which has led to a general increase in prices", 3),
                StringLineFormatter.Indent ("Anarchism can never be defeated solely by deporting its followers", 1),
                StringLineFormatter.Indent ("Many industrial workers are unionised", 2),
                StringLineFormatter.Indent ("However, it could stop militants from gaining currency in the socialist movement", 2),
            ],
            [
                "This largely backfires or has no effect",
                StringLineFormatter.Indent ("Anarchists only become more militant as a result of these deportations", 1),
                StringLineFormatter.Indent ("Trade unions expand across the state, as well as the strikes", 1),
                StringLineFormatter.Indent ("These are eventually suppressed or negotiated down", 2),
                "Persecution of socialists has largely made them a permanent fixture in politics",
                StringLineFormatter.Indent ("Events in Russia only embolden them further", 1),
                StringLineFormatter.Indent ("While the unrest is not yet catastrophic, it could explode in the future", 1),
            ],
            [
                "Anarchist movement grows bolder and more violent",
                StringLineFormatter.Indent ("To some degree, this is beneficial", 1),
                StringLineFormatter.Indent ("It's easier to legitimise the use of deadly force against violent strikers", 2),
                StringLineFormatter.Indent ("Not everyone is comfortable with militant anarchism", 2),
                "Socialism is likely here to stay",
                StringLineFormatter.Indent ("Events in Russia only embolden them further", 1),
                StringLineFormatter.Indent ("While the movement has split, it's not in any danger of destruction", 1),
                StringLineFormatter.Indent ("There may be an opportunity to negotiate with some moderates", 2),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Declare a state of emergency",
            [
                StringLineFormatter.Indent ("There was a revolt in Rio de Janeiro", 1),
                StringLineFormatter.Indent ("While the revolt was crushed, widespread army dissatisfaction means another will likely take place soon", 2),
                StringLineFormatter.Indent ("These \"tenentists\" demanded agrarian reform and the end of the milk and coffee oligarchy", 2),
                StringLineFormatter.Indent ("Some rebels appear to have ties with the anarchist and communist movements", 3),
                StringLineFormatter.Indent ("Others are merely worried about the steady erosion of military influence", 3),
                StringLineFormatter.Indent ("Generally speaking, these rebels are most concerned about middle-class, liberal interests", 3),
                StringLineFormatter.Indent ("Tenentism, now known to the public, is becoming very popular", 1),
                StringLineFormatter.Indent ("These rebels are now hailed as heroes worth emulating", 2),
                StringLineFormatter.Indent ("If this movement is not destroyed, then the entire oligarchic system could collapse", 3),
                StringLineFormatter.Indent ("This would institute martial law while the tenentists are militarily crushed", 3),
                "We will try to negotiate with the tenentists if this ballot fails"
            ],
            [
                "Tenentist uprisings grow in number and strength",
                StringLineFormatter.Indent ("São Paulo and south Brazil are revolutionary hotspots", 1),
                StringLineFormatter.Indent ("These revolts all fail, but the São Paulo revolt was extremely destructive and nearly successful", 2),
                StringLineFormatter.Indent ("Survivors are represented by a lone guerrilla force, Luís Prestes' Column", 3),
                "While Brazil is now at peace, it is certain not to last",
                StringLineFormatter.Indent ("Many tenentists have fled abroad, from where they continue to agitate against the government", 1),
            ],
            [
                "Most tenentist demands are unacceptable",
                StringLineFormatter.Indent ("However, certain demands are accepted:", 1),
                StringLineFormatter.Indent ("Secret ballot", 2),
                StringLineFormatter.Indent ("Free public education for all", 2),
                StringLineFormatter.Indent ("Amnesty for rebels", 2),
                StringLineFormatter.Indent ("This does not satisfy the tenentists, who continue to revolt", 1),
                StringLineFormatter.Indent ("They are especially concerned with the rejection of land reform and an independent electoral commission", 2),
                StringLineFormatter.Indent ("However, they find less support than they had hoped for, and the movement eventually dies out", 2),
                StringLineFormatter.Indent ("Survivors are represented by a lone guerrilla force, Luís Prestes' Column", 3),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (ballotA.ID, true), 1),
                new (new BallotPassedCondition (ballotA.ID, false), 9),
            ]
        );
        Result ballotAPassed = new (
            1,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 2),
                new (new BallotPassedCondition (ballotC.ID, false), 5),
            ]
        );
        Result ballotCPassed = new (
            2,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 3),
                new (new BallotPassedCondition (incidentA.ID, false), 4),
            ]
        );
        Result incidentAPassed = new (
            3,
            []
        );
        Result incidentAFailed = new (
            4,
            []
        );
        Result ballotCFailed = new (
            5,
            [
                new (new AndCondition (
                    new ProcedureActiveCondition (militaryClub.ID, false),
                    new BallotPassedCondition (incidentA.ID, true)
                ), 6),
                new (new AndCondition (
                    new ProcedureActiveCondition (militaryClub.ID, false),
                    new BallotPassedCondition (incidentA.ID, false)
                ), 7),
                new (new ProcedureActiveCondition (militaryClub.ID, true), 8),
            ]
        );
        Result militaryClubInactiveIncidentAPassed = new (
            6,
            []
        );
        Result militaryClubInactiveIncidentAFailed = new (
            7,
            []
        );
        Result militaryClubActive = new (
            8,
            []
        );
        Result ballotAFailed = new (
            9,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 10),
                new (new BallotPassedCondition (ballotB.ID, true), 13),
            ]
        );
        Result ballotBPassed = new (
            10,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 13),
                new (new BallotPassedCondition (ballotC.ID, false), 14),
            ]
        );
        Result ballotCPassed2 = new (
            11,
            []
        );
        Result ballotCFailed2 = new (
            12,
            []
        );
        Result ballotBFailed = new (
            13,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 6),
                new (new BallotPassedCondition (incidentA.ID, false), 7),
            ]
        );
        Result incidentAPassed2 = new (
            14,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 6),
                new (new BallotPassedCondition (incidentA.ID, false), 7),
            ]
        );
        Result incidentAFailed2 = new (
            15,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 6),
                new (new BallotPassedCondition (incidentA.ID, false), 7),
            ]
        );
        List<Result> results = [
            resultStart,
            ballotAPassed,
            ballotCPassed,
            incidentAPassed,
            incidentAFailed,
            ballotCFailed,
            militaryClubInactiveIncidentAPassed,
            militaryClubInactiveIncidentAFailed,
            militaryClubActive,
            ballotAFailed,
            ballotBPassed,
            ballotCPassed2,
            ballotCFailed2,
            ballotBFailed,
            incidentAPassed2,
            incidentAFailed2,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[ballotAPassed.ID] = (
            "Ballot A Passed",
            [
                "Government has the funds needed to carry out its policy",
                StringLineFormatter.Indent ("However, the threats of debt and foreign control continue to loom over society", 1),
            ]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "Anarchists eventually lose influence in society",
                StringLineFormatter.Indent ("Tenentism becomes the voice of popular discontent", 1),
                StringLineFormatter.Indent ("However, the two movements tolerate each other and sometimes cooperate", 1),
            ]
        );
        // Based on the Rio Grande do Sul revolts in 1924 - 1926 and the revolution in 1930
        resultsLocs[incidentAPassed.ID] = (
            "Incident A Passed",
            [
                "Revolution breaks out in Porto Alegre",
                StringLineFormatter.Indent ("Tenentist exiles and officers back a liberal coalition that overthrows the government", 1),
                StringLineFormatter.Indent ("Anarchists organise some support for the revolution, although they aren't too excited about liberalism", 1),
                "New government cracks down on dissent and organises a dictatorship",
                StringLineFormatter.Indent ("Goal is to break the oligarchy and prepare Brazil for a liberal republic", 1),
                StringLineFormatter.Indent ("Many doubt that they will succeed, but there is some hope for the future", 2),
            ]
        );
        resultsLocs[incidentAFailed.ID] = (
            "Incident A Failed",
            [
                "Revolution breaks out in Porto Alegre",
                StringLineFormatter.Indent ("Tenentist exiles and officers back a liberal coalition that fails", 1),
                StringLineFormatter.Indent ("Anarchists organise some support for the revolution, although they aren't too excited about liberalism", 1),
                "Revolution's aftershocks are concerning",
                StringLineFormatter.Indent ("Economic shift away from exports badly hurts the oligarchy anyway", 1),
                StringLineFormatter.Indent ("Traditional coffee with milk system is slowly dismantled under the influence of other states", 2),
                StringLineFormatter.Indent ("In reality, little actually changes for the common person", 1),
                StringLineFormatter.Indent ("Brazil will probably remain unstable in the years to come", 2),
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "Socialist split leads to communism gaining power",
                StringLineFormatter.Indent ("Support from the USSR leads to rapid growth", 1),
                StringLineFormatter.Indent ("Communists eventually infiltrate the tenentist movement", 1),
            ]
        );
        // Based on endemic banditry, the Prestes column, and the Natal and Recife revolt in 1935
        resultsLocs[militaryClubInactiveIncidentAPassed.ID] = (
            "Military Club Inactive and Incident A Passed",
            [
                "Revolution breaks out in Natal",
                StringLineFormatter.Indent ("Tenentist exiles and officers back a socialist coalition that overthrows the government", 1),
                StringLineFormatter.Indent ("Anarchists organise some support for the revolution, although they aren't too excited about communism", 1),
                "New government cracks down on dissent and organises a dictatorship",
                StringLineFormatter.Indent ("Goal is to break the oligarchy and convert Brazil into a socialist dictatorship", 1),
                StringLineFormatter.Indent ("This is such a bizarre and drastic change of course that it's destined to fail", 2),
                StringLineFormatter.Indent ("International opposition and rural rebellions destroy the socialist experiment", 3),
            ]
        );
        resultsLocs[militaryClubInactiveIncidentAFailed.ID] = (
            "Military Club Inactive and Incident A Failed",
            [
                "Revolution breaks out in Natal",
                StringLineFormatter.Indent ("Tenentist exiles and officers back a socialist coalition that fails", 1),
                StringLineFormatter.Indent ("Anarchists stay silent", 1),
                "Revolution's aftershocks are concerning",
                StringLineFormatter.Indent ("Economic shift away from exports badly hurts the oligarchy anyway", 1),
                StringLineFormatter.Indent ("Traditional coffee with milk system is slowly dismantled under the influence of other states", 2),
                StringLineFormatter.Indent ("In reality, little actually changes for the common person", 1),
                StringLineFormatter.Indent ("However, the growth of communism has not gone unnoticed and it could prove to be much more dangerous in the future", 2),
                StringLineFormatter.Indent ("Brazil will probably remain unstable in the years to come", 2),
            ]
        );
        // See previous
        resultsLocs[militaryClubActive.ID] = (
            "Military Club Active",
            [
                "Revolution breaks out in Porto Alegre",
                StringLineFormatter.Indent ("Tenentist exiles and officers back a liberal coalition that fails", 1),
                StringLineFormatter.Indent ("Anarchists organise some support for the revolution, although they aren't too excited about liberalism", 1),
                "Government survives mostly intact",
                StringLineFormatter.Indent ("Economic shift away from exports badly hurts the oligarchy anyway", 1),
                StringLineFormatter.Indent ("Traditional coffee with milk system is slowly dismantled under the influence of other states", 2),
                StringLineFormatter.Indent ("In reality, little actually changes for the common person", 1),
                StringLineFormatter.Indent ("Communism will slowly fade away", 2),
                StringLineFormatter.Indent ("Brazil will probably remain unstable in the years to come", 2),
            ]
        );
        resultsLocs[ballotAFailed.ID] = (
            "Ballot A Failed",
            [
                "Government lacks the funds needed to carry out its policy",
                StringLineFormatter.Indent ("This is likely to exacerbate tensions in the short term", 1),
            ]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Leftover discontent from mandatory vaccination gives the opposition a wide-ranging character",
                StringLineFormatter.Indent ("Essentially, many regard the government as wasting money on vanity projects instead of helping Brazilians", 1),
                StringLineFormatter.Indent ("With the rise of tenentism, this could be disastrous", 1),
            ]
        );
        // Based on the São Paulo strikes in 1917 - 1919 and the revolt in 1924
        resultsLocs[ballotCPassed2.ID] = (
            "Ballot C Passed",
            [
                "Revolution breaks out in São Paulo",
                StringLineFormatter.Indent ("Tenentist exiles and officers back an anarchist coalition that overthrows the government", 1),
                StringLineFormatter.Indent ("Rural poor organise some support for the revolution, although they aren't too involved", 1),
                "New government cracks down on dissent and organises a dictatorship",
                StringLineFormatter.Indent ("Goal is to break the oligarchy and convert Brazil into a council republic", 1),
                StringLineFormatter.Indent ("This is such a bizarre and drastic change of course that it's destined to fail", 2),
                StringLineFormatter.Indent ("International opposition and northeastern counter-revolutionaries destroy the anarchist experiment", 3),
            ]
        );
        resultsLocs[ballotCFailed2.ID] = (
            "Ballot C Failed",
            [
                "Revolution breaks out in São Paulo",
                StringLineFormatter.Indent ("Tenentist exiles and officers back an anarchist coalition that fails", 1),
                StringLineFormatter.Indent ("Rural poor organise some support for the revolution, although they aren't too involved", 1),
                "Revolution's aftershocks are concerning",
                StringLineFormatter.Indent ("Fear of anarchism leads to a flurry of reform", 1),
                StringLineFormatter.Indent ("Economic shift away from exports badly hurts the oligarchy anyway", 2),
                StringLineFormatter.Indent ("Some change is noted for the common person", 1),
                StringLineFormatter.Indent ("Living standards remain poor, but there are more political freedoms", 2),
                StringLineFormatter.Indent ("Transition to a liberal government seems plausible", 3),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Government doesn't particularly spend beyond its means anyway",
                StringLineFormatter.Indent ("This tempers some discontent, but entrenches the oligarchy", 1),
            ]
        );
        // Based on the São Paulo revolt in 1924 
        resultsLocs[incidentAPassed2.ID] = (
            "Incident A Passed",
            [
                "Revolution breaks out in São Paulo",
                StringLineFormatter.Indent ("Tenentist exiles and officers back a conservative coalition that overthrows the government", 1),
                StringLineFormatter.Indent ("Anarchists organise some support for the revolution, although they aren't too excited about conservatism", 1),
                "New government cracks down on dissent and organises a dictatorship",
                StringLineFormatter.Indent ("Goal is to break the oligarchy and improve the lot of Brazilians", 1),
                StringLineFormatter.Indent ("This particular revolution has an incredibly unclear focus and is not exactly ideological", 2),
                StringLineFormatter.Indent ("Many doubt that they will succeed, and there is already opposition brewing in São Paulo", 2),
                StringLineFormatter.Indent ("Most likely outcome is a different kind of oligarchy", 3),
            ]
        );
        resultsLocs[incidentAFailed2.ID] = (
            "Incident A Failed",
            [
                "Revolution breaks out in São Paulo",
                StringLineFormatter.Indent ("Tenentist exiles and officers back a conservative coalition that fails", 1),
                StringLineFormatter.Indent ("Anarchists stay silent", 1),
                "Government survives mostly intact",
                StringLineFormatter.Indent ("Economic shift away from exports badly hurts the oligarchy anyway", 1),
                StringLineFormatter.Indent ("Traditional coffee with milk system is slowly dismantled under the influence of other states", 2),
                StringLineFormatter.Indent ("Lack of a credible opposition means very little will ever change for the common person", 1),
                StringLineFormatter.Indent ("Most likely outcome is a different kind of oligarchy", 2),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[ballotB.ID] = [revolt.ID];
        ballotsProceduresDeclared[incidentA.ID] = [revolt.ID];
        History history = new (
            [ballotA.ID, ballotB.ID, ballotC.ID, incidentA.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            states,
            [],
            currenciesValues,
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Brazil",
                "Presidential Republic",
                [
                    "Brazil gained independence as a monarchy",
                    StringLineFormatter.Indent ("Under Pedro II, Brazil's economy and international prestige were at their peak", 1),
                    StringLineFormatter.Indent ("However, Pedro II had grown weary of ruling and silently allowed a coup to establish a republic", 1),
                    "Public opinion was opposed to the coup",
                    StringLineFormatter.Indent ("\"Republic of the Sword\" violently stamped out dissent but gradually gained popular support", 1),
                    StringLineFormatter.Indent ("Eventually, the military stepped down and Brazil held its first elections", 1),
                ],
                "15 November 1898",
                "Ascension of Campos Sales",
                "The Old Republic",
                rolesLocs,
                "Chamber President (Chairman)",
                ("State", "States"),
                statesLocs,
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
