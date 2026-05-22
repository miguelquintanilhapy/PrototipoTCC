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
    public partial class EditarFuncionarioDialog : Window
    {
        private readonly Funcionario _funcionario;

        public EditarFuncionarioDialog(Funcionario funcionario)
        {
            InitializeComponent();

            _funcionario = funcionario;

            CarregarCargos();

            PreencherCampos();
        }

        private void CarregarCargos()
        {
            var repo = new CargoRepository();

            ComboCargo.ItemsSource = repo.ListarTodos();
        }

        private void PreencherCampos()
        {
            TxtNome.Text = _funcionario.nome;
            
            ComboCargo.SelectedValue = _funcionario.id_cargo;

            // NÍVEL
            foreach (ComboBoxItem item in ComboNivel.Items)
            {
                if (item.Content.ToString() == _funcionario.nivel)
                {
                    ComboNivel.SelectedItem = item;
                    break;
                }
            }

            // TIPO VÍNCULO
            foreach (ComboBoxItem item in ComboTipoVinculo.Items)
            {
                if (item.Content.ToString() == _funcionario.tipo_vinculo)
                {
                    ComboTipoVinculo.SelectedItem = item;
                    break;
                }
            }

            // STATUS
            foreach (ComboBoxItem item in ComboStatus.Items)
            {
                if (item.Content.ToString() == _funcionario.status)
                {
                    ComboStatus.SelectedItem = item;
                    break;
                }
            }
        }

        private void BtnSalvar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNome.Text) ||
                ComboCargo.SelectedValue == null ||
                ComboNivel.SelectedItem == null ||                
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

           

            try
            {
                _funcionario.nome = TxtNome.Text.Trim();

                _funcionario.id_cargo = (int)ComboCargo.SelectedValue;

                _funcionario.nivel = ((ComboBoxItem)ComboNivel.SelectedItem)
                    .Content
                    .ToString();
            
                _funcionario.tipo_vinculo = ((ComboBoxItem)ComboTipoVinculo.SelectedItem)
                    .Content
                    .ToString();

                _funcionario.status = ((ComboBoxItem)ComboStatus.SelectedItem)
                    .Content
                    .ToString();

                var repo = new FuncionarioRepository();

                repo.Atualizar(_funcionario);

                MessageBox.Show(
                    "Funcionário atualizado com sucesso!",
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