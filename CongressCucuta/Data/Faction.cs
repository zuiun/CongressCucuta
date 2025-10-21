using System.Text.Json.Serialization;

namespace CongressCucuta.Data;

internal readonly record struct Faction : IID {
    public IDType ID { get; }
    public bool IsActiveStart { get; }

    [JsonConstructor]
    public Faction (IDType id, bool isActiveStart = true) {
        if (id >= Role.RESERVED_1) {
            throw new ArgumentException ($"Faction ID {id} is reserved by Role", nameof (id));
        }

        ID = id;
        IsActiveStart = isActiveStart;
    }
}
