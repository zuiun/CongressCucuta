using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal abstract class SlideModel (byte id, string title, List<LineModel> description) : IID {
    public byte ID { get; } = id;
    public string Title { get; } = title;
    public List<LineModel> Description { get; } = description;

    public abstract byte? YieldNext (ref readonly SimulationContext context);
}

internal class SlideLinearModel (
    byte id,
    string title,
    List<LineModel> description
) : SlideModel (id, title, description) {
    override public byte? YieldNext (ref readonly SimulationContext context) => (byte) (ID + 1);
}

internal class SlideBranchingModel (
    byte id,
    string title,
    List<LineModel> description,
    List<Link<SlideModel>> links
) : SlideModel(id, title, description) {
    public List<Link<SlideModel>> Links { get; } = links;

    override public byte? YieldNext (ref readonly SimulationContext context) {
        foreach (var link in Links) {
            if (link.Evaluate (in context) is byte evaluation) {
                return evaluation;
            }
        }

        return null;
    }
}

internal class SlideConstantModel (
    byte id,
    string title,
    List<LineModel> description
) : SlideModel (id, title, description) {
    override public byte? YieldNext (ref readonly SimulationContext context) => null;
}
