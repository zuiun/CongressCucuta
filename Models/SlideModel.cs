using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal abstract class SlideModel : IID {
    public IDType ID { get; }
    public string Title { get; }
    public List<LineModel> Description { get; }
    public List<Link<SlideModel>> Links { get; }
    public bool IsContent { get; }
    public bool? IsForward { get; }

    protected SlideModel (IDType id, string title, List<LineModel> description, List<Link<SlideModel>> links, bool isContent, bool? isForward) {
        ID = id;
        Title = title;
        Description = description;
        Links = links;
        IsContent = isContent;
        IsForward = isForward;
        
        foreach (LineModel line in description) {
            line.IsContent = isContent;
        }
    }
}

internal class SlideForwardModel (
    IDType id,
    string title,
    List<LineModel> description,
    bool isContent = true
) : SlideModel (id, title, description, [new (new AlwaysCondition (), id + 1)], isContent, true) { }

internal class SlideBackwardModel (
    IDType id,
    string title,
    List<LineModel> description,
    bool isContent = false
) : SlideModel (id, title, description, [new (new AlwaysCondition (), id - 1)], isContent, false) { }

internal class SlideBidirectionalModel (
    IDType id,
    string title,
    List<LineModel> description,
    bool isContent = true
) : SlideModel (id, title, description, [new (new AlwaysCondition (), id - 1), new (new AlwaysCondition (), id + 1)], isContent, false) { }

internal class SlideBranchingModel (
    IDType id,
    string title,
    List<LineModel> description,
    List<Link<SlideModel>> links
) : SlideModel (id, title, description, links, true, null) { }

internal class SlideConstantModel (
    IDType id,
    string title,
    List<LineModel> description,
    bool isContent = false
) : SlideModel (id, title, description, [], isContent, null) { }
