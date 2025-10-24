using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

internal class LinkViewModel (string name, Link<SlideViewModel> link) : ViewModel {
    public string Name => name;
    public Link<SlideViewModel> Link => link;
}
