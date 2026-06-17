using System.Windows.Controls;
using magal.ViewModels;

namespace magal.Views
{
    public partial class CustoView : UserControl
    {
        public CustoView()
        {
            InitializeComponent();
            DataContext = new CustoViewModel();
        }
    }
}