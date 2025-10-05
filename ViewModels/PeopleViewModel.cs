using System.Collections.ObjectModel;
using congress_cucuta.Models;

namespace congress_cucuta.ViewModels;

internal class PeopleViewModel : ViewModel {
    private List<PersonModel> _people = [];
    private ObservableCollection<string> _names = [];
    public ObservableCollection<string> Names {
        get => _names;
        set => _names = value;
    }

}
