using System;
using System.Windows;
using System.Windows.Controls;
using magal.Data.Repositories;
using magal.Models;

namespace magal.Views
{
    public partial class EditarCustoDialog : Window
    {
        private readonly Custo _custo;

        public EditarCustoDialog(Custo custo)
        {
            InitializeComponent();

            _custo = custo;

            PreencherCampos();
        }

        private void PreencherCampos()
        {
            TxtNome.Text = _custo.nome;

            // Atribui o valor convertendo para string (pode usar .ToString("F2") se preferir fixar duas casas decimais)
            TxtValor.Text = _custo.valor.ToString();

            // Seleciona a CATEGORIA correspondente no ComboBox
            foreach (ComboBoxItem item in ComboCategoria.Items)
            {
                if (item.Content.ToString() == _custo.categoria)
                {
                    ComboCategoria.SelectedItem = item;
                    break;
                }
            }

            // Seleciona o TIPO correspondente no ComboBox
            foreach (ComboBoxItem item in ComboTipo.Items)
            {
                if (item.Content.ToString() == _custo.tipo)
                {
                    ComboTipo.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            // Validação dos campos obrigatórios
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

            // Validação do formato numérico do valor
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
                // Atualiza o objeto com os dados modificados da tela
                _custo.nome = TxtNome.Text.Trim();
                _custo.valor = valorConvertido;

                _custo.categoria = ((ComboBoxItem)ComboCategoria.SelectedItem)
                    .Content
                    .ToString();

                _custo.tipo = ((ComboBoxItem)ComboTipo.SelectedItem)
                    .Content
                    .ToString();

                // Persiste as alterações no banco de dados via repositório
                var repo = new CustoRepository();
                repo.Atualizar(_custo);

                MessageBox.Show(
                    "Custo atualizado com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao salvar: " + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}