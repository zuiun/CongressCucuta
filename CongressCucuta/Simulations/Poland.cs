using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Simulations;

[ExcludeFromCodeCoverage]
internal class Poland : ISimulation {
    public Simulation Simulation { get; }

    public Poland () {
        IDType deputy = Role.MEMBER;
        IDType primeMinister = Role.HEAD_GOVERNMENT;
        List<IDType> roles = [
            deputy,
            primeMinister,
            0,
            1,
            2,
            Role.LEADER_PARTY,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[deputy] = ("Deputy", "Deputies");
        rolesLocs[primeMinister] = ("Prime Minister", "Prime Ministers");
        rolesLocs[0] = ("Leader of Chjeno-Piast", "Leaders of Chjeno-Piast");
        rolesLocs[1] = ("Chairman of PPS", "Chairmen of PPS");
        rolesLocs[2] = ("Leader of BMN", "Leaders of BMN");
        rolesLocs[Role.LEADER_PARTY] = ("Party Leader", "Party Leaders");

        Faction chjenoPiast = new (0);
        Faction pps = new (1);
        Faction bmn = new (2);
        List<Faction> factions = [chjenoPiast, pps, bmn];
        Dictionary<IDType, (string, string[])> factionsLocs = [];
        factionsLocs[chjenoPiast.ID] = (
            "Chjeno-Piast",
            [
                StringLineFormatter.Indent ("Coalition of Catholic hardliners and nationalists", 1),
                StringLineFormatter.Indent ("Right, vilified by labourers and non-Poles", 1),
            ]
        );
        factionsLocs[pps.ID] = (
            "Polish Socialist Party",
            [
                StringLineFormatter.Indent ("Historically prominent democratic socialist party", 1),
                StringLineFormatter.Indent ("Left, but increasingly fractured", 1),
            ]
        );
        factionsLocs[bmn.ID] = (
            "Bloc of National Minorities",
            [
                StringLineFormatter.Indent ("Minorities opposed to a united Polish state", 1),
                StringLineFormatter.Indent ("Generally left, but only out of necessity", 1),
            ]
        );
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[pps.ID] = "PPS";
        abbreviations[bmn.ID] = "BMN";

        ProcedureImmediate generalElection = new (
            0,
            [
                new (Procedure.Effect.EffectType.ElectionParty, []),
                new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister, bmn.ID]),
            ]
        );
        ProcedureImmediate article95 = new (
            1,
            [new (Procedure.Effect.EffectType.PermissionsVotes, [bmn.ID], 1)]
        );
        List<ProcedureImmediate> proceduresGovernmental = [generalElection, article95];
        ProcedureTargeted assassinationGabrielNarutowicz = new (
            2,
            [
                new (Procedure.Effect.EffectType.VotePassTwoThirds, []),
                new (Procedure.Effect.EffectType.PermissionsCanVote, [chjenoPiast.ID], 0),
            ],
            [1, 2]
        );
        ProcedureTargeted partitionedPoland = new (
            3,
            [new (Procedure.Effect.EffectType.ProcedureActivate, [generalElection.ID])],
            []
        );
        ProcedureTargeted ukrainianMilitaryOrganisation = new (
            4,
            [new (Procedure.Effect.EffectType.PermissionsCanSpeak, [], 0)],
            []
        );
        ProcedureTargeted moralNations = new (
            5,
            [new (Procedure.Effect.EffectType.PermissionsVotes, [chjenoPiast.ID], 2)],
            []
        );
        ProcedureTargeted railwaymensUnion = new (
            6,
            [
                new (Procedure.Effect.EffectType.ProcedureActivate, [generalElection.ID]),
                new (Procedure.Effect.EffectType.PermissionsVotes, [pps.ID], 1),
            ],
            [],
            false
        );
        ProcedureTargeted ukrainianMilitaryOrganisation2 = new (
            7,
            [
                new (Procedure.Effect.EffectType.PermissionsCanVote, [], 0),
            ],
            [],
            false
        );
        ProcedureTargeted ethnicInsurgents = new (
            8,
            [
                new (Procedure.Effect.EffectType.PermissionsCanVote, [], 0),
                new (Procedure.Effect.EffectType.PermissionsCanSpeak, [], 0),
            ],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            assassinationGabrielNarutowicz,
            partitionedPoland,
            ukrainianMilitaryOrganisation,
            moralNations,
            railwaymensUnion,
            ukrainianMilitaryOrganisation2,
            ethnicInsurgents
        ];
        ProcedureDeclared voteNoConfidence = new (
            9,
            [new (Procedure.Effect.EffectType.ElectionNominated, [primeMinister])],
            new Confirmation (Confirmation.ConfirmationType.DivisionChamber),
            []
        );
        List<ProcedureDeclared> proceduresDeclared = [voteNoConfidence];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[generalElection.ID] = (
            "General Election",
            "This country has a legislature with regular elections, along with an empowered head of government."
        );
        procedures[article95.ID] = (
            "Article 95",
            "The March Constitution explicitly grants equal rights to minority ethnonational and religious groups, who make up a third of the Polish population, but in practice this protection is rarely enforced and often actively opposed."
        );
        procedures[assassinationGabrielNarutowicz.ID] = (
            "Assassination of Gabriel Narutowicz",
            "Narutowicz was never a popular politician, being a centrist and sympathetic to Jews, but his assassination ruined the reputation of rightists in the eyes of many, especially Józef Piłsudski, the most powerful person in Poland."
        );
        procedures[partitionedPoland.ID] = (
            "Partitioned Poland",
            "Even after independence, the scars of the Partitions of Poland remain - there are obvious regional disparities in wealth, German trade and businesses dominate the economy, and there is no unified rail network, much less unified politics."
        );
        procedures[ukrainianMilitaryOrganisation.ID] = (
            "Ukrainian Military Organisation",
            "Ukrainians are generally opposed to living in a Polish state, and some took up arms against Poland in an attempt to become free. The Ukrainian Military Organisation is by far the most powerful of such insurgent groups."
        );
        procedures[moralNations.ID] = (
            "Moral Nations",
            "Everyone agrees that something is wrong with Polish society, but nobody knows what exactly - it reveals itself through religion, women's dress, sexual expression, and language, and there will be dire consequences if this problem is not solved."
        );
        procedures[railwaymensUnion.ID] = (
            "Railwaymen's Union",
            Localisation.UNUSED
        );
        procedures[ukrainianMilitaryOrganisation2.ID] = (
            "Ukrainian Military Organisation",
            Localisation.UNUSED
        );
        procedures[ethnicInsurgents.ID] = (
            "Ethnic Insurgents",
            Localisation.UNUSED
        );
        procedures[voteNoConfidence.ID] = (
            "Vote of No Confidence",
            "This country's head of government is responsible to the legislature, which means that he can be removed if he is seen to be inept."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [],
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
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [partitionedPoland.ID, railwaymensUnion.ID])],
                [new (new AlwaysCondition (), 2)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 2)]
            ),
            true
        );
        Ballot ballotB = new (
            2,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [ukrainianMilitaryOrganisation.ID, ukrainianMilitaryOrganisation2.ID])],

                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.EffectType.RemoveProcedure, [moralNations.ID])],
                [new (new AlwaysCondition (), 4)]
            )
        );
        Ballot incidentB = new (
            3,
            new Ballot.Result (
                [
                    new Ballot.Effect (Ballot.Effect.EffectType.DissolveParty, [bmn.ID]),
                    new Ballot.Effect (Ballot.Effect.EffectType.ReplaceProcedure, [ukrainianMilitaryOrganisation2.ID, ethnicInsurgents.ID]),
                ],
                [new (new AlwaysCondition (), 4)]
            ),
            new Ballot.Result (
                [],
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
            "Establish the Bank of Poland",
            [
                StringLineFormatter.Indent ("Poland has been suffering from hyperinflation", 1),
                StringLineFormatter.Indent ("Great War and Polish-Soviet War were extremely expensive", 2),
                StringLineFormatter.Indent ("There is little foreign investment, especially since Germany is Poland's largest trade partner", 2),
                StringLineFormatter.Indent ("Creating a national bank would involve issuing a new gold-backed currency and issuing shares to citizens", 1),
                StringLineFormatter.Indent ("This would limit inflation, but could also lead to severe price discrepancies", 2),
                StringLineFormatter.Indent ("This would lessen government control over monetary policy", 2),
            ],
            [
                "Bank of Poland is surprisingly successful",
                StringLineFormatter.Indent ("New currency reduces inflation dramatically", 1),
                StringLineFormatter.Indent ("More stocks are issued to match demand", 1),
                StringLineFormatter.Indent ("Foreign investors have much more confidence in the security of the Polish economy", 1),
                "Government now has an effective ally that can conduct monetary policy",
            ],
            [
                "Hyperinflation continues to run rampant",
                StringLineFormatter.Indent ("This badly hurts industrial growth", 1),
                StringLineFormatter.Indent ("Surprisingly, this unites workers and capitalists in opposition to the government", 2),
                StringLineFormatter.Indent ("Farmers benefit from high agricultural prices", 1),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Unite the rail networks",
            [
                StringLineFormatter.Indent ("Polish rail networks are made of separate rail networks inherited from the partitioning powers", 1),
                StringLineFormatter.Indent ("Western Poland has the best rail network, but it is broken by the new German border", 2),
                StringLineFormatter.Indent ("This would improve economic growth at a major cost to the treasury", 1),
                StringLineFormatter.Indent ("Farmers would benefit most from being able to easily ship their products west", 2),
                StringLineFormatter.Indent ("Additionally, the railway workers are unionised, which means that this would greatly strengthen the PPS", 2),
                StringLineFormatter.Indent ("This would also help decrease reliance on German trade", 2),
            ],
            [
                "Railroad building greatly improves the economy",
                StringLineFormatter.Indent ("Additionally, a new seaport in Gdynia is being expanded to bypass German trade routes", 1),
                StringLineFormatter.Indent ("However, this places railroad operation at the mercy of the PPS", 1),
            ],
            [
                "Divisions between western and eastern Poland become more and more pronounced",
                StringLineFormatter.Indent ("This also greatly weakens minority groups", 1),
                StringLineFormatter.Indent ("Their participation in the economy is dependent on railroads from southern and eastern Poland", 2),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Close minority schools",
            [
                StringLineFormatter.Indent ("One-third of the population is minorities", 1),
                StringLineFormatter.Indent ("Most are Jews or Ukrainians", 2),
                StringLineFormatter.Indent ("Significant portion is Germans, Lithuanians, and White Ruthenians", 2),
                StringLineFormatter.Indent ("Many of these minorities, especially Ukrainians, Germans, and Lithuanians, have national aspirations that cannot feasibly be crushed", 2),
                StringLineFormatter.Indent ("This would begin the process of Polonisation, assimilating the minorities", 1),
                StringLineFormatter.Indent ("This will be extremely unpopular among the minorities and could worsen insurgent activities", 2),
            ],
            [
                "Ukrainian insurgents increase their activities",
                StringLineFormatter.Indent ("Assassinations and bombings become commonplace in Galicia", 1),
                "Lithuania protests strongly",
                StringLineFormatter.Indent ("Fate of Lithuanian minorities could be used as a bargaining chip in future diplomatic relations", 1),
            ],
            [
                "Minority rights remain legally protected",
                StringLineFormatter.Indent ("This policy remains tenuous, but it is settled for now", 1),
                StringLineFormatter.Indent ("Ukrainian insurgents still want independence and remain in violent opposition", 1),
                StringLineFormatter.Indent ("Schools in Volhynia promote loyalty to Poland while also teaching Ukrainian", 1),
                StringLineFormatter.Indent ("Only time will tell if this policy will succeed", 2),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Dissolve minority organisations",
            [
                StringLineFormatter.Indent ("This would end minority participation in government", 1),
                StringLineFormatter.Indent ("Any loyalist Ukrainians are likely to join insurgent groups", 2),
                StringLineFormatter.Indent ("Lithuanians and White Ruthenians, who have so far been relatively quiet, are likely to create insurgent groups", 2),
                StringLineFormatter.Indent ("Germans are likely to bide their time until Germany invades", 2),
                StringLineFormatter.Indent ("If minorities aren't quickly assimilated, then Poland's existence will be at threat", 1),
                StringLineFormatter.Indent ("USSR, Lithuania, and Germany all want Polish land and may make use of minority plight to make demands", 2),
            ],
            [
                "Poland devolves into a low-intensity civil war",
                StringLineFormatter.Indent ("Allies consider abandoning Poland entirely", 1),
                StringLineFormatter.Indent ("British public opinion is overwhelmingly opposed to heavy-handed repression", 2),
                StringLineFormatter.Indent ("France worries that Poland has made too many enemies", 2),
            ],
            [
                "Polonisation cannot continue much further",
                StringLineFormatter.Indent ("Allowing minority organisations to persist guarantees that many minorities will maintain some aspects of their culture", 1),
                StringLineFormatter.Indent ("Polish allies are thankful that they will not need to come to Poland's aid in the near future", 1),
                StringLineFormatter.Indent ("However, diplomatic conditions are already set for war", 2),
            ]
        );
        ballotsLocs[ballotC.ID] = (
            "Ballot C",
            "Raise tariffs on German imports",
            [
                StringLineFormatter.Indent ("German businesses overwhelmingly dominate the Polish economy", 1),
                StringLineFormatter.Indent ("Raising tariffs would help limit their competitiveness while also bringing in additional revenue", 2),
                StringLineFormatter.Indent ("This state of affairs is extremely favourable for Germany, which doesn't need Polish trade", 2),
                StringLineFormatter.Indent ("This policy will likely be economically disastrous in the short-term, but could lead to better self-sufficiency in the long-term", 1),
                StringLineFormatter.Indent ("In any case, there are many potential trade partners nearby", 2),
            ],
            [
                "Trade war begins disastrously",
                StringLineFormatter.Indent ("Economy crashes due to German opposition", 1),
                StringLineFormatter.Indent ("Germany stops importing coal and steel, which were Poland's main industrial exports", 1),
                "Gradually, trade routes are established with other states",
                StringLineFormatter.Indent ("Italy and Czechoslovakia make up for some of the lost exports", 1),
                StringLineFormatter.Indent ("Economy survives for now, but a global recession could ruin this tenuous situation", 1),
            ],
            [
                "German economic dominance over Poland increases",
                StringLineFormatter.Indent ("Many prominent German newspapers explicitly propose using economic leverage to demand territorial cessions", 1),
                StringLineFormatter.Indent ("If Germany cannot be appeased, then war is very likely", 2),
            ]
        );

        Result resultStart = new (
            0,
            [
                new (new BallotPassedCondition (ballotB.ID, true), 1),
                new (new BallotPassedCondition (ballotB.ID, false), 10),
            ]
        );
        Result ballotBPassed = new (
            1,
            [
                new (new BallotPassedCondition (ballotA.ID, true), 2),
                new (new BallotPassedCondition (ballotA.ID, false), 9),
            ]
        );
        Result ballotAPassed = new (
            2,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 3),
                new (new BallotPassedCondition (incidentA.ID, false), 8),
            ]
        );
        Result incidentAPassed = new (
            3,
            [
                new (new BallotPassedCondition (incidentB.ID, true), 4),
                new (new BallotPassedCondition (incidentB.ID, false), 5),
            ]
        );
        Result incidentBPassed = new (
            4,
            []
        );
        Result incidentBFailed = new (
            5,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 6),
                new (new BallotPassedCondition (ballotC.ID, false), 7),
            ]
        );
        Result ballotCPassed = new (
            6,
            []
        );
        Result ballotCFailed = new (
            7,
            []
        );
        Result incidentAFailed = new (
            8,
            []
        );
        Result ballotAFailed = new (
            9,
            [
                new (new BallotPassedCondition (incidentB.ID, true), 4),
                new (new BallotPassedCondition (incidentB.ID, false), 5),
            ]
        );
        Result ballotBFailed = new (
            10,
            [
                new (new BallotPassedCondition (incidentA.ID, true), 11),
                new (new BallotPassedCondition (incidentA.ID, false), 14),
            ]
        );
        Result incidentAPassed2 = new (
            11,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 12),
                new (new BallotPassedCondition (ballotC.ID, false), 13),
            ]
        );
        Result ballotCPassed2 = new (
            12,
            []
        );
        Result ballotCFailed2 = new (
            13,
            []
        );
        Result incidentAFailed2 = new (
            14,
            [
                new (new BallotPassedCondition (ballotC.ID, true), 15),
                new (new BallotPassedCondition (ballotC.ID, false), 16),
            ]
        );
        Result ballotCPassed3 = new (
            15,
            []
        );
        Result ballotCFailed3 = new (
            16,
            []
        );
        List<Result> results = [
            resultStart,
            ballotBPassed,
            ballotAPassed,
            incidentAPassed,
            incidentBPassed,
            incidentBFailed,
            ballotCPassed,
            ballotCFailed,
            incidentAFailed,
            ballotAFailed,
            ballotBFailed,
            incidentAPassed2,
            ballotCPassed2,
            ballotCFailed2,
            incidentAFailed2,
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
                "Piłsudski is angered by the lack of consensus in Polish society",
                StringLineFormatter.Indent ("Most leftists agree with him", 1),
                StringLineFormatter.Indent ("Most of the Warsaw garrison defects and joins Piłsudski, who demands the resignation of the government", 1),
            ]
        );
        resultsLocs[ballotAPassed.ID] = (
            "Ballot A Passed",
            [
                "Due to generally satisfactory policy by the rightists, some soldiers oppose Piłsudski",
                StringLineFormatter.Indent ("Fighting breaks out and a civil war seems inevitable", 1),
            ]
        );
        resultsLocs[incidentAPassed.ID] = (
            "Incident A Passed",
            [
                "PPS declares a general strike and the railways are paralysed",
                StringLineFormatter.Indent ("Since reinforcements won't arrive anytime soon, the rightists surrender", 1),
                "Piłsudski is now a dictator",
                StringLineFormatter.Indent ("Due to deep societal conflict, Piłsudski will have to tread carefully to avoid alienating his supporters", 1),
            ]
        );
        resultsLocs[incidentBPassed.ID] = (
            "Incident B Passed",
            [
                "Southern and eastern Poland are under a permanent state of siege",
                StringLineFormatter.Indent ("Constant reprisals against minority insurgents only inflame the conflict more", 1),
                StringLineFormatter.Indent ("Polonisation efforts can be stopped, but not reversed, while minorities are openly warring against the state", 1),
                "Poland's internal conflict makes any external conflict much more deadly",
                StringLineFormatter.Indent ("Current conflict with Germany has passed, but the next will guarantee the end of Poland", 1),
            ]
        );
        resultsLocs[incidentBFailed.ID] = (
            "Incident B Failed",
            [
                "Southern and eastern Poland are under a state of siege",
                StringLineFormatter.Indent ("Over time, some minorities grow to tolerate Polish rule and some Polonisation policies are reversed", 1),
                StringLineFormatter.Indent ("However, Piłsudski's dream of a multi-ethnic, tolerant Poland will never come to fruition", 1),
            ]
        );
        resultsLocs[ballotCPassed.ID] = (
            "Ballot C Passed",
            [
                "Poland slowly grows in self-sufficiency, until the Great Depression arrives",
                StringLineFormatter.Indent ("Luckily, previous economic policy prevents some classes from being hit as hard as other classes", 1),
                StringLineFormatter.Indent ("Piłsudski is slow to implement reforms to address this", 2),
                "Class conflict eventually ends, but minority opposition remains dangerous",
                StringLineFormatter.Indent ("Poland is in a precarious situation, but reliable allies could prevent Polish independence from being threatened again", 1),
            ]
        );
        resultsLocs[ballotCFailed.ID] = (
            "Ballot C Failed",
            [
                "Continued reliance on Germany is deeply distressing",
                StringLineFormatter.Indent ("Once the Great Depression hits, the economy collapses due to lack of German trade", 1),
                StringLineFormatter.Indent ("No previous economic policy could possibly have saved Poland from the Great Depression", 2),
                StringLineFormatter.Indent ("Piłsudski is also unsure of how to solve this problem", 2),
                "No neighbouring states are willing to help Poland recover",
                StringLineFormatter.Indent ("Poland won't be able to defend itself in the future", 1),
            ]
        );
        resultsLocs[incidentAFailed.ID] = (
            "Incident A Failed",
            [
                "Loyalist reinforcements arrive and the rightists continue to refuse to back down",
                StringLineFormatter.Indent ("Piłsudski also refuses to back down and civil war breaks out", 1),
                "With two evenly matched opponents, this civil war will not end anytime soon",
                StringLineFormatter.Indent ("Poland will most likely never recover from this conflict", 1),
                StringLineFormatter.Indent ("Ethnic insurgents revolt for independence", 1),
                StringLineFormatter.Indent ("USSR, Lithuania, and Germany consider intervening to secure their land claims", 1),
            ]
        );
        resultsLocs[ballotAFailed.ID] = (
            "Ballot A Failed",
            [
                "Without much military opposition, Piłsudski seizes control of Warsaw quickly",
                "Piłsudski is now a dictator",
                StringLineFormatter.Indent ("Due to his quick and easy seizure of power, Piłsudski can easily direct policy as he sees fit", 1),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Piłsudski cautiously accepts the government",
                StringLineFormatter.Indent ("His dream of a multi-ethnic, tolerant Poland may be possible in the future, since ethnoreligious rights have been protected", 1),
            ]
        );
        resultsLocs[incidentAPassed2.ID] = (
            "Incident A Passed",
            [
                "Railroads help decrease severe wealth differences across Poland",
                StringLineFormatter.Indent ("Additionally, economic opportunity helps to soothe minority discontent", 1),
                StringLineFormatter.Indent ("This endeavour was very expensive and Poland will remain vulnerable to outside pressures without allies", 1),
            ]
        );
        resultsLocs[ballotCPassed2.ID] = (
            "Ballot C Passed",
            [
                "New trade routes and political protections help to protect Poland from Germany",
                StringLineFormatter.Indent ("However, whether these allies will truly be reliable in the future remains to be seen", 1),
                "Overall, Poland is doing very well",
                StringLineFormatter.Indent ("Unless a major political realignment takes place, which is very possible, Poland should not have to worry about internal conflict much", 1),
                StringLineFormatter.Indent ("External conflict may be devastating, but Poland will at least have a fighting chance", 1),
            ]
        );
        resultsLocs[ballotCFailed2.ID] = (
            "Ballot C Failed",
            [
                "Despite all of Poland's advancements, reliance on Germany remains a dangerous weakness",
                StringLineFormatter.Indent ("Lack of reliable allies also contributes to this reliance", 1),
                "Great Depression crashes the Polish economy",
                StringLineFormatter.Indent ("Luckily, this does not stoke too much internal conflict, but this is still extremely concerning", 1),
                StringLineFormatter.Indent ("In this weakened state, Poland will have to recover quickly if she wants to protect her independence", 1),
            ]
        );
        resultsLocs[incidentAFailed2.ID] = (
            "Incident A Failed",
            [
                "Lack of significant economic investment proves dangerous in the face of external opposition",
                StringLineFormatter.Indent ("USSR, Lithuania, and Germany spread anti-Polish propaganda, with varying degrees of success", 1),
                StringLineFormatter.Indent ("Minorities may not be living well, but they're not being oppressed", 2),
                StringLineFormatter.Indent ("This could change in the future", 2),
            ]
        );
        resultsLocs[ballotCPassed3.ID] = (
            "Ballot C Passed",
            [
                "Trade war significantly heightens German opposition without a reasonable way out",
                StringLineFormatter.Indent ("Some economic reforms are quickly enacted, but the initial impact has already hit and Poland will not recover for some time", 1),
                "Great Depression crashes the Polish economy",
                StringLineFormatter.Indent ("Some regions are hit much worse than others, which fuels significant resentment", 1),
                StringLineFormatter.Indent ("Poland will probably not recover quickly enough to protect its independence", 1),
            ]
        );
        resultsLocs[ballotCFailed3.ID] = (
            "Ballot C Failed",
            [
                "Germans become more and more powerful",
                StringLineFormatter.Indent ("Not only is the economy largely controlled by Germans, but their rights are protected as well", 1),
                StringLineFormatter.Indent ("This means that Poles now cannot afford to anger Germany too much, unless they want Poland to collapse", 2),
                "Great Depression crashes the Polish economy",
                StringLineFormatter.Indent ("Recovery is largely in German hands, but Germany's weakness makes this a chance to stop this pseudo-puppet state situation", 1),
                StringLineFormatter.Indent ("Whether Poland's allies are reliable enough to help Poland break away is uncertain", 1),
            ]
        );

        History history = new (
            [ballotA.ID, incidentA.ID, ballotB.ID, ballotC.ID],
            []
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
                "Poland",
                "Parliamentary Republic",
                [
                    "Poland gained its independence from Russia and Germany in the aftermath of the Great War",
                    StringLineFormatter.Indent ("After defeating the USSR in the Polish-Soviet War, Polish society became badly divided", 1),
                    StringLineFormatter.Indent ("Ethnic and religious minorities were all extremely concerned for their future in an increasingly right-wing Poland", 2),
                    StringLineFormatter.Indent ("Polish independence was spearheaded by leftists, who held significant power through the army", 2),
                    StringLineFormatter.Indent ("This includes Józef Piłsudski, who led the war against the USSR and has since retired", 3),
                    StringLineFormatter.Indent ("After the elections of 1922, the new centrist president was assassinated by rightists", 1),
                    StringLineFormatter.Indent ("This upset the delicate balance of power holding Poland together", 2),
                ],
                "19 December 1923",
                "Ascension of Władysław Grabski",
                "Parliamentocracy",
                rolesLocs,
                "Marshal",
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
