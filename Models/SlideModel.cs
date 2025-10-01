using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal abstract class SlideModel (IDType id, string title, List<LineModel> description) : IID {
    public IDType ID { get; } = id;
    public string Title { get; } = title;
    public List<LineModel> Description { get; } = description;

    public abstract List<Link<SlideModel>> YieldLinks ();
    public abstract IDType? YieldNext (ref readonly SimulationContext context);
}

internal class SlideLinearModel (
    IDType id,
    string title,
    List<LineModel> description
) : SlideModel (id, title, description) {
    override public List<Link<SlideModel>> YieldLinks () => [new (new AlwaysCondition (), ID + 1)];

    override public IDType? YieldNext (ref readonly SimulationContext context) => ID + 1;
}

internal class SlideBranchingModel (
    IDType id,
    string title,
    List<LineModel> description,
    List<Link<SlideModel>> links
) : SlideModel(id, title, description) {
    public List<Link<SlideModel>> Links { get; } = links;

    override public List<Link<SlideModel>> YieldLinks () => Links;

    override public IDType? YieldNext (ref readonly SimulationContext context) {
        foreach (var link in Links) {
            if (link.Evaluate (in context) is IDType evaluation) {
                return evaluation;
            }
        }

        return null;
    }
}

internal class SlideConstantModel (
    IDType id,
    string title,
    List<LineModel> description
) : SlideModel (id, title, description) {
    override public List<Link<SlideModel>> YieldLinks () => [];

    override public IDType? YieldNext (ref readonly SimulationContext context) => ID;
}
