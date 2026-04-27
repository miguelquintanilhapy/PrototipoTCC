using System.Windows.Controls;
using magal.ViewModels;

namespace magal.Views
{
    public partial class OrcamentoView : UserControl
    {
        public OrcamentoView()
        {
            InitializeComponent(); // Verifique se tem o ; aqui
            this.DataContext = new OrcamentoViewModel(); // E aqui
        }
    }
}