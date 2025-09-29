namespace congress_cucuta.Models;

internal class SlideModel (string title, List<LineModel> description) {
    public string Title { get; set; } = title;
    public List<LineModel> Description { get; } = description;
}
