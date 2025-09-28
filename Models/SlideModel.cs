using System.Collections.ObjectModel;

namespace congress_cucuta.Models;

public class SlideModel (string title, ObservableCollection<LineModel> description) {
    public string Title { get; set; } = title;
    public ObservableCollection<LineModel> Description { get; } = description;
}
