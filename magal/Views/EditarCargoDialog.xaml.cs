using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using magal.Models;
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class EditarCargoDialog : Window
    {
        private readonly Cargo _cargo;

        public EditarCargoDialog(Cargo cargo)
        {
            InitializeComponent();

            _cargo = cargo;

            PreencherCampos();
        }

        private void PreencherCampos()
        {
            TxtNome.Text = _cargo.nome;

            TxtCustoHora.Text = _cargo.custo_medio_hora.ToString("F2");


            foreach (ComboBoxItem item in ComboNivel.Items)
            {
                if (item.Content.ToString() == _cargo.nivel)
                {
                    ComboNivel.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                ComboNivel.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtCustoHora.Text))
            {
                MessageBox.Show(
                    "Preencha todos os campos obrigatórios.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(
                    TxtCustoHora.Text,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.CurrentCulture,
                    out decimal custoHora))
            {
                MessageBox.Show(
                    "Valor inválido para custo/hora.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                _cargo.nome = TxtNome.Text.Trim();

                _cargo.nivel = ((ComboBoxItem)ComboNivel.SelectedItem)
                    .Content
                    .ToString();

                _cargo.custo_medio_hora = custoHora;

                var repo = new CargoRepository();

                repo.Atualizar(_cargo);

                MessageBox.Show(
                    "Cargo atualizado com sucesso!",
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
