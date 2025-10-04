using congress_cucuta.Data;

namespace congress_cucuta.Models;

internal abstract class SlideModel : IID {
    public IDType ID { get; }
    public string Title { get; }
    public List<LineModel> Description { get; }
    public List<Link<SlideModel>> Links { get; }
    public bool IsContent { get; }

    protected SlideModel (IDType id, string title, List<LineModel> description, List<Link<SlideModel>> links, bool isContent) {
        ID = id;
        Title = title;
        Description = description;
        Links = links;
        IsContent = isContent;
        
        foreach (LineModel line in description) {
            line.IsContent = isContent;
        }
    }
}

internal class SlideLinearModel (
    IDType id,
    string title,
    List<LineModel> description,
    bool isContent = true
) : SlideModel (id, title, description, [new (new AlwaysCondition (), id + 1)], isContent) { }

internal class SlideBranchingModel (
    IDType id,
    string title,
    List<LineModel> description,
    List<Link<SlideModel>> links
) : SlideModel (id, title, description, links, true) { }

internal class SlideConstantModel (
    IDType id,
    string title,
    List<LineModel> description
) : SlideModel (id, title, description, [], false) { }
