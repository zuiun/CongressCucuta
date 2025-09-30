namespace congress_cucuta.Data;

internal interface IProcedure : IID {
    
}

internal class ProcedureGovernmental : IProcedure {
    public byte ID { get; } = 0;

}

internal class ProcedureSpecial : IProcedure {
    public byte ID { get; } = 0;

}

internal class ProcedureDeclared : IProcedure {
    public byte ID { get; } = 0;

}
