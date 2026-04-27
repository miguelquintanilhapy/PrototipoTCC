using System.Windows;
using magal.Views; // Certifique-se que a pasta Views existe

namespace magal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Opcional: Já iniciar com a tela de orçamento aberta
            AbrirOrcamento();
        }

        private void BtnOrcamentos_Click(object sender, RoutedEventArgs e)
        {
            AbrirOrcamento();
        }

        private void AbrirOrcamento()
        {
            // Instancia a View que criamos e coloca no ContentControl
            MainContent.Content = new OrcamentoView();
        }
    }
}