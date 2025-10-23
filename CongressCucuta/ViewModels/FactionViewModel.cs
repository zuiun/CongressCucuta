using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CongressCucuta.Core;

namespace CongressCucuta.ViewModels;

[ExcludeFromCodeCoverage]
internal class FactionViewModel (IDType id, string name) : ViewModel, IID {
    public static readonly IDType INDEPENDENT = byte.MaxValue;
    private string _name = name;
    private sbyte _value = 0;
    private ObservableCollection<PersonViewModel> _people = [];
    private bool _isCurrency = false;
    private bool _isNotCurrency = true;
    private string _currency = string.Empty;
    private string? _description = null;
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
    public string? Description {
        get => _description;
        set {
            _description = value;
            OnPropertyChanged ();
        }
    }

    public void Sort () => People = [.. _people.OrderBy (p => p.ID)];
}
