using congress_cucuta.Converters;
using congress_cucuta.Data;

namespace congress_cucuta.Simulations;

internal class Japan : ISimulation {
    public Simulation Simulation { get; }

    public Japan () {
        IDType governor = Role.MEMBER;
        List<IDType> roles = [
            governor,
            0,
            1,
            2,
            Role.LEADER_PARTY,
        ];
        Dictionary<IDType, (string, string)> rolesLocs = [];
        rolesLocs[governor] = ("Governor", "Governors");
        rolesLocs[0] = ("Shogun", "Shoguns");
        rolesLocs[1] = ("Emperor", "Emperors");
        rolesLocs[2] = ("Regent", "Regents");
        rolesLocs[Role.LEADER_PARTY] = ("Sovereign", "Sovereigns");

        Faction kyoto = new (0);
        Faction yoshino = new (1);
        Faction kamakura = new (2, false);
        List<Faction> courts = [kyoto, yoshino, kamakura];
        Dictionary<IDType, (string, string[])> courtsLocs = [];
        courtsLocs[kyoto.ID] = (
            "Northern Court",
            [
                StringLineFormatter.Indent ("Military faction", 1),
                StringLineFormatter.Indent ("Rejects civilian control, but respects tradition and culture", 1),
                StringLineFormatter.Indent ("Warriors are more numerous and powerful than the nobility", 2),
                StringLineFormatter.Indent ("Promotes martial values", 2),
            ]
        );
        courtsLocs[yoshino.ID] = (
            "Southern Court",
            [
                StringLineFormatter.Indent ("Imperial faction", 1),
                StringLineFormatter.Indent ("Wants to re-establish imperial and civilian rule", 1),
                StringLineFormatter.Indent ("This would increase the power of the nobility and the clergy", 2),
                StringLineFormatter.Indent ("Promotes cultural development", 2),
            ]
        );
        courtsLocs[kamakura.ID] = (
            "Eastern Court",
            []
        );
        Dictionary<IDType, string> abbreviations = [];
        abbreviations[kyoto.ID] = "Kyoto";
        abbreviations[yoshino.ID] = "Yoshino";
        abbreviations[kamakura.ID] = "Kamakura";

        Dictionary<IDType, sbyte> currenciesValues = [];
        IDType provincesKyoto = 0;
        IDType provincesYoshino = 1;
        IDType provincesKamakura = 2;
        currenciesValues[provincesKyoto] = 4;
        currenciesValues[provincesYoshino] = 2;
        currenciesValues[provincesKamakura] = 2;
        Dictionary<IDType, string> currencies = [];
        currencies[provincesKyoto] = "Province";
        currencies[provincesYoshino] = "Province";
        currencies[provincesKamakura] = "Province";
        currencies[Currency.PARTY] = "Province";

        ProcedureImmediate hereditarySuccession = new (
            0,
            [
                new (Procedure.Effect.ActionType.ElectionParty, [], 1),
            ]
        );
        List<ProcedureImmediate> proceduresGovernmental = [hereditarySuccession];
        ProcedureTargeted feudalSociety = new (
            1,
            [new (Procedure.Effect.ActionType.CurrencyInitialise, [])],
            []
        );
        ProcedureTargeted imperialPretenders = new (
            2,
            [new (Procedure.Effect.ActionType.ProcedureActivate, [hereditarySuccession.ID])],
            [2, 3, 4]
        );
        ProcedureTargeted threeSacredTreasures = new (
            3,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [provincesYoshino], 1)],
            []
        );
        ProcedureTargeted seiwaMinamotoClan = new (
            4,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [provincesKyoto], 1)],
            []
        );
        ProcedureTargeted threeSacredTreasures2 = new (
            5,
            [new (Procedure.Effect.ActionType.CurrencyAdd, [provincesYoshino], 2)],
            [],
            false
        );
        List<ProcedureTargeted> proceduresSpecial = [
            feudalSociety,
            imperialPretenders,
            threeSacredTreasures,
            seiwaMinamotoClan,
            threeSacredTreasures2,
        ];
        ProcedureDeclared civilWar = new (
            6,
            [
                new (Procedure.Effect.ActionType.BallotLimit, []),
            ],
            new Procedure.Confirmation (Procedure.Confirmation.CostType.DiceAdversarial),
            0,
            [Role.LEADER_PARTY]
        );
        List<ProcedureDeclared> proceduresDeclared = [civilWar];
        Dictionary<IDType, (string, string)> procedures = [];
        procedures[hereditarySuccession.ID] = (
            "Hereditary Succession",
            "This country's head of state is chosen based on familial lineage."
        );
        procedures[feudalSociety.ID] = (
            "Feudal Society",
            "This country's government rules through vassals, who have military and administrative authority over large swathes of land."
        );
        procedures[imperialPretenders.ID] = (
            "Imperial Pretenders",
            "Japan is currently split into two factions led by opposing emperors, both of which claim legitimate rulership over Japan. This conflict has divided Japan so badly that it will take at least a few generations to conclude."
        );
        procedures[threeSacredTreasures.ID] = (
            "Three Sacred Treasures",
            "The Three Sacred Treasures represent the divinity of the emperor and are currently in the Southern Court's posession, which makes the Southern emperor the legitimate emperor of Japan."
        );
        procedures[seiwaMinamotoClan.ID] = (
            "Seiwa Minamoto Clan",
            "The current shogun is a descendent of emperor Seiwa, whose scions form a branch of the legendary Minamoto warrior clan. This grants his line a unique martial authority separate from that of the emperor."
        );
        procedures[threeSacredTreasures2.ID] = (
            "Three Sacred Treasures",
            Localisation.UNUSED
        );
        procedures[civilWar.ID] = (
            "Civil War",
            "The conflict between the Northern and Southern Courts has spilled into open warfare and will likely continue to do so in the future until one faction submits."
        );

        Ballot ballotA = new (
            0,
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 1)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.FoundParty, [kamakura.ID])],
                [new (new AlwaysCondition (), 2)]
            )
        );
        Ballot incidentA = new (
            1,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ReplaceProcedure, [threeSacredTreasures.ID, threeSacredTreasures2.ID])],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 2)]
            ),
            true
        );
        Ballot incidentB = new (
            2,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [provincesYoshino], 1)],
                [new (new AlwaysCondition (), 3)]
            ),
            new Ballot.Result (
                [],
                [new (new AlwaysCondition (), 4)]
            ),
            true
        );
        Ballot incidentC = new (
            3,
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [provincesKyoto], 1)],
                [new (new AlwaysCondition (), 4)]
            ),
            new Ballot.Result (
                [new Ballot.Effect (Ballot.Effect.ActionType.ModifyCurrency, [provincesKyoto, provincesYoshino], -1)],
                [new (new AlwaysCondition (), 4)]
            ),
            true
        );
        Ballot ballotB = new (
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
        List<Ballot> ballots = [ballotA, incidentA, incidentB, incidentC, ballotB];
        Dictionary<IDType, (string, string, string[], string[], string[])> ballotsLocs = [];
        ballotsLocs[ballotA.ID] = (
            "Ballot A",
            "Expand the half-tax",
            [
                StringLineFormatter.Indent ("Half-tax forced some land owners to give half of their revenue to the shogun to fund his rebellion", 1),
                StringLineFormatter.Indent ("Expanding the half-tax would force all estate-owners to give half of their land to the shogun", 1),
                StringLineFormatter.Indent ("Most estate-owners are nobles", 2),
                StringLineFormatter.Indent ("However, many governors and warriors illegally seized estates to support the shogun's rebellion", 3),
                StringLineFormatter.Indent ("Shogun will reward his warriors with land for their loyalty", 2),
                StringLineFormatter.Indent ("Warriors revolted against the emperor because they weren't rewarded", 3),
                StringLineFormatter.Indent ("This could allow the warriors to create independent power bases", 3),
            ],
            [
                "Experienced bureaucrats believe that expanding the warriors' power could destroy the regime",
                StringLineFormatter.Indent ("Some nobles are planning to defect to the emperor", 1),
                StringLineFormatter.Indent ("For now, personal loyalty to the shogun prevents too much independent action", 1),
            ],
            [
                "Shogun has lost the support of the warriors",
                StringLineFormatter.Indent ("At the same time, the warriors don't trust the emperor, who also betrayed their expectations", 1),
                StringLineFormatter.Indent ("Powerful lords and warriors in Kanto join together to establish a rival military regime", 1),
            ]
        );
        ballotsLocs[incidentA.ID] = (
            "Incident A",
            "Create military governorships",
            [
                StringLineFormatter.Indent ("This would increase the involvement of the warriors in the bureaucracy", 1),
                StringLineFormatter.Indent ("This would also be considered a reward for loyalty and further legitimise the land grants from the expanded half-tax", 2),
                StringLineFormatter.Indent ("This is heavily opposed by the nobles and the clergy", 1),
                StringLineFormatter.Indent ("They don't believe that illiterate warmongers could be considered their superiors or even equals", 2),
                StringLineFormatter.Indent ("Even the peasants prefer noble overlords to warrior overlords", 3),
                StringLineFormatter.Indent ("This would also reduce the shogun's reliance on civilian powerholders", 2),
                StringLineFormatter.Indent ("Shogun wouldn't need imperial legitimacy to justify his reign", 3),
            ],
            [
                "Regime loses most civilian support",
                StringLineFormatter.Indent ("Defectors flock to the Southern Court and agitate for an immediate offensive on Kyoto", 1),
                StringLineFormatter.Indent ("Even members of the shogun's family believe that they would be better off supporting the emperor", 2),
                "Most warriors and governors remain loyal due to personal or familial ties to the shogun",
                StringLineFormatter.Indent ("However, their loyalty can't be guaranteed in the long-term", 1),
            ],
            [
                "Most warriors have no reason to rebel",
                StringLineFormatter.Indent ("They already gained land through the half-tax", 1),
                StringLineFormatter.Indent ("Some governors gradually take over the land of incapable warriors", 2),
                "Preventing further incursions upon civilian power somewhat appeases the governors",
                StringLineFormatter.Indent ("However, there is always the threat of the warriors gaining strength in the future", 1),
            ]
        );
        ballotsLocs[incidentB.ID] = (
            "Incident B",
            "Militarise the nobles",
            [
                StringLineFormatter.Indent ("This would force the nobles to provide military service", 1),
                StringLineFormatter.Indent ("Nobles would have to learn to fight and lead armies", 2),
                StringLineFormatter.Indent ("This would prevent the nobles from needing to rely on warriors to protect their holdings", 3),
                StringLineFormatter.Indent ("Therefore, a civilian regime could be possible", 4),
                StringLineFormatter.Indent ("This would restore some power to the weak nobles", 3),
                StringLineFormatter.Indent ("This will infuriate the warriors", 2),
                StringLineFormatter.Indent ("They currently enjoy privileged status thanks to their martial importance", 3),
                StringLineFormatter.Indent ("However, they're also uneducated and obstructive in times of peace", 4),
            ],
            [
                "Though this is a long-term programme, some results are already apparent",
                StringLineFormatter.Indent ("Nobles are nowhere near as powerful, useful, or numerous as the warriors", 1),
                StringLineFormatter.Indent ("However, developing a separate power base does strengthen the regime", 2),
                StringLineFormatter.Indent ("Many warriors feel threatened, leading to sporadic revolts throughout the country", 1),
            ],
            [
                "Nobles question the regime's commitment to civilian rule",
                StringLineFormatter.Indent ("Without empowering the nobles, there is no plausible way to break the power of the warriors", 1),
                "Warriors retain their dominant position in society",
                StringLineFormatter.Indent ("However, many would like more power, which has so far been denied to them", 1),
                StringLineFormatter.Indent ("Therefore, the regime is seen as stagnant", 2),
                StringLineFormatter.Indent ("Some warriors prepare for another rebellion in order to expand their powers", 2),
            ]
        );
        ballotsLocs[incidentC.ID] = (
            "Incident C",
            "Establish the Deputy's Council",
            [
                StringLineFormatter.Indent ("Deputy's Council would manage relations between the shogun and the governors", 1),
                StringLineFormatter.Indent ("Many governorships have been usurped by warriors", 1),
                StringLineFormatter.Indent ("Therefore, this would combine the military and civilian aspects of the bureaucracy", 2),
                StringLineFormatter.Indent ("This will improve the standing of the regime in the eyes of the peasants", 3),
                StringLineFormatter.Indent ("This will weaken the governors", 1),
                StringLineFormatter.Indent ("Settling any conflict outside of the council would be considered treason", 2),
                StringLineFormatter.Indent ("By making war a last resort, the shogun can expand his influence through his deputy without actually needing a strong army", 3),
            ],
            [
                "Governors become more loyal to the regime",
                StringLineFormatter.Indent ("They prefer to gain power through participation in government than through conquest", 1),
                "Some clans have become alarmingly powerful",
                StringLineFormatter.Indent ("They hold powerful positions on the Deputy's Council or aren't subject to its jurisdiction", 1),
                StringLineFormatter.Indent ("They use their authority to build large power bases", 2),
            ],
            [
                "Governors engage in regional conflicts",
                StringLineFormatter.Indent ("Some governors have become very powerful, but there is no legal mechanism to punish them", 1),
                StringLineFormatter.Indent ("As a result, the shogun and his allies are forced to constantly attack exceptionally powerful governors", 2),
                StringLineFormatter.Indent ("This stops any single governor from becoming too powerful", 3),
                StringLineFormatter.Indent ("Over time, this leads to poverty in Japan", 1),
                StringLineFormatter.Indent ("Constant warfare is extremely bad for civilians", 2),
                StringLineFormatter.Indent ("Southern Court gains popularity among the peasants", 3),
            ]
        );
        ballotsLocs[ballotB.ID] = (
            "Ballot B",
            "Adopt a compulsory residence policy",
            [
                StringLineFormatter.Indent ("This would force every governor to live in Kyoto", 1),
                StringLineFormatter.Indent ("If a governor must leave to put down a rebellion, then he must provide a hostage in exchange", 2),
                StringLineFormatter.Indent ("This would dramatically weaken the governors", 2),
                StringLineFormatter.Indent ("Governors would no longer be able to personally govern", 3),
                StringLineFormatter.Indent ("They would need to appoint someone to govern in their names", 4),
                StringLineFormatter.Indent ("Governors would be forced to maintain an extra residence", 3),
                StringLineFormatter.Indent ("This would decrease their wealth significantly", 4),
                StringLineFormatter.Indent ("However, mansion-building in Kyoto is fashionable anyway", 4),
                StringLineFormatter.Indent ("Provincial strongmen could take over governance", 3),
                StringLineFormatter.Indent ("This includes the warriors who were not given governorships", 4),
            ],
            ["Intentionally left blank"],
            ["Intentionally left blank"]
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
                new (new BallotPassedCondition (ballotA.ID, true), 2),
                new (new BallotPassedCondition (ballotA.ID, false), 5),
            ]
        );
        Result ballotAPassed = new (
            2,
            [
                new (new BallotPassedCondition (incidentC.ID, true), 3),
                new (new BallotPassedCondition (incidentC.ID, false), 4),
            ]
        );
        Result incidentCPassed = new (
            3,
            []
        );
        Result incidentCFailed = new (
            4,
            []
        );
        Result ballotAFailed = new (
            5,
            [
                new (new BallotPassedCondition (incidentB.ID, true), 6),
                new (new BallotPassedCondition (incidentB.ID, false), 7),
            ]
        );
        Result incidentBPassed = new (
            6,
            []
        );
        Result incidentBFailed = new (
            7,
            [
                new (new BallotPassedCondition (incidentC.ID, true), 3),
                new (new BallotPassedCondition (incidentC.ID, false), 4),
            ]
        );
        Result ballotBFailed = new (
            8,
            []
        );
        List<Result> results = [
            resultStart,
            ballotBPassed,
            ballotAPassed,
            incidentCPassed,
            incidentCFailed,
            ballotAFailed,
            incidentBPassed,
            incidentBFailed,
            ballotBFailed,
        ];
        Dictionary<IDType, (string, string[])> resultsLocs = [];
        resultsLocs[resultStart.ID] = (
            "Results",
            ["Intentionally left blank"]
        );
        resultsLocs[ballotBPassed.ID] = (
            "Ballot B Passed",
            [
                "Government has guaranteed some degree of long-term stability",
                StringLineFormatter.Indent ("However, whether another class replaces the governors or the governors resurge in power is unknown", 1),
            ]
        );
        resultsLocs[ballotAPassed.ID] = (
            "Ballot A Passed",
            [
                "Warrior class has been elevated at the expense of the nobles",
                StringLineFormatter.Indent ("In the long-term, this will probably lead to severe instability", 1),
                StringLineFormatter.Indent ("However, with proper vassal management, the half-tax could also be a tool to stabilise the regime", 2),
            ]
        );
        resultsLocs[incidentCPassed.ID] = (
            "Incident C Passed",
            [
                "Regime has a strong support base",
                StringLineFormatter.Indent ("Strengthening the warriors at the expense of the nobles was a net benefit", 1),
                StringLineFormatter.Indent ("However, by weakening the governors as well, the regime has made several enemies", 1),
                "Japan remains stable with a number of localised conflicts mostly caused by upstart governors",
                StringLineFormatter.Indent ("However, as time passes, loyalty ties will be forgotten and Japan could be destroyed in a civil war", 1),
                StringLineFormatter.Indent ("Regime is too reliant on the support of the warriors", 2),
            ]
        );
        resultsLocs[incidentCFailed.ID] = (
            "Incident C Failed",
            [
                "In the short-term, the regime is much more unstable",
                StringLineFormatter.Indent ("Without any legal structure to contain their power, many governors revolt or are pre-emptively suppressed", 1),
                StringLineFormatter.Indent ("However, every failed revolt strengthens the regime and its supporters further", 2),
                StringLineFormatter.Indent ("Eventually, the power of the governors is broken and assumed by the government", 1),
                StringLineFormatter.Indent ("Though still unstable, Japan would need a severe crisis to collapse entirely", 2),
            ]
        );
        resultsLocs[ballotAFailed.ID] = (
            "Ballot A Failed",
            [
                "Backlash against the shogun leads to a more civilian-led regime",
                StringLineFormatter.Indent ("Whether the government is truly committed to civilian rule, especially in the long-term, is unknown", 1),
            ]
        );
        resultsLocs[incidentBPassed.ID] = (
            "Incident B Passed",
            [
                "Government consolidates its rule",
                StringLineFormatter.Indent ("Many consider this to be a revival of the Heian Period", 1),
                StringLineFormatter.Indent ("However, there's too much conflict to make cultural accomplishments of similar magnitude", 2),
                "Despite the weakening of the warriors, the governors continue to resist central rule",
                StringLineFormatter.Indent ("Governors are actually more powerful", 1),
                StringLineFormatter.Indent ("Warriors are no longer a credible threat to their rule", 2),
                StringLineFormatter.Indent ("Japan no longer faces an ideological split", 1),
                StringLineFormatter.Indent ("If the governors can be reigned in, then the government may be able to centralise governance entirely", 2),
            ]
        );
        resultsLocs[incidentBFailed.ID] = (
            "Incident B Failed",
            [
                "Eventually, the warriors are able to restore their influence in government",
                StringLineFormatter.Indent ("This essentially returns Japan to its state immediately after the end of the Kenmu Restoration", 1),
            ]
        );
        resultsLocs[ballotBFailed.ID] = (
            "Ballot B Failed",
            [
                "Government is destined to collapse",
                StringLineFormatter.Indent ("There is no way to guarantee stability in the face of ambitious and capable governors", 1),
                StringLineFormatter.Indent ("Eventually, Japan enters a prolonged period of civil war, known as the Warring States Period", 1),
            ]
        );

        Dictionary<IDType, SortedSet<IDType>> ballotsProceduresDeclared = [];
        ballotsProceduresDeclared[incidentA.ID] = [civilWar.ID];
        ballotsProceduresDeclared[incidentC.ID] = [civilWar.ID];
        ballotsProceduresDeclared[ballotB.ID] = [civilWar.ID];
        History history = new (
            [ballotA.ID, incidentA.ID, incidentC.ID, ballotB.ID],
            ballotsProceduresDeclared
        );

        Simulation = new (
            history,
            roles,
            [],
            courts,
            currenciesValues,
            proceduresGovernmental,
            proceduresSpecial,
            proceduresDeclared,
            ballots,
            results,
            new Localisation (
                "Japan",
                "Feudal Monarchy",
                [
                    "Kamakura Shogunate was dominated by the Hojo clan",
                    StringLineFormatter.Indent ("Daigo II and many governors were tired of Hojo rule", 1),
                    StringLineFormatter.Indent ("Daigo II enlisted the aid of Takauji Ashikaga to overthrow the Kamakura Shogunate, leading to the Kenmu Restoration", 2),
                    StringLineFormatter.Indent ("Daigo II failed to properly reward his allies", 1),
                    StringLineFormatter.Indent ("Takauji overthrew Daigo II and established the Ashikaga Shogunate in Kyoto", 2),
                    StringLineFormatter.Indent ("Daigo II fled to Yoshino, where he established the Southern Court to continue opposing Takauji", 3),
                    StringLineFormatter.Indent ("Takauji appointed a puppet emperor to lead the Northern Court", 3),
                ],
                "19 September 1339",
                "Death of Daigo II",
                "The Northern and Southern Courts",
                rolesLocs,
                "Deputy",
                (Localisation.UNUSED, Localisation.UNUSED),
                [],
                ("Court", "Courts"),
                courtsLocs,
                abbreviations,
                currencies,
                procedures,
                ballotsLocs,
                resultsLocs
            )
        );
    }
}
