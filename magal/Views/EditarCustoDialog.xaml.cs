using System;
using System.Windows;
using System.Windows.Controls;
using magal.Data.Repositories;
using magal.Models;

namespace magal.Views
{
    public partial class EditarCustoDialog : Window
    {
        private readonly CatalogoCusto _custoItem;

        public EditarCustoDialog(CatalogoCusto custoItem)
        {
            InitializeComponent();
            _custoItem = custoItem;
            PreencherCampos();
        }

        private void PreencherCampos()
        {
            TxtNome.Text = _custoItem.nome;
            TxtValor.Text = _custoItem.valor.ToString();

            foreach (ComboBoxItem item in ComboCategoria.Items)
            {
                if (item.Content.ToString() == _custoItem.categoria)
                {
                    ComboCategoria.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                ComboCategoria.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtValor.Text))
            {
                MessageBox.Show(
                    "Preencha todos os campos.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtValor.Text.Trim(), out decimal valorConvertido) || valorConvertido < 0)
            {
                MessageBox.Show(
                    "Por favor, insira um valor numérico válido e positivo.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            try
            {
                _custoItem.nome = TxtNome.Text.Trim();
                _custoItem.categoria = ((ComboBoxItem)ComboCategoria.SelectedItem).Content.ToString();
                _custoItem.valor = valorConvertido;

                // CORRIGIDO: Instancia o repositório mestre do catálogo para salvar as alterações globais
                var repo = new CatalogoCustoRepository();
                repo.Atualizar(_custoItem);

                MessageBox.Show(
                    "Item do catálogo atualizado com sucesso!",
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