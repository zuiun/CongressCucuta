namespace CongressCucuta.Core.Contexts;

public class ResetVotesEventArgs (BallotContext context) {
    public byte VotesPass = context.CalculateVotesPass ();
    public byte VotesFail = context.CalculateVotesFail ();
    public byte VotesAbstain = context.CalculateVotesAbstain ();
    public byte VotesPassThreshold = context.CalculateVotesPassThreshold ();
    public byte VotesFailThreshold = context.CalculateVotesFailThreshold ();
}

public class BallotContext {
    // Replaced as necessary (every ballot, upon VotersLimit declared, upon role change)
    public Dictionary<IDType, Permissions> PeoplePermissions { get; set; } = [];
    public List<IDType> VotesPass { get; } = [];
    public List<IDType> VotesFail { get; } = [];
    public byte VotesPassBonus { get; set; } = 0;
    public byte VotesFailBonus { get; set; } = 0;
    public bool IsSimpleMajority { get; set; } = true;
    public List<IDType> ProceduresDeclared = [];
    public event Action<ResetVotesEventArgs>? ResetVotes;

    public void Reset () {
        VotesPass.Clear ();
        VotesFail.Clear ();
        ProceduresDeclared.Clear ();
        VotesPassBonus = 0;
        VotesFailBonus = 0;
    }

    // For ProcedureDeclared
    public void OnResetVotes () {
        VotesPass.Clear ();
        VotesFail.Clear ();

        ResetVotesEventArgs args = new (this);

        ResetVotes?.Invoke (args);
    }

    public byte CalculateVotesTotal () {
        byte votes = (byte) (VotesPassBonus + VotesFailBonus);

        foreach (Permissions p in PeoplePermissions.Values) {
            if (p.CanVote) {
                votes += p.Votes;
            }
        }

        return votes;
    }

    public byte CalculateVotesPass () {
        byte passCount = VotesPassBonus;

        foreach (IDType p in VotesPass) {
            passCount += PeoplePermissions[p].Votes;
        }

        return passCount;
    }

    public byte CalculateVotesFail () {
        byte failCount = VotesFailBonus;

        foreach (IDType p in VotesFail) {
            failCount += PeoplePermissions[p].Votes;
        }

        return failCount;
    }

    public byte CalculateVotesAbstain () => (byte) (CalculateVotesTotal () - CalculateVotesPass () - CalculateVotesFail ());

    public byte CalculateVotesPassThreshold () {
        byte votesTotal = CalculateVotesTotal ();

        return IsSimpleMajority
            ? (byte) (Math.Floor (votesTotal / 2m) + 1)
            : (byte) Math.Ceiling (votesTotal * 2 / 3m);
    }

    public byte CalculateVotesFailThreshold () {
        byte votesTotal = CalculateVotesTotal ();
        byte votesPassThreshold = CalculateVotesPassThreshold ();

        return IsSimpleMajority
            ? votesPassThreshold
            : (byte) (votesTotal - votesPassThreshold + 1);
    }

    public bool? IsBallotVoted () {
        byte votesPassThreshold = CalculateVotesPassThreshold ();
        byte votesFailThreshold = CalculateVotesFailThreshold ();
        byte votesPass = CalculateVotesPass ();
        byte votesFail = CalculateVotesFail ();

        if (votesPass >= votesPassThreshold) {
            return true;
        } else if (votesFail >= votesFailThreshold) {
            return false;
        } else {
            return null;
        }
    }
}
