using System.Collections.ObjectModel;
using CongressCucuta.Data;

namespace CongressCucuta.ViewModels;

internal class FactionViewModel (IDType id, string name) : ViewModel, IID {
    public static readonly IDType INDEPENDENT = byte.MaxValue;
    private string _name = name;
    private sbyte _value = 0;
    private ObservableCollection<PersonViewModel> _people = [];
    private bool _isCurrency = false;
    private bool _isNotCurrency = true;
    private string _currency = string.Empty;
    public IDType ID => id;
    public string Name {
        get => _name;
        set {
            _name = value;
            OnPropertyChanged ();
        }
    }
    public sbyte Value {
        get => _value;
        set {
            _value = value;
            OnPropertyChanged ();
        }
    }
    public ObservableCollection<PersonViewModel> People {
        get => _people;
        set {
            _people = value;
            OnPropertyChanged ();
        }
    }
    public bool IsCurrency {
        get => _isCurrency;
        set {
            _isCurrency = value;
            OnPropertyChanged ();
        }
    }
    public bool IsNotCurrency {
        get => _isNotCurrency;
        set {
            _isNotCurrency = value;
            OnPropertyChanged ();
        }
    }
    public string Currency {
        get => _currency;
        set {
            _currency = value;
            OnPropertyChanged ();
        }
    }

    public void Sort () => People = [.. _people.OrderBy (p => p.ID)];

    public void UpdatePermissions (Dictionary<IDType, Permissions> peoplePermissions) {
        foreach (PersonViewModel p in _people) {
            p.UpdatePermissions (peoplePermissions[p.ID]);
        }
    }

    public void SetInteractability (bool isInteractable) {
        foreach (PersonViewModel p in _people) {
            p.IsInteractable = isInteractable;
        }
    }
}
