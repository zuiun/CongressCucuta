using System.ComponentModel;
using System.Windows;

namespace congress_cucuta.Views;

public partial class ElectionWindow : Window {
    private bool _canClose = false;

    public ElectionWindow () {
        InitializeComponent ();
    }

    public void Election_CompletedElectionEventHandler () => _canClose = true;

    protected override void OnClosing (CancelEventArgs e) {
        if (! _canClose) {
            e.Cancel = true;
        }
    }
}
