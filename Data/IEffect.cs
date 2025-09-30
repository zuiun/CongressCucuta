namespace congress_cucuta.Data;

internal interface IEffect {
    public byte TargetID { get; }
    public byte? ReplacementID { get; }
}

internal readonly struct AddRegionEffect (byte targetID) : IEffect {
    public byte TargetID { get; } = targetID;
    public byte? ReplacementID { get; } = null;
}

internal readonly struct AddPartyEffect (byte targetID) : IEffect {
    public byte TargetID { get; } = targetID;
    public byte? ReplacementID { get; } = null;
}

internal readonly struct RemovePartyEffect (byte targetID) : IEffect {
    public byte TargetID { get; } = targetID;
    public byte? ReplacementID { get; } = null;
}

internal readonly struct ReplacePartyEffect (byte targetID, byte replacementID) : IEffect {
    public byte TargetID { get; } = targetID;
    public byte? ReplacementID { get; } = replacementID;
}

internal readonly struct RemoveProcedureEffect (byte targetID) : IEffect {
    public byte TargetID { get; } = targetID;
    public byte? ReplacementID { get; } = null;
}

internal readonly struct ReplaceProcedureEffect (byte targetID, byte replacementID) : IEffect {
    public byte TargetID { get; } = targetID;
    public byte? ReplacementID { get; } = replacementID;
}
