using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal class PersonModel () : IID {
    public IDType ID { get; set; } = 0;
    public string Name { get; set; } = "Name";
    public List<string> Roles { get; set; } = [];
    public HashSet<IDType> Factions { get; set; } = [];
}
