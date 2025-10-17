using CongressCucuta.Models;

namespace CongressCucuta.ViewModels;

internal class LineViewModel (LineModel line) : ViewModel {
    public string Text => line.Text;
    public byte IndentLevel => line.IndentLevel;
    public bool IsImportant => line.IsImportant;
    public string? Description => line.Description;
    public bool IsContent => line.IsContent;
    public bool IsSubtitle => ! IsContent;
}
