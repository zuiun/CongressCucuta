using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace CongressCucuta.Views;

[ExcludeFromCodeCoverage]
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
