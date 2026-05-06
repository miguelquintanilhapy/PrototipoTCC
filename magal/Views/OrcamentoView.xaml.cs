using magal.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace magal.Views
{
    public partial class OrcamentoView : UserControl
    {
        public OrcamentoView()
        {
            InitializeComponent(); 
            this.DataContext = new OrcamentoViewModel();
        }

        private void ValidarEntradaSemNegativo(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "-")
            {
                MessageBox.Show("Não é permitido valores negativos.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                e.Handled = true;
                return;
            }

            Regex regex = new Regex("[^0-9,]+");
            bool temCaractereInvalido = regex.IsMatch(e.Text);

            if (temCaractereInvalido)
            {
                MessageBox.Show("Este campo aceita apenas números positivos.", "Entrada Inválida", MessageBoxButton.OK, MessageBoxImage.Information);
                e.Handled = true; // Bloqueia a entrada da letra/símbolo
            }
        }

    }
}