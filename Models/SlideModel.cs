namespace congress_cucuta.Models;

public class SlideModel (string title, List<LineModel> description) {
    public string Title { get; set; } = title;
    public List<LineModel> Description { get; } = description;
}
