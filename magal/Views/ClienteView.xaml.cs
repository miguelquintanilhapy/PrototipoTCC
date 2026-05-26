using System;
using System.Windows;
using System.Windows.Controls;
using magal.ViewModels;
using magal.Models;
using magal.Data.Repositories;

namespace magal.Views
{
    public partial class ClienteView : UserControl
    {
        public ClienteView()
        {
            InitializeComponent();
            DataContext = new ClienteViewModel();
        }

        private void BtnNovoCliente_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CadastrarClienteDialog();
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                // Atualiza o DataContext para recarregar a lista na tela
                DataContext = new ClienteViewModel();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null)
                return;

            var cliente = button.DataContext as Cliente;

            if (cliente == null)
                return;

            var dialog = new EditarClienteDialog(cliente);
            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true)
            {
                // Atualiza o DataContext para recarregar a lista com as alterações
                DataContext = new ClienteViewModel();
            }
        }

        private void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button == null)
                return;

            var cliente = button.DataContext as Cliente;

            if (cliente == null)
                return;

            var resultado = MessageBox.Show(
                $"Deseja realmente excluir o cliente '{cliente.nome}'?",
                "Aero Concepts",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (resultado != MessageBoxResult.Yes)
                return;

            try
            {
                var repo = new ClienteRepository();

                repo.Excluir(cliente.id_cliente);

                MessageBox.Show(
                    "Cliente excluído com sucesso!",
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Atualiza o DataContext para remover o cliente excluído da listagem
                DataContext = new ClienteViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao excluir: " + ex.Message,
                    "Aero Concepts",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}