using System;
using System.Windows;
using System.Windows.Controls;
using magal.Data.Repositories;
using magal.Models;

namespace magal.Views
{
    public partial class CadastrarCustoDialog : Window
    {
        public CadastrarCustoDialog()
        {
            InitializeComponent();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Validação dos campos obrigatórios (sem o combo de projeto)
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                ComboCategoria.SelectedItem == null ||
                ComboTipo.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtValor.Text))
            {
                MessageBox.Show(
                    "Preencha todos os campos.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Validação do formato do valor numérico
            if (!decimal.TryParse(TxtValor.Text.Trim(), out decimal valorConvertido) || valorConvertido < 0)
            {
                MessageBox.Show(
                    "Por favor, insira um valor numérico válido e positivo para o custo.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Instancia o modelo apenas com as informações locais da tela
                var custo = new Custo
                {
                    nome = TxtNome.Text.Trim(),

                    categoria = ((ComboBoxItem)ComboCategoria.SelectedItem)
                        .Content
                        .ToString(),

                    tipo = ((ComboBoxItem)ComboTipo.SelectedItem)
                        .Content
                        .ToString(),

                    valor = valorConvertido
                };

                // Executa a persistência através do repositório
                var repo = new CustoRepository();
                repo.Inserir(custo);

                MessageBox.Show(
                    "Custo lançado com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar custo: " + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}