using SAD.Services;
using SAD.ViewModels;
using System.Windows;

namespace SAD.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new OrcamentoViewModel(new PdfService());
        }
    }
}