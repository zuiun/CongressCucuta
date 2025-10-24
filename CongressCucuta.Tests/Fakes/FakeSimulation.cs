using CongressCucuta.Core;
using CongressCucuta.Core.Conditions;
using CongressCucuta.Core.Procedures;

namespace CongressCucuta.Tests.Fakes;

public class FakeSimulation () : Simulation (
    new ([0], new Dictionary<IDType, SortedSet<IDType>> { [1] = [3] }),
    [0, 1, 2, 3, Role.LEADER_REGION, Role.LEADER_PARTY, Role.HEAD_STATE, Role.HEAD_GOVERNMENT, Role.MEMBER],
    [new (0), new (1)],
    [new (2), new (3)],
    new () {
        [0] = 1,
        [1] = 1,
        [2] = 1,
        [3] = 1,
        [Currency.STATE] = 1,
    },
    [new (0, [new (Procedure.Effect.EffectType.PermissionsCanVote, [Role.MEMBER], 1)])],
    [
        new (1, [new (Procedure.Effect.EffectType.CurrencyInitialise, [])], []),
        new (2, [new (Procedure.Effect.EffectType.VotePassAdd, [], 1)], [], false),
    ],
    [new (3, [new (Procedure.Effect.EffectType.BallotPass, [])], new (Confirmation.ConfirmationType.Always), [])],
    [
        new (
            0,
            new ([new (Ballot.Effect.EffectType.ReplaceProcedure, [1, 2])], [new (new AlwaysCondition (), 1)]),
            new ([], [new (new AlwaysCondition (), Ballot.END)])
        ),
        new (1, new ([], []), new ([], []), true),
    ],
    [new (0, [])],
    FakeLocalisation.Create ()
);
