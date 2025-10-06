using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal class PersonModel (IDType id, string name) : IID {
    public IDType ID { get; set; } = id;
    public string Name { get; set; } = name;
    public List<string> Roles { get; set; } = [];
    public HashSet<IDType> Factions { get; set; } = [];
}
