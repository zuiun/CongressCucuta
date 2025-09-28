using System.Windows.Controls;
using congress_cucuta.ViewModels;

namespace congress_cucuta.Views {
    public partial class LineView : UserControl {
        public LineView () {
            InitializeComponent ();
            this.DataContext = new LineViewModel ();
        }
    }
}
