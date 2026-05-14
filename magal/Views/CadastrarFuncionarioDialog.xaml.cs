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
using magal.Data.Repositories;
using magal.Models;

namespace magal.Views
{
    public partial class CadastrarFuncionarioDialog : Window
    {
        public CadastrarFuncionarioDialog()
        {
            InitializeComponent();
            CarregarCargos();
        }

        private void CarregarCargos()
        {
            var repo = new CargoRepository();
            ComboCargo.ItemsSource = repo.ListarTodos();
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                ComboCargo.SelectedValue == null ||
                ComboNivel.SelectedItem == null ||
                string.IsNullOrWhiteSpace(TxtCustoHora.Text) ||
                ComboTipoVinculo.SelectedItem == null ||
                ComboStatus.SelectedItem == null)
            {
                MessageBox.Show(
                    "Preencha todos os campos.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(TxtCustoHora.Text, out decimal custoHora))
            {
                MessageBox.Show(
                    "Custo/Hora inválido.",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            try
            {
                var funcionario = new Funcionario
                {
                    nome = TxtNome.Text.Trim(),

                    id_cargo = (int)ComboCargo.SelectedValue,

                    nivel = ((ComboBoxItem)ComboNivel.SelectedItem)
                        .Content
                        .ToString(),

                    custo_hora = custoHora,

                    tipo_vinculo = ((ComboBoxItem)ComboTipoVinculo.SelectedItem)
                        .Content
                        .ToString(),

                    status = ((ComboBoxItem)ComboStatus.SelectedItem)
                        .Content
                        .ToString()
                };

                var repo = new FuncionarioRepository();

                repo.Inserir(funcionario);

                MessageBox.Show(
                    "Funcionário cadastrado com sucesso!",
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