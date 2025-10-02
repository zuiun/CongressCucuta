using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal abstract class SlideModel : IID {
    public IDType ID { get; }
    public string Title { get; }
    public List<LineModel> Description { get; }
    
    protected SlideModel (IDType id, string title, List<LineModel> description) {
        ID = id;
        Title = title;
        Description = description;
        
        if (Description.Count <= 1) {
            foreach (LineModel line in description) {
                line.IsContent = false;
            }
        }
    }

    public abstract List<Link<SlideModel>> YieldLinks ();

    public abstract IDType? YieldNext (ref readonly SimulationContext context);
}

internal class SlideLinearModel (
    IDType id,
    string title,
    List<LineModel> description
) : SlideModel (id, title, description) {
    public override List<Link<SlideModel>> YieldLinks () => [new (new AlwaysCondition (), ID + 1)];

    public override IDType? YieldNext (ref readonly SimulationContext context) => ID + 1;
}

internal class SlideBranchingModel (
    IDType id,
    string title,
    List<LineModel> description,
    List<Link<SlideModel>> links
) : SlideModel (id, title, description) {
    private readonly List<Link<SlideModel>> _links = links;

    public override List<Link<SlideModel>> YieldLinks () => _links;

    public override IDType? YieldNext (ref readonly SimulationContext context) {
        foreach (Link<SlideModel> link in _links) {
            if (link.Evaluate (in context) ?? false) {
                return link.TargetID;
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
    public override List<Link<SlideModel>> YieldLinks () => [];

    public override IDType? YieldNext (ref readonly SimulationContext context) => null;
}
