using SAD.Services;
using SAD.ViewModels;
using System.Windows;

namespace SAD.Views
{
    /// <summary>
    /// Code-behind mínimo — apenas instancia ViewModel e injeta dependências.
    /// Nenhuma lógica de negócio aqui. Tudo está no OrcamentoViewModel.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Composição de dependências (em produção, use um DI container como Microsoft.Extensions.DI)
            var pdfService = new PdfService();
            DataContext = new OrcamentoViewModel(pdfService);
        }
    }
}
